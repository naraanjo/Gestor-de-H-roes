using DandDSoft.Infrastructure.Data;
using GestorHeroes.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE LA BASE DE DATOS
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// 2. REGISTRO DE SERVICIOS Y CONTROLADORES
builder.Services.AddScoped<IPersonajeService, PersonajeService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Esto asegura que el JSON se vea bien en la API
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    });

// 3. CONFIGURACIÓN DE SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "D&DSoft API", Version = "v1" });
    // Fix para el campo JSONB (JsonDocument o JsonElement)
    c.MapType<JsonDocument>(() => new OpenApiSchema { Type = "object" });
    c.MapType<JsonElement>(() => new OpenApiSchema { Type = "object" });
});

var app = builder.Build();

// 4. MIGRACIÓN AUTOMÁTICA AL ARRANCAR
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    context.Database.Migrate();
}

// 5. CONFIGURACIÓN DEL PIPELINE
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Esto ayuda a que Swagger cargue correctamente en la raíz
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "D&DSoft API v1");
    });
}

// app.UseHttpsRedirection(); // Opcional en desarrollo local

app.UseAuthorization();
app.MapControllers();
app.Run();