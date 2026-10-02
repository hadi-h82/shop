using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sevart.Application.Abstractions.Identity;
using Sevart.Infrastructure.Persistence;

namespace Sevart.Infrastructure.Identity;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SevartDbContext _dbContext;
    private readonly ITokenService _tokenService;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SevartDbContext dbContext,
        ITokenService tokenService)
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _tokenService = tokenService;
    }

    public async Task<RegisterUserResult> RegisterAsync(
        RegisterUserCommand command,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var email =
            command.Email
                .Trim()
                .ToLowerInvariant();

        var existingUser =
            await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return RegisterUserResult.Failure(
            [
                new AuthError(
                    "DuplicateEmail",
                    "این ایمیل قبلاً ثبت شده است.")
            ]);
        }

        var user =
            new ApplicationUser(
                command.FullName,
                email,
                command.PhoneNumber);

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken);

        var createResult =
            await _userManager.CreateAsync(
                user,
                command.Password);

        if (!createResult.Succeeded)
        {
            return RegisterUserResult.Failure(
                MapErrors(createResult.Errors));
        }

        var roleResult =
            await _userManager.AddToRoleAsync(
                user,
                ApplicationRoles.Customer);

        if (!roleResult.Succeeded)
        {
            return RegisterUserResult.Failure(
                MapErrors(roleResult.Errors));
        }

        var roles =
            (await _userManager.GetRolesAsync(user))
                .ToArray();

        var generatedTokens =
            _tokenService.GenerateTokens(
                user.Id,
                user.Email!,
                user.FullName,
                roles);

        var refreshToken =
            new RefreshToken(
                generatedTokens.RefreshTokenHash,
                generatedTokens.RefreshTokenExpiresAt,
                command.IpAddress);

        user.AddRefreshToken(refreshToken);

        _dbContext.Entry(user).State =
            EntityState.Unchanged;

        _dbContext.RefreshTokens.Add(
            refreshToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await transaction.CommitAsync(
            cancellationToken);

        var session =
            new AuthSession(
                UserId: user.Id,
                FullName: user.FullName,
                Email: user.Email!,
                Roles: roles,
                AccessToken:
                    generatedTokens.AccessToken,
                AccessTokenExpiresAt:
                    generatedTokens.AccessTokenExpiresAt,
                RefreshToken:
                    generatedTokens.RefreshToken,
                RefreshTokenExpiresAt:
                    generatedTokens.RefreshTokenExpiresAt);

        return RegisterUserResult.Success(session);
    }

    private static IReadOnlyCollection<AuthError> MapErrors(
        IEnumerable<IdentityError> errors)
    {
        return errors
            .Select(
                error => new AuthError(
                    error.Code,
                    error.Description))
            .ToArray();
    }
}