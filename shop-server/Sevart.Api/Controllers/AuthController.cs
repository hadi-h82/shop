using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevart.Api.Contracts.Auth;
using Sevart.Application.Abstractions.Identity;
using Microsoft.AspNetCore.RateLimiting;

namespace Sevart.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private const string RefreshTokenCookieName =
    "sevart.refresh_token";
    private readonly IAuthService _authService;

    private readonly IWebHostEnvironment _environment;

    public AuthController(
        IAuthService authService,
        IWebHostEnvironment environment)
    {
        _authService = authService;
        _environment = environment;
    }


    [AllowAnonymous]
    [EnableRateLimiting("OtpRequestPolicy")]
    [HttpPost("request-otp")]
    [ProducesResponseType(
    typeof(RequestOtpResponse),
    StatusCodes.Status202Accepted)]
    [ProducesResponseType(
    typeof(RequestOtpErrorResponse),
    StatusCodes.Status400BadRequest)]
    [ProducesResponseType(
    typeof(RequestOtpErrorResponse),
    StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<RequestOtpResponse>> RequestOtp(
    [FromBody] RequestOtpRequest request,
    CancellationToken cancellationToken)
    {
        var command =
            new RequestOtpCommand(
                PhoneNumber: request.PhoneNumber,
                IpAddress:
                    HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString());

        var result =
            await _authService.RequestOtpAsync(
                command,
                cancellationToken);

        if (!result.Succeeded ||
            result.ChallengeId is null ||
            result.ExpiresInSeconds is null)
        {
            var errorResponse =
                new RequestOtpErrorResponse(
                    Errors:
                        result.Errors
                            .Select(
                                error =>
                                    new AuthErrorItem(
                                        error.Code,
                                        error.Description))
                            .ToArray(),
                    RetryAfterSeconds:
                        result.RetryAfterSeconds);

            if (result.RetryAfterSeconds > 0)
            {
                Response.Headers["Retry-After"] =
                    result.RetryAfterSeconds.ToString();

                return StatusCode(
                    StatusCodes.Status429TooManyRequests,
                    errorResponse);
            }

            return BadRequest(errorResponse);
        }

        var response =
            new RequestOtpResponse(
                ChallengeId:
                    result.ChallengeId.Value,
                Message:
                    "در صورت معتبر بودن شماره، کد ارسال شد.",
                ExpiresInSeconds:
                    result.ExpiresInSeconds.Value,
                RetryAfterSeconds:
                    result.RetryAfterSeconds);

        return StatusCode(
            StatusCodes.Status202Accepted,
            response);
    }


    [AllowAnonymous]
    [EnableRateLimiting("OtpVerifyPolicy")]
    [HttpPost("verify-otp")]
    [ProducesResponseType(
    typeof(AuthResponse),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    typeof(AuthErrorResponse),
    StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> VerifyOtp(
    [FromBody] VerifyOtpRequest request,
    CancellationToken cancellationToken)
    {
        var command =
            new VerifyOtpCommand(
                ChallengeId: request.ChallengeId,
                Code: request.Code,
                IpAddress:
                    HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString());

        var result =
            await _authService.VerifyOtpAsync(
                command,
                cancellationToken);

        if (!result.Succeeded ||
            result.Session is null)
        {
            var errorResponse =
                new AuthErrorResponse(
                    result.Errors
                        .Select(
                            error =>
                                new AuthErrorItem(
                                    error.Code,
                                    error.Description))
                        .ToArray());

            return BadRequest(errorResponse);
        }

        var session = result.Session;

        AppendRefreshTokenCookie(session);
        SetSensitiveResponseHeaders();

        var response =
            new AuthResponse(
                UserId: session.UserId,
                FullName: session.FullName,
                PhoneNumber: session.PhoneNumber,
                Email: session.Email,
                Roles: session.Roles,
                TokenType: "Bearer",
                AccessToken: session.AccessToken,
                AccessTokenExpiresAt:
                    session.AccessTokenExpiresAt);

        return Ok(response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(
    typeof(AuthResponse),
    StatusCodes.Status200OK)]
    [ProducesResponseType(
    typeof(AuthErrorResponse),
    StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<AuthResponse>> Refresh(
    CancellationToken cancellationToken)
    {
        if (!Request.Cookies.TryGetValue(
                RefreshTokenCookieName,
                out var refreshToken) ||
            string.IsNullOrWhiteSpace(refreshToken))
        {
            DeleteRefreshTokenCookie();

            return Unauthorized(
                new AuthErrorResponse(
                [
                    new AuthErrorItem(
                    "MissingRefreshToken",
                    "نشست کاربری معتبر نیست.")
                ]));
        }

        var command =
            new RefreshSessionCommand(
                RefreshToken: refreshToken,
                IpAddress:
                    HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString());

        var result =
            await _authService.RefreshSessionAsync(
                command,
                cancellationToken);

        if (!result.Succeeded ||
            result.Session is null)
        {
            DeleteRefreshTokenCookie();

            return Unauthorized(
                new AuthErrorResponse(
                    result.Errors
                        .Select(
                            error =>
                                new AuthErrorItem(
                                    error.Code,
                                    error.Description))
                        .ToArray()));
        }

        var session = result.Session;

        AppendRefreshTokenCookie(session);

        SetSensitiveResponseHeaders();

        var response =
            new AuthResponse(
                UserId: session.UserId,
                FullName: session.FullName,
                PhoneNumber: session.PhoneNumber,
                Email: session.Email,
                Roles: session.Roles,
                TokenType: "Bearer",
                AccessToken: session.AccessToken,
                AccessTokenExpiresAt:
                    session.AccessTokenExpiresAt);

        return Ok(response);
    }



    [AllowAnonymous]
    [HttpPost("logout")]
    [ProducesResponseType(
    StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Logout(
    CancellationToken cancellationToken)
    {
        Request.Cookies.TryGetValue(
            RefreshTokenCookieName,
            out var refreshToken);

        var command =
            new LogoutCommand(
                RefreshToken: refreshToken,
                IpAddress:
                    HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString());

        await _authService.LogoutAsync(
            command,
            cancellationToken);

        DeleteRefreshTokenCookie();

        return NoContent();
    }


    [Authorize]
    [HttpPost("logout-all")]
    [ProducesResponseType(
    StatusCodes.Status204NoContent)]
    [ProducesResponseType(
    StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> LogoutAll(
    CancellationToken cancellationToken)
    {
        var userIdValue =
            User.FindFirst("sub")?
                .Value;

        if (!Guid.TryParse(
                userIdValue,
                out var userId))
        {
            return Unauthorized();
        }

        var command =
            new LogoutAllSessionsCommand(
                UserId: userId,
                IpAddress:
                    HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString());

        await _authService.LogoutAllSessionsAsync(
            command,
            cancellationToken);

        DeleteRefreshTokenCookie();

        return NoContent();
    }





    private void AppendRefreshTokenCookie(
        AuthSession session)
    {
        var isDevelopment =
            _environment.IsDevelopment();

        var isHttps =
            Request.IsHttps;

        var cookieOptions =
            new CookieOptions
            {
                HttpOnly = true,

                Secure =
                    !isDevelopment ||
                    isHttps,

                SameSite =
                    isDevelopment && isHttps
                        ? SameSiteMode.None
                        : SameSiteMode.Lax,

                IsEssential = true,

                Path = "/api/auth",

                Expires =
                    new DateTimeOffset(
                        session.RefreshTokenExpiresAt)
            };

        Response.Cookies.Append(
            RefreshTokenCookieName,
            session.RefreshToken,
            cookieOptions);
    }


    private void DeleteRefreshTokenCookie()
    {
        var isDevelopment =
            _environment.IsDevelopment();

        var isHttps =
            Request.IsHttps;

        Response.Cookies.Delete(
            RefreshTokenCookieName,
            new CookieOptions
            {
                HttpOnly = true,

                Secure =
                    !isDevelopment ||
                    isHttps,

                SameSite =
                    isDevelopment && isHttps
                        ? SameSiteMode.None
                        : SameSiteMode.Lax,

                IsEssential = true,

                Path = "/api/auth"
            });
    }


    private void SetSensitiveResponseHeaders()
    {
        Response.Headers["Cache-Control"] =
            "no-store, no-cache, max-age=0";

        Response.Headers["Pragma"] =
            "no-cache";
    }
}