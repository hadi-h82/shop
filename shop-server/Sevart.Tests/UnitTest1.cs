
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Moq;
using Sevart.Application.Abstractions.Identity;
using Sevart.Infrastructure.Identity;
using Sevart.Infrastructure.Persistence;
using Xunit;
using Microsoft.AspNetCore.Identity;


namespace Sevart.Tests;

public class UnitTest1
{
    [Fact]
    public async Task RequestOtpAsync_ShouldReject_WhenPhoneHourlyLimitIsReached()
    {
        // Arrange
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        const string phoneNumber = "09123456789";
        const string ipAddress = "192.0.2.10";

        var otpOptions = new OtpOptions
        {
            CodeLength = 6,
            ExpirationMinutes = 2,
            ResendCooldownSeconds = 90,
            MaxVerificationAttempts = 5,
            MaxSendsPerPhonePerHour = 2,
            MaxSendsPerPhonePerDay = 5,
            MaxSendsPerIpPerHour = 10,
            MaxSendsPerIpPerDay = 30
        };

        for (var i = 0; i < 2; i++)
        {
            var challenge = new OtpChallenge(
                Guid.NewGuid(),
                phoneNumber,
                "test-hash",
                OtpPurpose.SignIn,
                DateTime.UtcNow.AddMinutes(10),
                otpOptions.MaxVerificationAttempts,
                ipAddress);

            // درخواست‌ها قدیمی‌تر از cooldown هستند،
            // اما همچنان در بازه یک‌ساعته قرار دارند.
            typeof(OtpChallenge)
                .GetProperty(nameof(OtpChallenge.CreatedAt))!
                .SetValue(
                    challenge,
                    DateTime.UtcNow.AddMinutes(-5));

            dbContext.OtpChallenges.Add(challenge);
        }

        await dbContext.SaveChangesAsync();

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            null!,
            null!,
            Options.Create(otpOptions),
            Options.Create(new JwtOptions()));

        var command = new RequestOtpCommand(
            phoneNumber,
            ipAddress);

        // Act
        var result = await service.RequestOtpAsync(command);

