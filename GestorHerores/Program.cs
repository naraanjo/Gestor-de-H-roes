using DandDSoft.Infrastructure.Data;
using GestorHeroes.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// REGISTRO DE LA BASE DE DATOS
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// REGISTRO DE SERVICIOS
builder.Services.AddScoped<IPersonajeService, PersonajeService>();

// Restricciones de JSON para evitar atributos no mapeados
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        // Mantenemos la restricción estricta
        options.JsonSerializerOptions.UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow;
    })
    .ConfigureApiBehaviorOptions(options =>
    {
        // Personalizamos el error
        options.InvalidModelStateResponseFactory = context =>
        {
            // Obtenemos los errores de validación relacionados con miembros no mapeados
            var errores = context.ModelState
                .Where(e => e.Value.Errors.Count > 0)
                .Select(e => new
                {
                    Campo = e.Key.Replace("$.", ""), // Quitamos el símbolo raro '$' si aparece
                    Mensaje = "Este campo no pertenece al personaje o el formato es incorrecto." // Notifico al usuario
                })
                .ToList();

            // Mensaje al usuario
            var respuestaPersonalizada = new
            {
                Titulo = "Datos invalidos",
                Estado = 400,
                Ayuda = "Por favor, revisa que no estés enviando atributos que no existen en este tipo de héroe.",
                DetallesErrores = errores
            };

            // Devolvemos el error
            return new BadRequestObjectResult(respuestaPersonalizada);
        };
    });

// CONFIGURACIÓN DE SWAGGER
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "D&DSoft API", Version = "v1" });
    c.MapType<JsonDocument>(() => new OpenApiSchema { Type = "object" });
    c.MapType<JsonElement>(() => new OpenApiSchema { Type = "object" });
});

var app = builder.Build();

// MIGRACIÓN AUTOMÁTICA
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameDbContext>();
    context.Database.Migrate();
}

// PIPELINE HTTP
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