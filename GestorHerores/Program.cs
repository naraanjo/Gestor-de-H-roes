using DandDSoft.Infrastructure.Data;
using GestorHeroes.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. REGISTRO DE LA BASE DE DATOS (OBLIGATORIO PARA MIGRACIONES)
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// 2. REGISTRO DE SERVICIOS
builder.Services.AddScoped<IPersonajeService, PersonajeService>();
builder.Services.AddControllers();

// 3. CONFIGURACIÓN DE SWAGGER (ESTO QUITA EL ERROR ROJO DE TU IMAGEN)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "D&DSoft API", Version = "v1" });
    // Fix para el campo JSONB
    c.MapType<System.Text.Json.JsonDocument>(() => new OpenApiSchema { Type = "object" });
});

var app = builder.Build();

// 4. MIGRACIÓN AUTOMÁTICA AL ARRANCAR
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    context.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();