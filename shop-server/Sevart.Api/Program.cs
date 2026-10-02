using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Application.Abstractions.Storage;
using Sevart.Infrastructure.Persistence;
using Sevart.Infrastructure.Persistence.Repositories;
using Sevart.Infrastructure.Storage;
using Microsoft.AspNetCore.Identity;
using Sevart.Infrastructure.Identity;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Sevart.Application.Abstractions.Identity;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SevartDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.User.RequireUniqueEmail = true;

        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;

        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);

        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddRoles<IdentityRole<Guid>>()
    .AddEntityFrameworkStores<SevartDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<
    ITokenService,
    TokenService>();

builder.Services.AddScoped<
    IAuthService,
    AuthService>();

var jwtSection =
    builder.Configuration.GetSection(
        JwtOptions.SectionName);

builder.Services
    .AddOptions<JwtOptions>()
    .Bind(jwtSection)
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.Issuer),
        "JWT issuer is required.")
    .Validate(
        options =>
            !string.IsNullOrWhiteSpace(options.Audience),
        "JWT audience is required.")
    .Validate(
        options =>
            Encoding.UTF8.GetByteCount(
                options.SecretKey) >= 32,
        "JWT secret key must be at least 32 bytes.")
    .Validate(
        options =>
            options.AccessTokenExpirationMinutes > 0,
        "Access token expiration must be greater than zero.")
    .Validate(
        options =>
            options.RefreshTokenExpirationDays > 0,
        "Refresh token expiration must be greater than zero.")
    .ValidateOnStart();

var jwtOptions =
    jwtSection.Get<JwtOptions>()
    ?? throw new InvalidOperationException(
        "JWT configuration is missing.");

var signingKey =
    new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(
            jwtOptions.SecretKey));

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = false;
        options.MapInboundClaims = false;

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtOptions.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtOptions.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = signingKey,

                ValidateLifetime = true,

                NameClaimType = "sub",

                RoleClaimType = "role",

                ClockSkew = TimeSpan.Zero
            };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<
    ICategoryRepository,
    CategoryRepository>();

builder.Services.AddScoped<
    IProductRepository,
    ProductRepository>();

builder.Services.AddScoped<
    IProductOptionDefinitionRepository,
    ProductOptionDefinitionRepository>();

var uploadsRootPath =
    builder.Configuration["Storage:RootPath"]
    ?? Path.Combine(
        builder.Environment.ContentRootPath,
        "uploads");

var uploadsBaseUrl =
    builder.Configuration["Storage:BaseUrl"]
    ?? "/uploads";

Directory.CreateDirectory(uploadsRootPath);

builder.Services.AddSingleton<IFileStorage>(
    new LocalFileStorage(
        uploadsRootPath,
        uploadsBaseUrl));

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

var allowedOrigins =
    builder.Configuration
        .GetSection("Cors:AllowedOrigins")
        .Get<string[]>()
    ??
    [
        "http://localhost:4200",
        "http://localhost:4500",
        "https://sevart.ir",
        "https://www.sevart.ir",
        "https://noviraone.ir",
        "https://www.noviraone.ir",
        "https://noviraone.runflare.run"
    ];

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularClient",
        policy =>
        {
            policy
                .WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto;

    options.KnownIPNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider =
            new PhysicalFileProvider(uploadsRootPath),

        RequestPath = uploadsBaseUrl
    });

app.UseCors("AngularClient");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapGet(
    "/health",
    () => Results.Ok(
        new
        {
            status = "healthy"
        }));

await using (var scope = app.Services.CreateAsyncScope())
{
    var dbContext =
        scope.ServiceProvider
            .GetRequiredService<SevartDbContext>();

    await dbContext.Database.MigrateAsync();

    var roleManager =
        scope.ServiceProvider
            .GetRequiredService<
                RoleManager<IdentityRole<Guid>>>();

    await IdentitySeeder.SeedRolesAsync(
        roleManager);
}

app.Run();