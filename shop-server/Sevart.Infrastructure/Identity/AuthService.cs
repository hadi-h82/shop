using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sevart.Application.Abstractions.Identity;
using Sevart.Infrastructure.Persistence;
using Microsoft.Extensions.Options;

namespace Sevart.Infrastructure.Identity;

public sealed class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SevartDbContext _dbContext;
    private readonly ITokenService _tokenService;
    private readonly IOtpCodeService _otpCodeService;
    private readonly IOtpMessageSender _otpMessageSender;
    private readonly OtpOptions _otpOptions;
    private readonly JwtOptions _jwtOptions;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SevartDbContext dbContext,
        ITokenService tokenService,
        IOtpCodeService otpCodeService,
        IOtpMessageSender otpMessageSender,
        IOptions<OtpOptions> otpOptions,
        IOptions<JwtOptions> jwtOptions
        )
    {
        _userManager = userManager;
        _dbContext = dbContext;
        _tokenService = tokenService;
        _otpCodeService = otpCodeService;
        _otpMessageSender = otpMessageSender;
        _otpOptions = otpOptions.Value;
        _otpOptions = otpOptions.Value;
    }

    public async Task<RequestOtpResult> RequestOtpAsync(
        RequestOtpCommand command,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!IranianMobileNumberNormalizer.TryNormalize(
                command.PhoneNumber,
                out var phoneNumber))
        {
            return RequestOtpResult.Failure(
                new AuthError(
                    "InvalidPhoneNumber",
                    "شماره موبایل واردشده معتبر نیست."));
        }

        var now = DateTime.UtcNow;

        var ipAddress =
            string.IsNullOrWhiteSpace(command.IpAddress)
                ? null
                : command.IpAddress.Trim();

        var lastChallengeCreatedAt =
            await _dbContext.OtpChallenges
                .AsNoTracking()
                .Where(challenge =>
                    challenge.PhoneNumber == phoneNumber &&
                    challenge.Purpose == OtpPurpose.SignIn)
                .OrderByDescending(challenge =>
                    challenge.CreatedAt)
                .Select(challenge =>
                    (DateTime?)challenge.CreatedAt)
                .FirstOrDefaultAsync(
                    cancellationToken);

        if (lastChallengeCreatedAt is not null)
        {
            var nextAllowedSendAt =
                lastChallengeCreatedAt.Value.AddSeconds(
                    _otpOptions.ResendCooldownSeconds);

            if (nextAllowedSendAt > now)
            {
                var retryAfterSeconds =
                    (int)Math.Ceiling(
                        (nextAllowedSendAt - now)
                        .TotalSeconds);

                return RequestOtpResult.Failure(
                    new AuthError(
                        "OtpCooldown",
                        "برای ارسال مجدد کد کمی صبر کنید."),
                    retryAfterSeconds);
            }
        }

        var oneHourAgo =
            now.AddHours(-1);

        var oneDayAgo =
            now.AddDays(-1);

        var phoneHourlySendCount =
            await _dbContext.OtpChallenges
                .AsNoTracking()
                .CountAsync(
                    challenge =>
                        challenge.PhoneNumber == phoneNumber &&
                        challenge.Purpose == OtpPurpose.SignIn &&
                        challenge.CreatedAt >= oneHourAgo,
                    cancellationToken);

        if (phoneHourlySendCount >=
            _otpOptions.MaxSendsPerPhonePerHour)
        {
            return RequestOtpResult.Failure(
                new AuthError(
                    "PhoneHourlyLimitExceeded",
                    "تعداد درخواست‌های این شماره بیش از حد مجاز است."),
                retryAfterSeconds: 3600);
        }

        var phoneDailySendCount =
            await _dbContext.OtpChallenges
                .AsNoTracking()
                .CountAsync(
                    challenge =>
                        challenge.PhoneNumber == phoneNumber &&
                        challenge.Purpose == OtpPurpose.SignIn &&
                        challenge.CreatedAt >= oneDayAgo,
                    cancellationToken);

        if (phoneDailySendCount >=
            _otpOptions.MaxSendsPerPhonePerDay)
        {
            return RequestOtpResult.Failure(
                new AuthError(
                    "PhoneDailyLimitExceeded",
                    "تعداد درخواست‌های امروز این شماره بیش از حد مجاز است."),
                retryAfterSeconds: 86400);
        }

        if (ipAddress is not null)
        {
            var ipHourlySendCount =
                await _dbContext.OtpChallenges
                    .AsNoTracking()
                    .CountAsync(
                        challenge =>
                            challenge.CreatedByIp == ipAddress &&
                            challenge.Purpose == OtpPurpose.SignIn &&
                            challenge.CreatedAt >= oneHourAgo,
                        cancellationToken);

            if (ipHourlySendCount >=
                _otpOptions.MaxSendsPerIpPerHour)
            {
                return RequestOtpResult.Failure(
                    new AuthError(
                        "IpHourlyLimitExceeded",
                        "تعداد درخواست‌ها بیش از حد مجاز است."),
                    retryAfterSeconds: 3600);
            }

            var ipDailySendCount =
                await _dbContext.OtpChallenges
                    .AsNoTracking()
                    .CountAsync(
                        challenge =>
                            challenge.CreatedByIp == ipAddress &&
                            challenge.Purpose == OtpPurpose.SignIn &&
                            challenge.CreatedAt >= oneDayAgo,
                        cancellationToken);

            if (ipDailySendCount >=
                _otpOptions.MaxSendsPerIpPerDay)
            {
                return RequestOtpResult.Failure(
                    new AuthError(
                        "IpDailyLimitExceeded",
                        "تعداد درخواست‌های امروز بیش از حد مجاز است."),
                    retryAfterSeconds: 86400);
            }
        }

        var previousActiveChallenges =
            await _dbContext.OtpChallenges
                .Where(challenge =>
                    challenge.PhoneNumber == phoneNumber &&
                    challenge.Purpose == OtpPurpose.SignIn &&
                    challenge.UsedAt == null &&
                    challenge.InvalidatedAt == null &&
                    challenge.ExpiresAt > now)
                .ToListAsync(
                    cancellationToken);

        foreach (var previousChallenge
                 in previousActiveChallenges)
        {
            previousChallenge.Invalidate();
        }

        var challengeId =
            Guid.NewGuid();

        var code =
            _otpCodeService.GenerateCode();

        var codeHash =
            _otpCodeService.ComputeHash(
                challengeId,
                code);

        var expiresAt =
            now.AddMinutes(
                _otpOptions.ExpirationMinutes);

        var challenge =
            new OtpChallenge(
                challengeId,
                phoneNumber,
                codeHash,
                OtpPurpose.SignIn,
                expiresAt,
                _otpOptions.MaxVerificationAttempts,
                ipAddress);

        _dbContext.OtpChallenges.Add(
            challenge);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        try
        {
            await _otpMessageSender.SendSignInCodeAsync(
                phoneNumber,
                code,
                cancellationToken);
        }
        catch
        {
            challenge.Invalidate();

            await _dbContext.SaveChangesAsync(
                CancellationToken.None);

            throw;
        }

        return RequestOtpResult.Success(
            challengeId,
            expiresInSeconds:
                _otpOptions.ExpirationMinutes * 60,
            retryAfterSeconds:
                _otpOptions.ResendCooldownSeconds);
    }


    public async Task<RefreshSessionResult> RefreshSessionAsync(
        RefreshSessionCommand command,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(
                command.RefreshToken))
        {
            return RefreshSessionResult.Failure(
                new AuthError(
                    "InvalidRefreshToken",
                    "نشست کاربری معتبر نیست."));
        }

        var ipAddress =
            string.IsNullOrWhiteSpace(command.IpAddress)
                ? null
                : command.IpAddress.Trim();

        var refreshTokenHash =
            _tokenService.HashRefreshToken(
                command.RefreshToken);

        var currentRefreshToken =
            await _dbContext.RefreshTokens
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    token =>
                        token.TokenHash ==
                        refreshTokenHash,
                    cancellationToken);

        if (currentRefreshToken is null)
        {
            return RefreshSessionResult.Failure(
                new AuthError(
                    "InvalidRefreshToken",
                    "نشست کاربری منقضی یا نامعتبر است."));
        }

        if (currentRefreshToken.RevokedAt is not null &&
            !string.IsNullOrWhiteSpace(
                currentRefreshToken.ReplacedByTokenHash))
        {
            var reuseDetectedAt =
                DateTime.UtcNow;

            await _dbContext.RefreshTokens
                .Where(token =>
                    token.UserId ==
                        currentRefreshToken.UserId &&
                    token.RevokedAt == null &&
                    token.ExpiresAt > reuseDetectedAt)
                .ExecuteUpdateAsync(
                    setters =>
                        setters
                            .SetProperty(
                                token =>
                                    token.RevokedAt,
                                reuseDetectedAt)
                            .SetProperty(
                                token =>
                                    token.RevokedByIp,
                                ipAddress),
                    cancellationToken);

            return RefreshSessionResult.Failure(
                new AuthError(
                    "RefreshTokenReuseDetected",
                    "به دلیل تشخیص رفتار امنیتی، تمام نشست‌های حساب بسته شدند."));
        }

        if (!currentRefreshToken.IsActive)
        {
            return RefreshSessionResult.Failure(
                new AuthError(
                    "InvalidRefreshToken",
                    "نشست کاربری منقضی یا نامعتبر است."));
        }

        var user =
            await _userManager.FindByIdAsync(
                currentRefreshToken.UserId
                    .ToString());

        if (user is null ||
            string.IsNullOrWhiteSpace(
                user.PhoneNumber))
        {
            return RefreshSessionResult.Failure(
                new AuthError(
                    "UserNotFound",
                    "حساب کاربری یافت نشد."));
        }

        var roles =
            (await _userManager.GetRolesAsync(user))
                .ToArray();

        var generatedTokens =
            _tokenService.GenerateTokens(
                user.Id,
                user.PhoneNumber,
                user.Email,
                user.FullName,
                roles);


        var now =
            DateTime.UtcNow;

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken);

        var revokedTokenCount =
            await _dbContext.RefreshTokens
                .Where(token =>
                    token.Id ==
                        currentRefreshToken.Id &&
                    token.RevokedAt == null &&
                    token.ExpiresAt > now)
                .ExecuteUpdateAsync(
                    setters =>
                        setters
                            .SetProperty(
                                token =>
                                    token.RevokedAt,
                                now)
                            .SetProperty(
                                token =>
                                    token.RevokedByIp,
                                ipAddress)
                            .SetProperty(
                                token =>
                                    token.ReplacedByTokenHash,
                                generatedTokens
                                    .RefreshTokenHash),
                    cancellationToken);

        if (revokedTokenCount != 1)
        {
            return RefreshSessionResult.Failure(
                new AuthError(
                    "RefreshTokenAlreadyUsed",
                    "این نشست قبلاً تمدید شده یا دیگر معتبر نیست."));
        }

        await EnforceSessionLimitAsync(
            user.Id,
            ipAddress,
            cancellationToken);

        var newRefreshToken =
            new RefreshToken(
                generatedTokens.RefreshTokenHash,
                generatedTokens.RefreshTokenExpiresAt,
                ipAddress);

        user.AddRefreshToken(
            newRefreshToken);

        _dbContext.Entry(user).State =
            EntityState.Unchanged;

        _dbContext.RefreshTokens.Add(
            newRefreshToken);

        await _dbContext.SaveChangesAsync(
            cancellationToken);

        await transaction.CommitAsync(
            cancellationToken);

        var session =
            new AuthSession(
                UserId: user.Id,
                FullName: user.FullName,
                PhoneNumber: user.PhoneNumber,
                Email: user.Email,
                Roles: roles,
                AccessToken:
                    generatedTokens.AccessToken,
                AccessTokenExpiresAt:
                    generatedTokens.AccessTokenExpiresAt,
                RefreshToken:
                    generatedTokens.RefreshToken,
                RefreshTokenExpiresAt:
                    generatedTokens.RefreshTokenExpiresAt);

        return RefreshSessionResult.Success(
            session);
    }


    public async Task LogoutAsync(
    LogoutCommand command,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (string.IsNullOrWhiteSpace(
                command.RefreshToken))
        {
            return;
        }

        var refreshTokenHash =
            _tokenService.HashRefreshToken(
                command.RefreshToken);

        var ipAddress =
            string.IsNullOrWhiteSpace(command.IpAddress)
                ? null
                : command.IpAddress.Trim();

        var now =
            DateTime.UtcNow;

        await _dbContext.RefreshTokens
            .Where(token =>
                token.TokenHash == refreshTokenHash &&
                token.RevokedAt == null &&
                token.ExpiresAt > now)
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(
                            token =>
                                token.RevokedAt,
                            now)
                        .SetProperty(
                            token =>
                                token.RevokedByIp,
                            ipAddress),
                cancellationToken);
    }


    public async Task LogoutAllSessionsAsync(
    LogoutAllSessionsCommand command,
    CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (command.UserId == Guid.Empty)
        {
            return;
        }

        var ipAddress =
            string.IsNullOrWhiteSpace(command.IpAddress)
                ? null
                : command.IpAddress.Trim();

        var now =
            DateTime.UtcNow;

        await _dbContext.RefreshTokens
            .Where(token =>
                token.UserId == command.UserId &&
                token.RevokedAt == null &&
                token.ExpiresAt > now)
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(
                            token =>
                                token.RevokedAt,
                            now)
                        .SetProperty(
                            token =>
                                token.RevokedByIp,
                            ipAddress),
                cancellationToken);
    }
    public async Task<VerifyOtpResult> VerifyOtpAsync(
        VerifyOtpCommand command,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (command.ChallengeId == Guid.Empty ||
            string.IsNullOrWhiteSpace(command.Code))
        {
            return VerifyOtpResult.Failure(
                new AuthError(
                    "InvalidOtp",
                    "کد واردشده معتبر نیست."));
        }

        var now = DateTime.UtcNow;

        var challenge =
            await _dbContext.OtpChallenges
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == command.ChallengeId &&
                        item.Purpose == OtpPurpose.SignIn,
                    cancellationToken);

        if (challenge is null ||
            challenge.UsedAt is not null ||
            challenge.InvalidatedAt is not null ||
            challenge.ExpiresAt <= now ||
            challenge.FailedAttemptCount >=
                challenge.MaxAttempts)
        {
            return VerifyOtpResult.Failure(
                new AuthError(
                    "InvalidOrExpiredOtp",
                    "کد واردشده نامعتبر یا منقضی شده است."));
        }

        var codeIsValid =
            _otpCodeService.VerifyCode(
                challenge.Id,
                command.Code,
                challenge.CodeHash);

        if (!codeIsValid)
        {
            await _dbContext.OtpChallenges
                .Where(item =>
                    item.Id == challenge.Id &&
                    item.UsedAt == null &&
                    item.InvalidatedAt == null &&
                    item.ExpiresAt > now &&
                    item.FailedAttemptCount <
                        item.MaxAttempts)
                .ExecuteUpdateAsync(
                    setters =>
                        setters
                            .SetProperty(
                                item =>
                                    item.FailedAttemptCount,
                                item =>
                                    item.FailedAttemptCount + 1)
                            .SetProperty(
                                item =>
                                    item.InvalidatedAt,
                                item =>
                                    item.FailedAttemptCount + 1 >=
                                    item.MaxAttempts
                                        ? now
                                        : item.InvalidatedAt),
                    cancellationToken);

            return VerifyOtpResult.Failure(
                new AuthError(
                    "InvalidOtp",
                    "کد واردشده صحیح نیست."));
        }

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken);

        var claimedChallengeCount =
            await _dbContext.OtpChallenges
                .Where(item =>
                    item.Id == challenge.Id &&
                    item.UsedAt == null &&
                    item.InvalidatedAt == null &&
                    item.ExpiresAt > now &&
                    item.FailedAttemptCount <
                        item.MaxAttempts)
                .ExecuteUpdateAsync(
                    setters =>
                        setters.SetProperty(
                            item => item.UsedAt,
                            now),
                    cancellationToken);

        if (claimedChallengeCount != 1)
        {
            return VerifyOtpResult.Failure(
                new AuthError(
                    "OtpAlreadyUsed",
                    "این کد قبلاً استفاده شده یا دیگر معتبر نیست."));
        }

        var user =
            await _userManager.FindByNameAsync(
                challenge.PhoneNumber);

        if (user is null)
        {
            user =
                new ApplicationUser(
                    challenge.PhoneNumber)
                {
                    PhoneNumberConfirmed = true
                };

            var createResult =
                await _userManager.CreateAsync(user);

            if (!createResult.Succeeded)
            {
                var error =
                    MapErrors(createResult.Errors)
                        .FirstOrDefault()
                    ?? new AuthError(
                        "UserCreationFailed",
                        "ایجاد حساب کاربری انجام نشد.");

                return VerifyOtpResult.Failure(error);
            }

            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    ApplicationRoles.Customer);

            if (!roleResult.Succeeded)
            {
                var error =
                    MapErrors(roleResult.Errors)
                        .FirstOrDefault()
                    ?? new AuthError(
                        "RoleAssignmentFailed",
                        "تخصیص نقش کاربر انجام نشد.");

                return VerifyOtpResult.Failure(error);
            }
        }
        else if (!user.PhoneNumberConfirmed)
        {
            var confirmationResult =
                await _userManager.SetPhoneNumberAsync(
                    user,
                    challenge.PhoneNumber);

            if (!confirmationResult.Succeeded)
            {
                var error =
                    MapErrors(confirmationResult.Errors)
                        .FirstOrDefault()
                    ?? new AuthError(
                        "PhoneConfirmationFailed",
                        "تأیید شماره موبایل انجام نشد.");

                return VerifyOtpResult.Failure(error);
            }

            user.PhoneNumberConfirmed = true;

            var updateResult =
                await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                var error =
                    MapErrors(updateResult.Errors)
                        .FirstOrDefault()
                    ?? new AuthError(
                        "PhoneConfirmationFailed",
                        "تأیید شماره موبایل انجام نشد.");

                return VerifyOtpResult.Failure(error);
            }
        }

        var isAdmin =
            await _userManager.IsInRoleAsync(
                user,
                ApplicationRoles.Admin);

        if (isAdmin)
        {
            await transaction.CommitAsync(
                cancellationToken);

            return VerifyOtpResult.Failure(
                new AuthError(
                    "OtpSignInNotAllowed",
                    "ورود این حساب از این روش امکان‌پذیر نیست."));
        }

        var roles =
            (await _userManager.GetRolesAsync(user))
                .ToArray();

        var generatedTokens =
            _tokenService.GenerateTokens(
                user.Id,
                user.PhoneNumber!,
                user.Email,
                user.FullName,
                roles);

        var ipAddress =
            string.IsNullOrWhiteSpace(command.IpAddress)
                ? null
                : command.IpAddress.Trim();

        await EnforceSessionLimitAsync(
            user.Id,
            ipAddress,
            cancellationToken);

        var refreshToken =
            new RefreshToken(
                generatedTokens.RefreshTokenHash,
                generatedTokens.RefreshTokenExpiresAt,
                ipAddress);

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
                PhoneNumber: user.PhoneNumber!,
                Email: user.Email,
                Roles: roles,
                AccessToken:
                    generatedTokens.AccessToken,
                AccessTokenExpiresAt:
                    generatedTokens.AccessTokenExpiresAt,
                RefreshToken:
                    generatedTokens.RefreshToken,
                RefreshTokenExpiresAt:
                    generatedTokens.RefreshTokenExpiresAt);

        return VerifyOtpResult.Success(session);
    }

   
    private async Task EnforceSessionLimitAsync(
    Guid userId,
    string? ipAddress,
    CancellationToken cancellationToken)
    {
        var now =
            DateTime.UtcNow;

        var normalizedIpAddress =
            string.IsNullOrWhiteSpace(ipAddress)
                ? null
                : ipAddress.Trim();

        var activeSessionCount =
            await _dbContext.RefreshTokens
                .AsNoTracking()
                .CountAsync(
                    token =>
                        token.UserId == userId &&
                        token.RevokedAt == null &&
                        token.ExpiresAt > now,
                    cancellationToken);

        var sessionsToRevoke =
            activeSessionCount -
            _jwtOptions.MaxActiveSessionsPerUser +
            1;

        if (sessionsToRevoke <= 0)
        {
            return;
        }

        var oldestSessionIds =
            await _dbContext.RefreshTokens
                .AsNoTracking()
                .Where(token =>
                    token.UserId == userId &&
                    token.RevokedAt == null &&
                    token.ExpiresAt > now)
                .OrderBy(token =>
                    token.CreatedAt)
                .Take(sessionsToRevoke)
                .Select(token =>
                    token.Id)
                .ToArrayAsync(
                    cancellationToken);

        if (oldestSessionIds.Length == 0)
        {
            return;
        }

        await _dbContext.RefreshTokens
            .Where(token =>
                oldestSessionIds.Contains(
                    token.Id) &&
                token.RevokedAt == null)
            .ExecuteUpdateAsync(
                setters =>
                    setters
                        .SetProperty(
                            token =>
                                token.RevokedAt,
                            now)
                        .SetProperty(
                            token =>
                                token.RevokedByIp,
                            normalizedIpAddress),
                cancellationToken);
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