        // Assert
        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "PhoneHourlyLimitExceeded");
    }



    [Fact]
    public async Task RequestOtpAsync_ShouldReject_WhenIpHourlyLimitIsReached()
    {
        // Arrange
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        const string phoneNumber = "09123456789";
        const string ipAddress = "192.0.2.10";

        var otpOptions = new OtpOptions
        {
            CodeLength = 6,
            ExpirationMinutes = 2,
            ResendCooldownSeconds = 90,
            MaxVerificationAttempts = 5,

            MaxSendsPerPhonePerHour = 100,
            MaxSendsPerPhonePerDay = 100,

            MaxSendsPerIpPerHour = 1,
            MaxSendsPerIpPerDay = 100
        };

        for (var i = 0; i < 2; i++)
        {
            var challenge = new OtpChallenge(
                Guid.NewGuid(),
                phoneNumber,
                "test-hash",
                OtpPurpose.SignIn,
                DateTime.UtcNow.AddMinutes(10),
                otpOptions.MaxVerificationAttempts,
                ipAddress);

            // درخواست‌ها قدیمی‌تر از cooldown هستند،
            // اما همچنان در بازه یک‌ساعته قرار دارند.
            typeof(OtpChallenge)
                .GetProperty(nameof(OtpChallenge.CreatedAt))!
                .SetValue(
                    challenge,
                    DateTime.UtcNow.AddMinutes(-5));

            dbContext.OtpChallenges.Add(challenge);
        }

        await dbContext.SaveChangesAsync();

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            null!,
            null!,
            Options.Create(otpOptions),
            Options.Create(new JwtOptions()));

        var command = new RequestOtpCommand(
            phoneNumber,
            ipAddress);

        // Act
        var result = await service.RequestOtpAsync(command);

        // Assert
        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "IpHourlyLimitExceeded");
    }




    [Fact]
    public async Task RequestOtpAsync_ShouldReject_WhenPhoneDailyLimitIsReached()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        const string phoneNumber = "09123456789";
        const string ipAddress = "192.0.2.10";

        var otpOptions = new OtpOptions
        {
            CodeLength = 6,
            ExpirationMinutes = 2,
            ResendCooldownSeconds = 90,
            MaxVerificationAttempts = 5,

            MaxSendsPerPhonePerHour = 100,
            MaxSendsPerPhonePerDay = 2,
            MaxSendsPerIpPerHour = 100,
            MaxSendsPerIpPerDay = 100
        };

        for (var i = 0; i < 2; i++)
        {
            var challenge = new OtpChallenge(
                Guid.NewGuid(),
                phoneNumber,
                "test-hash",
                OtpPurpose.SignIn,
                DateTime.UtcNow.AddMinutes(10),
                otpOptions.MaxVerificationAttempts,
                ipAddress);

            typeof(OtpChallenge)
                .GetProperty(nameof(OtpChallenge.CreatedAt))!
                .SetValue(
                    challenge,
                    DateTime.UtcNow.AddHours(-2));

            dbContext.OtpChallenges.Add(challenge);
        }

        await dbContext.SaveChangesAsync();

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            null!,
            null!,
            Options.Create(otpOptions),
            Options.Create(new JwtOptions()));

        var command = new RequestOtpCommand(
            phoneNumber,
            ipAddress);

        var result = await service.RequestOtpAsync(command);

        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "PhoneDailyLimitExceeded");
    }



    [Fact]
    public async Task RequestOtpAsync_ShouldReject_WhenIpDailyLimitIsReached()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        const string phoneNumber = "09123456789";
        const string ipAddress = "192.0.2.10";

        var otpOptions = new OtpOptions
        {
            CodeLength = 6,
            ExpirationMinutes = 2,
            ResendCooldownSeconds = 90,
            MaxVerificationAttempts = 5,

            MaxSendsPerPhonePerHour = 100,
            MaxSendsPerPhonePerDay = 100,
            MaxSendsPerIpPerHour = 100,
            MaxSendsPerIpPerDay = 2
        };

        for (var i = 0; i < 2; i++)
        {
            var challenge = new OtpChallenge(
                Guid.NewGuid(),
                phoneNumber,
                "test-hash",
                OtpPurpose.SignIn,
                DateTime.UtcNow.AddMinutes(10),
                otpOptions.MaxVerificationAttempts,
                ipAddress);

            typeof(OtpChallenge)
                .GetProperty(nameof(OtpChallenge.CreatedAt))!
                .SetValue(
                    challenge,
                    DateTime.UtcNow.AddHours(-2));

            dbContext.OtpChallenges.Add(challenge);
        }

        await dbContext.SaveChangesAsync();

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            null!,
            null!,
            Options.Create(otpOptions),
            Options.Create(new JwtOptions()));

        var command = new RequestOtpCommand(
            phoneNumber,
            ipAddress);

        var result = await service.RequestOtpAsync(command);

        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "IpDailyLimitExceeded");
    }



    [Fact]
    public async Task RequestOtpAsync_ShouldReject_WhenCooldownIsActive()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        const string phoneNumber = "09123456789";
        const string ipAddress = "192.0.2.10";

        var otpOptions = new OtpOptions
        {
            CodeLength = 6,
            ExpirationMinutes = 2,
            ResendCooldownSeconds = 90,
            MaxVerificationAttempts = 5,
            MaxSendsPerPhonePerHour = 100,
            MaxSendsPerPhonePerDay = 100,
            MaxSendsPerIpPerHour = 100,
            MaxSendsPerIpPerDay = 100
        };

        var challenge = new OtpChallenge(
            Guid.NewGuid(),
            phoneNumber,
            "test-hash",
            OtpPurpose.SignIn,
            DateTime.UtcNow.AddMinutes(2),
            otpOptions.MaxVerificationAttempts,
            ipAddress);

        typeof(OtpChallenge)
            .GetProperty(nameof(OtpChallenge.CreatedAt))!
            .SetValue(
                challenge,
                DateTime.UtcNow.AddSeconds(-30));

        dbContext.OtpChallenges.Add(challenge);

        await dbContext.SaveChangesAsync();

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            null!,
            null!,
            Options.Create(otpOptions),
            Options.Create(new JwtOptions()));

        var command = new RequestOtpCommand(
            phoneNumber,
            ipAddress);

        var result = await service.RequestOtpAsync(command);

        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "OtpCooldown");
    }


    [Theory]
    [InlineData("123")]
    [InlineData("abcdefghijk")]
    [InlineData("")]
    public async Task RequestOtpAsync_ShouldReject_WhenPhoneNumberIsInvalid(
        string phoneNumber)
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        var otpOptions = new OtpOptions
        {
            CodeLength = 6,
            ExpirationMinutes = 2,
            ResendCooldownSeconds = 90,
            MaxVerificationAttempts = 5,
            MaxSendsPerPhonePerHour = 5,
            MaxSendsPerPhonePerDay = 10,
            MaxSendsPerIpPerHour = 10,
            MaxSendsPerIpPerDay = 30
        };

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            null!,
            null!,
            Options.Create(otpOptions),
            Options.Create(new JwtOptions()));

        var command = new RequestOtpCommand(
            phoneNumber,
            "192.0.2.10");

        var result = await service.RequestOtpAsync(command);

        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "InvalidPhoneNumber");

        Assert.Equal(
            0,
            await dbContext.OtpChallenges.CountAsync());
    }



    [Fact]
    public async Task VerifyOtpAsync_ShouldInvalidateChallenge_WhenMaxFailedAttemptsReached()
    {
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        var otpOptions = new OtpOptions
        {
            CodeLength = 6,
            ExpirationMinutes = 2,
            ResendCooldownSeconds = 90,
            MaxVerificationAttempts = 5,
            HashKey = Convert.ToBase64String(
                new byte[32])
        };

        var otpCodeService =
            new OtpCodeService(
                Options.Create(otpOptions));

        const string phoneNumber = "09123456789";
        const string validCode = "123456";
        const string wrongCode = "000000";
        const string ipAddress = "192.0.2.10";

        var challengeId = Guid.NewGuid();

        var codeHash =
            otpCodeService.ComputeHash(
                challengeId,
                validCode);

        var challenge = new OtpChallenge(
            challengeId,
            phoneNumber,
            codeHash,
            OtpPurpose.SignIn,
            DateTime.UtcNow.AddMinutes(2),
            otpOptions.MaxVerificationAttempts,
            ipAddress);

        dbContext.OtpChallenges.Add(challenge);

        await dbContext.SaveChangesAsync();

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            otpCodeService,
            null!,
            Options.Create(otpOptions),
            Options.Create(new JwtOptions()));

        // پنج بار وارد کردن کد اشتباه
        for (var i = 0;
             i < otpOptions.MaxVerificationAttempts;
             i++)
        {
            var result =
                await service.VerifyOtpAsync(
                    new VerifyOtpCommand(
                        challengeId,
                        wrongCode,
                        ipAddress));

            Assert.False(result.Succeeded);

            Assert.Contains(
                result.Errors,
                error => error.Code == "InvalidOtp");
        }

        // بررسی وضعیت واقعی رکورد در دیتابیس
        var savedChallenge =
            await dbContext.OtpChallenges
                .AsNoTracking()
                .SingleAsync(
                    item => item.Id == challengeId);

        Assert.Equal(
            otpOptions.MaxVerificationAttempts,
            savedChallenge.FailedAttemptCount);

        Assert.NotNull(
            savedChallenge.InvalidatedAt);

        // حتی کد صحیح هم نباید پذیرفته شود.
        var correctCodeResult =
            await service.VerifyOtpAsync(
                new VerifyOtpCommand(
                    challengeId,
                    validCode,
                    ipAddress));

        Assert.False(correctCodeResult.Succeeded);

        Assert.Contains(
            correctCodeResult.Errors,
            error =>
                error.Code == "InvalidOrExpiredOtp");
    }



    [Fact]
    public async Task VerifyOtpAsync_ShouldReject_WhenChallengeIsExpired()
    {
        // Arrange
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        const string phoneNumber = "09123456789";
        const string ipAddress = "192.0.2.10";

        var challengeId = Guid.NewGuid();

        var challenge = new OtpChallenge(
            challengeId,
            phoneNumber,
            "test-hash",
            OtpPurpose.SignIn,
            DateTime.UtcNow.AddMinutes(2),
            5,
            ipAddress);

        typeof(OtpChallenge)
            .GetProperty(nameof(OtpChallenge.ExpiresAt))!
            .SetValue(
                challenge,
                DateTime.UtcNow.AddMinutes(-1));

        dbContext.OtpChallenges.Add(challenge);

        await dbContext.SaveChangesAsync();

        var otpOptions = new OtpOptions
        {
            CodeLength = 6,
            ExpirationMinutes = 2,
            ResendCooldownSeconds = 90,
            MaxVerificationAttempts = 5
        };

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            null!,
            null!,
            Options.Create(otpOptions),
            Options.Create(new JwtOptions()));

        var command = new VerifyOtpCommand(
            challengeId,
            "123456",
            ipAddress);

        // Act
        var result = await service.VerifyOtpAsync(command);

        // Assert
        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "InvalidOrExpiredOtp");
    }


    [Fact]
    public async Task VerifyOtpAsync_ShouldReject_WhenChallengeWasAlreadyUsed()
    {
        // Arrange
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        const string phoneNumber = "09123456789";
        const string ipAddress = "192.0.2.10";

        var challengeId = Guid.NewGuid();

        var challenge = new OtpChallenge(
            challengeId,
            phoneNumber,
            "test-hash",
            OtpPurpose.SignIn,
            DateTime.UtcNow.AddMinutes(2),
            5,
            ipAddress);

        typeof(OtpChallenge)
            .GetProperty(nameof(OtpChallenge.UsedAt))!
            .SetValue(
                challenge,
                DateTime.UtcNow.AddSeconds(-30));

        dbContext.OtpChallenges.Add(challenge);

        await dbContext.SaveChangesAsync();

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            null!,
            null!,
            Options.Create(new OtpOptions
            {
                CodeLength = 6,
                ExpirationMinutes = 2,
                ResendCooldownSeconds = 90,
                MaxVerificationAttempts = 5
            }),
            Options.Create(new JwtOptions()));

        var command = new VerifyOtpCommand(
            challengeId,
            "123456",
            ipAddress);

        // Act
        var result = await service.VerifyOtpAsync(command);

        // Assert
        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "InvalidOrExpiredOtp");
    }


    [Fact]
    public async Task VerifyOtpAsync_ShouldReject_WhenChallengeIsInvalidated()
    {
        // Arrange
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        const string phoneNumber = "09123456789";
        const string ipAddress = "192.0.2.10";

        var challengeId = Guid.NewGuid();

        var challenge = new OtpChallenge(
            challengeId,
            phoneNumber,
            "test-hash",
            OtpPurpose.SignIn,
            DateTime.UtcNow.AddMinutes(2),
            5,
            ipAddress);

        typeof(OtpChallenge)
            .GetProperty(nameof(OtpChallenge.InvalidatedAt))!
            .SetValue(
                challenge,
                DateTime.UtcNow.AddSeconds(-30));

        dbContext.OtpChallenges.Add(challenge);

        await dbContext.SaveChangesAsync();

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            null!,
            null!,
            Options.Create(new OtpOptions
            {
                CodeLength = 6,
                ExpirationMinutes = 2,
                ResendCooldownSeconds = 90,
                MaxVerificationAttempts = 5
            }),
            Options.Create(new JwtOptions()));

        var command = new VerifyOtpCommand(
            challengeId,
            "123456",
            ipAddress);

        // Act
        var result = await service.VerifyOtpAsync(command);

        // Assert
        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "InvalidOrExpiredOtp");
    }


    [Fact]
    public async Task VerifyOtpAsync_ShouldIncrementFailedAttemptCount_WhenCodeIsWrong()
    {
        // Arrange
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        const string phoneNumber = "09123456789";
        const string ipAddress = "192.0.2.10";
        const string wrongCode = "000000";

        var challengeId = Guid.NewGuid();

        var challenge = new OtpChallenge(
            challengeId,
            phoneNumber,
            "test-hash",
            OtpPurpose.SignIn,
            DateTime.UtcNow.AddMinutes(2),
            5,
            ipAddress);

        dbContext.OtpChallenges.Add(challenge);
        await dbContext.SaveChangesAsync();

        var otpCodeServiceMock = new Mock<IOtpCodeService>();

        otpCodeServiceMock
            .Setup(service => service.VerifyCode(
                challengeId,
                wrongCode,
                "test-hash"))
            .Returns(false);

        var service = new AuthService(
            null!,
            dbContext,
            null!,
            otpCodeServiceMock.Object,
            null!,
            Options.Create(new OtpOptions
            {
                CodeLength = 6,
                ExpirationMinutes = 2,
                ResendCooldownSeconds = 90,
                MaxVerificationAttempts = 5
            }),
            Options.Create(new JwtOptions()));

        var command = new VerifyOtpCommand(
            challengeId,
            wrongCode,
            ipAddress);

        // Act
        var result = await service.VerifyOtpAsync(command);

        // Assert
        Assert.False(result.Succeeded);

        Assert.Contains(
            result.Errors,
            error => error.Code == "InvalidOtp");

        var savedChallenge =
            await dbContext.OtpChallenges
                .AsNoTracking()
                .SingleAsync(item => item.Id == challengeId);

        Assert.Equal(1, savedChallenge.FailedAttemptCount);
        Assert.Null(savedChallenge.InvalidatedAt);
    }


    [Fact]
    public async Task VerifyOtpAsync_ShouldSucceed_WhenCodeIsCorrect()
    {
        // Arrange
        await using var connection =
            new SqliteConnection("Data Source=:memory:");

        await connection.OpenAsync();

        var dbOptions =
            new DbContextOptionsBuilder<SevartDbContext>()
                .UseSqlite(connection)
                .Options;

        await using var dbContext =
            new SevartDbContext(dbOptions);

        await dbContext.Database.EnsureCreatedAsync();

        const string phoneNumber = "09123456789";
        const string ipAddress = "192.0.2.10";
        const string code = "123456";

        var challengeId = Guid.NewGuid();

        var challenge = new OtpChallenge(
            challengeId,
            phoneNumber,
            "test-hash",
            OtpPurpose.SignIn,
            DateTime.UtcNow.AddMinutes(2),
            5,
            ipAddress);

        var user = new ApplicationUser(phoneNumber)
        {
            PhoneNumberConfirmed = true
        };

        dbContext.Users.Add(user);
        dbContext.OtpChallenges.Add(challenge);

        await dbContext.SaveChangesAsync();

        var userStoreMock =
            new Mock<IUserStore<ApplicationUser>>();

        var userManagerMock =
            new Mock<UserManager<ApplicationUser>>(
                userStoreMock.Object,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!,
                null!);

        userManagerMock
            .Setup(manager =>
                manager.FindByNameAsync(
                    It.IsAny<string>()))
            .ReturnsAsync(user);

        userManagerMock
            .Setup(manager =>
                manager.IsInRoleAsync(
                    user,
                    ApplicationRoles.Admin))
            .ReturnsAsync(false);

        userManagerMock
            .Setup(manager =>
                manager.GetRolesAsync(user))
            .ReturnsAsync(
                new List<string>
                {
                ApplicationRoles.Customer
                });

        var otpCodeServiceMock =
            new Mock<IOtpCodeService>();

        otpCodeServiceMock
            .Setup(service =>
                service.VerifyCode(
                    challengeId,
                    code,
                    "test-hash"))
            .Returns(true);

        var tokenServiceMock =
            new Mock<ITokenService>();

        var accessTokenExpiresAt =
            DateTime.UtcNow.AddMinutes(15);

        var refreshTokenExpiresAt =
            DateTime.UtcNow.AddDays(7);

        tokenServiceMock
            .Setup(service =>
                service.GenerateTokens(
                    It.IsAny<Guid>(),
                    It.IsAny<string>(),
                    It.IsAny<string?>(),
                    It.IsAny<string?>(),
                    It.IsAny<IReadOnlyCollection<string>>()))
                    .Returns(
                new GeneratedAuthTokens(
                    "access-token",
                    accessTokenExpiresAt,
                    "refresh-token",
                    "refresh-token-hash",
                    refreshTokenExpiresAt));

        var service = new AuthService(
            userManagerMock.Object,
            dbContext,
            tokenServiceMock.Object,
            otpCodeServiceMock.Object,
            null!,
            Options.Create(new OtpOptions
            {
                CodeLength = 6,
                ExpirationMinutes = 2,
                ResendCooldownSeconds = 90,
                MaxVerificationAttempts = 5
            }),
            Options.Create(new JwtOptions
            {
                MaxActiveSessionsPerUser = 5
            }));

        var command = new VerifyOtpCommand(
            challengeId,
            code,
            ipAddress);

        // Act
        var result =
            await service.VerifyOtpAsync(command);

        // Assert
        Assert.True(result.Succeeded);

        Assert.NotNull(result.Session);

        Assert.Equal(user.Id, result.Session.UserId);
        Assert.Equal(user.PhoneNumber, result.Session.PhoneNumber);
        Assert.Equal("access-token", result.Session.AccessToken);
        Assert.Equal("refresh-token", result.Session.RefreshToken);

        var savedChallenge =
            await dbContext.OtpChallenges
                .AsNoTracking()
                .SingleAsync(item => item.Id == challengeId);

        Assert.NotNull(savedChallenge.UsedAt);

        var savedRefreshToken =
            await dbContext.RefreshTokens
                .AsNoTracking()
                .SingleAsync(item => item.UserId == user.Id);

        Assert.Equal(
            "refresh-token-hash",
            savedRefreshToken.TokenHash);
    }


}
