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

var uploadsRootPath = Path.Combine(
    builder.Environment.ContentRootPath,
    "uploads");

Directory.CreateDirectory(uploadsRootPath);

builder.Services.AddSingleton<IFileStorage>(
    new LocalFileStorage(
        uploadsRootPath,
        "/uploads"));

builder.Services.AddControllers();

builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy(
        "AngularClient",
        policy =>
        {
            policy
                .WithOrigins(
                    "http://localhost:4200",
                    "http://localhost:4500")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseStaticFiles(
    new StaticFileOptions
    {
        FileProvider =
            new PhysicalFileProvider(uploadsRootPath),

        RequestPath = "/uploads"
    });

app.UseCors("AngularClient");

app.UseAuthorization();

app.MapControllers();

app.Run();