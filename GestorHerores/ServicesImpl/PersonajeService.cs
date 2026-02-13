using DandDSoft.Infrastructure.Data;
using GestorHeroes.Models;
using GestorHerores.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GestorHeroes.Services
{
    /*
     * Author: Álvaro Naranjo Rodriguez
     * Descripción: Implementación de la lógica de negocio para la gestión de personajes. 
     * Se encarga de la persistencia polimórfica (TPT), validaciones de integridad 
     * y procesamiento de datos dinámicos JSONB.
     */
    public class PersonajeService : IPersonajeService
    {
        private readonly GameDbContext _context;

        // Inyección del contexto de la base de datos para operaciones de persistencia
        public PersonajeService(GameDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Personaje>> GetAllAsync()
        {
            // Recupera la lista completa de personajes incluyendo especializaciones
            return await _context.Personajes.ToListAsync();
        }

        public async Task<Personaje?> GetByIdAsync(int id)
        {
            // Búsqueda de un personaje específico por su identificador único
            return await _context.Personajes.FindAsync(id);
        }

        // --- MÉTODOS DE CREACIÓN ---

        public async Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto)
        {
            // Validación de unicidad del nombre antes de la inserción
            await ValidarNombreUnicoAsync(dto.Nombre);

            var guerrero = new Guerrero
            {
                Nombre = dto.Nombre,
                Nivel = dto.Nivel,
                FechaCreacion = DateTime.UtcNow,
                Gremio = dto.Gremio,
                ArmaPrincipal = dto.ArmaPrincipal,
                Furia = dto.Furia,
                Rasgos = ConvertJson(dto.Rasgos) // Conversión de formato para JSONB
            };
            _context.Guerreros.Add(guerrero);
            await _context.SaveChangesAsync();
            return guerrero;
        }

        public async Task<Mago> CreateMagoAsync(MagoCreateDto dto)
        {
            // Comprobación de disponibilidad del nombre en el sistema
            await ValidarNombreUnicoAsync(dto.Nombre);

            var mago = new Mago
            {
                Nombre = dto.Nombre,
                Nivel = dto.Nivel,
                FechaCreacion = DateTime.UtcNow,
                Gremio = dto.Gremio,
                Mana = dto.Mana,
                ElementoPrincipal = dto.ElementoPrincipal,
                Rasgos = ConvertJson(dto.Rasgos)
            };
            _context.Magos.Add(mago);
            await _context.SaveChangesAsync();
            return mago;
        }

        public async Task<Arquero> CreateArqueroAsync(ArqueroCreateDto dto)
        {
            // Verificación técnica de nombre duplicado
            await ValidarNombreUnicoAsync(dto.Nombre);

            var arquero = new Arquero
            {
                Nombre = dto.Nombre,
                Nivel = dto.Nivel,
                FechaCreacion = DateTime.UtcNow,
                Gremio = dto.Gremio,
                Precision = dto.Precision,
                TieneMascota = dto.TieneMascota,
                Rasgos = ConvertJson(dto.Rasgos)
            };
            _context.Arqueros.Add(arquero);
            await _context.SaveChangesAsync();
            return arquero;
        }

        public async Task<Clerigo> CreateClerigoAsync(ClerigoCreateDto dto)
        {
            // Asegura que el nombre no colisione con registros existentes
            await ValidarNombreUnicoAsync(dto.Nombre);

            var clerigo = new Clerigo
            {
                Nombre = dto.Nombre,
                Nivel = dto.Nivel,
                FechaCreacion = DateTime.UtcNow,
                Gremio = dto.Gremio,
                Deidad = dto.Deidad,
                PuntosSanacion = dto.PuntosSanacion,
                Rasgos = ConvertJson(dto.Rasgos)
            };
            _context.Clerigos.Add(clerigo);
            await _context.SaveChangesAsync();
            return clerigo;
        }

        public async Task<bool> UpdateAsync(int id, PersonajeBaseDto dto)
        {
            var personaje = await _context.Personajes.FindAsync(id);
            if (personaje == null) return false;

            // Validación de nombre si se detecta un cambio respecto al original
            if (!string.Equals(personaje.Nombre, dto.Nombre, StringComparison.CurrentCultureIgnoreCase))
            {
                await ValidarNombreUnicoAsync(dto.Nombre);
            }

            personaje.Nombre = dto.Nombre;
            personaje.Nivel = dto.Nivel;
            personaje.Gremio = dto.Gremio;

            // Actualización del campo JSONB si el DTO contiene nuevos rasgos
            if (dto.Rasgos.HasValue)
            {
                personaje.Rasgos = ConvertJson(dto.Rasgos);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var personaje = await _context.Personajes.FindAsync(id);
            if (personaje == null) return false;

            _context.Personajes.Remove(personaje);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- CONSULTAS COMPLEJAS ---

        public async Task<IEnumerable<Personaje>> GetByRasgoAsync(string claveRasgo)
        {
            // Filtrado en memoria de personajes que contienen una clave específica en sus rasgos JSON
            var todos = await _context.Personajes.ToListAsync();
            return todos.Where(p => p.Rasgos != null &&
                               p.Rasgos.RootElement.EnumerateObject()
                               .Any(prop => prop.Name.Equals(claveRasgo, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<object> GetEstadisticasPorGremioAsync()
        {
            // Agrupación y cálculo de métricas de nivel por cada Gremio registrado
            return await _context.Personajes
                .GroupBy(p => p.Gremio)
                .Select(g => new
                {
                    Gremio = g.Key ?? "Sin Gremio",
                    Cantidad = g.Count(),
                    NivelPromedio = g.Average(p => p.Nivel)
                })
                .ToListAsync();
        }

        // Helper para convertir JsonElement en JsonDocument persistente
        private JsonDocument? ConvertJson(JsonElement? elemento)
        {
            if (!elemento.HasValue) return null;
            return JsonDocument.Parse(elemento.Value.GetRawText());
        }

        // --- VALIDACIONES PRIVADAS ---

        private async Task ValidarNombreUnicoAsync(string nombre)
        {
            // Comprobación de existencia en la base de datos (case-insensitive)
            bool existe = await _context.Personajes
                .AnyAsync(p => p.Nombre.ToLower() == nombre.ToLower());

            if (existe)
            {
                // Disparo de excepción personalizada para control de flujo en el controlador
                throw new NombreDuplicadoException($"El nombre '{nombre}' ya está en uso por otro héroe.");
            }
        }
    }

    // --- EXCEPCIONES PERSONALIZADAS ---

    public class NombreDuplicadoException : Exception
    {
        public NombreDuplicadoException(string message) : base(message) { }
    }
}