using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using VerticalLogistica.Application;
using VerticalLogistica.Infrastructure;
using VerticalLogistica.Infrastructure.Context;

var builder = WebApplication.CreateBuilder(args);

// 1. MVC e JSON
builder.Services
    .AddControllers()
    .AddNewtonsoftJson();

// 2. Domain/Application/Infrastructure
builder.Services.AddApplication();
builder.Services.AddInfrastructure();

// 3. DbContext SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 4. Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();

// Primeiro registre o SwaggerGen
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Vertical Logistica API",
        Version = "v1",
        Description = "API para processamento de pedidos do sistema legado",
        Contact = new OpenApiContact
        {
            Name = "Wellington Americano",
            Email = "americanosdigital@gmail.com"
        }
    });
});

// Em seguida adicione o suporte ao Newtonsoft no Swagger (no IServiceCollection)
builder.Services.AddSwaggerGenNewtonsoftSupport();

// 5. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultPolicy", policy =>
    {
        policy
            .WithOrigins("http://localhost:4200", "https://localhost:7183")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

// 6. Pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Vertical Logistica API v1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("DefaultPolicy");
app.UseAuthorization();
app.MapControllers();

app.Run();
