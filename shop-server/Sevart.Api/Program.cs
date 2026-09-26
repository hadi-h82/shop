using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Sevart.Application.Abstractions.Persistence;
using Sevart.Application.Abstractions.Storage;
using Sevart.Infrastructure.Persistence;
using Sevart.Infrastructure.Persistence.Repositories;
using Sevart.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<SevartDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

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
}

app.Run();