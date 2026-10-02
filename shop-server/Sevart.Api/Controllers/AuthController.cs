using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sevart.Api.Contracts.Auth;
using Sevart.Application.Abstractions.Identity;

namespace Sevart.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(
        IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    [ProducesResponseType(
        typeof(AuthResponse),
        StatusCodes.Status201Created)]
    [ProducesResponseType(
        typeof(AuthErrorResponse),
        StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<AuthResponse>> Register(
        [FromBody] RegisterRequest request,
        CancellationToken cancellationToken)
    {
        var command =
            new RegisterUserCommand(
                FullName: request.FullName,
                Email: request.Email,
                PhoneNumber: request.PhoneNumber,
                Password: request.Password,
                IpAddress:
                    HttpContext.Connection
                        .RemoteIpAddress?
                        .ToString());

        var result =
            await _authService.RegisterAsync(
                command,
                cancellationToken);

        if (!result.Succeeded ||
            result.Session is null)
        {
            var errorResponse =
                new AuthErrorResponse(
                    result.Errors
                        .Select(
                            error => new AuthErrorItem(
                                error.Code,
                                error.Description))
                        .ToArray());

            return BadRequest(errorResponse);
        }

        var session = result.Session;

        var response =
            new AuthResponse(
                UserId: session.UserId,
                FullName: session.FullName,
                Email: session.Email,
                Roles: session.Roles,
                TokenType: "Bearer",
                AccessToken: session.AccessToken,
                AccessTokenExpiresAt:
                    session.AccessTokenExpiresAt,
                RefreshToken: session.RefreshToken,
                RefreshTokenExpiresAt:
                    session.RefreshTokenExpiresAt);

        return StatusCode(
            StatusCodes.Status201Created,
            response);
    }
}