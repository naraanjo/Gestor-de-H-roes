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
     * Esta clase gestiona la persistencia polimórfica (TPT) y el procesamiento 
     * de datos dinámicos mediante JSONB en PostgreSQL.
     */
    public class PersonajeService : IPersonajeService
    {
        private readonly GameDbContext _context;

        // Inyección del contexto de base de datos a través del constructor
        public PersonajeService(GameDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Obtiene todos los personajes registrados en el sistema.
        /// EF Core resuelve automáticamente el polimorfismo gracias a la estrategia TPT.
        /// </summary>
        public async Task<IEnumerable<Personaje>> GetAllAsync()
        {
            return await _context.Personajes.ToListAsync();
        }

        /// <summary>
        /// Busca un personaje específico por su identificador único.
        /// </summary>
        public async Task<Personaje?> GetByIdAsync(int id)
        {
            return await _context.Personajes.FindAsync(id);
        }

        // --- MÉTODOS DE CREACIÓN (Implementación TPT) ---
        // Cada método mapea el DTO específico a su entidad correspondiente en la base de datos.

        public async Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto)
        {
            var guerrero = new Guerrero
            {
                Nombre = dto.Nombre,
                Nivel = dto.Nivel,
                FechaCreacion = DateTime.UtcNow, // Fecha automática en UTC
                Gremio = dto.Gremio,
                ArmaPrincipal = dto.ArmaPrincipal,
                Furia = dto.Furia,
                Rasgos = ConvertJson(dto.Rasgos) // Conversión de JsonElement a JsonDocument
            };
            _context.Guerreros.Add(guerrero);
            await _context.SaveChangesAsync();
            return guerrero;
        }

        public async Task<Mago> CreateMagoAsync(MagoCreateDto dto)
        {
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

        /// <summary>
        /// Actualiza los datos base de un personaje existente y sus rasgos dinámicos.
        /// </summary>
        public async Task<bool> UpdateAsync(int id, PersonajeBaseDto dto)
        {
            var personaje = await _context.Personajes.FindAsync(id);
            if (personaje == null) return false;

            // Actualización de campos comunes
            personaje.Nombre = dto.Nombre;
            personaje.Nivel = dto.Nivel;
            personaje.Gremio = dto.Gremio;

            // Actualización del documento JSON si se proporciona en el DTO
            if (dto.Rasgos.HasValue)
            {
                personaje.Rasgos = ConvertJson(dto.Rasgos);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Elimina un personaje del sistema por su ID.
        /// </summary>
        public async Task<bool> DeleteAsync(int id)
        {
            var personaje = await _context.Personajes.FindAsync(id);
            if (personaje == null) return false;

            _context.Personajes.Remove(personaje);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- CONSULTAS COMPLEJAS (Requisito Punto 6) ---

        /// <summary>
        /// Filtrado Profundo por JSON: Busca personajes que posean una clave específica 
        /// dentro de su columna 'Rasgos' (JSONB).
        /// </summary>
        public async Task<IEnumerable<Personaje>> GetByRasgoAsync(string claveRasgo)
        {
            // Nota: Se realiza una búsqueda insensible a mayúsculas/minúsculas sobre las claves del JSON
            var todos = await _context.Personajes.ToListAsync();
            return todos.Where(p => p.Rasgos != null &&
                               p.Rasgos.RootElement.EnumerateObject()
                               .Any(prop => prop.Name.Equals(claveRasgo, StringComparison.OrdinalIgnoreCase)));
        }

        /// <summary>
        /// Agrupación Polimórfica: Genera estadísticas de nivel promedio y conteo agrupados por Gremio.
        /// </summary>
        public async Task<object> GetEstadisticasPorGremioAsync()
        {
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

        // --- HELPERS ---

        /// <summary>
        /// Método auxiliar para transformar un JsonElement (volátil) en un JsonDocument 
        /// persistente apto para ser almacenado en PostgreSQL.
        /// </summary>
        private JsonDocument? ConvertJson(JsonElement? elemento)
        {
            if (!elemento.HasValue) return null;
            // Parseamos el RawText para asegurar que el documento sea independiente del ciclo de vida del request
            return JsonDocument.Parse(elemento.Value.GetRawText());
        }
    }
}