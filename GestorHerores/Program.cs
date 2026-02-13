using DandDSoft.Infrastructure.Data;
using GestorHeroes.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization; // Necesario para JsonUnmappedMemberHandling

var builder = WebApplication.CreateBuilder(args);

// REGISTRO DE LA BASE DE DATOS
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// REGISTRO DE SERVICIOS Y CONTROLADORES
builder.Services.AddScoped<IPersonajeService, PersonajeService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Configuro la política de nombres a CamelCase para seguir estándares JSON
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;

        // Esta es la configuración clave: Indico que si llega una propiedad en el JSON 
        // que no está definida en el DTO, se debe lanzar un error automáticamente
        options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
    });

// CONFIGURACIÓN DE SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "D&DSoft API", Version = "v1" });
    // Mapeo los tipos JSON para que Swagger los reconozca correctamente
    c.MapType<JsonDocument>(() => new OpenApiSchema { Type = "object" });
    c.MapType<JsonElement>(() => new OpenApiSchema { Type = "object" });
});

var app = builder.Build();

// MIGRACIÓN AUTOMÁTICA AL ARRANCAR
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    context.Database.Migrate();
}

// CONFIGURACIÓN DEL PIPELINE HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "D&DSoft API v1");
    });
}

app.UseAuthorization();
app.MapControllers();
app.Run();