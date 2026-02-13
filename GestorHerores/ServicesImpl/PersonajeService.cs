using DandDSoft.Infrastructure.Data;
using GestorHeroes.Models;
using GestorHerores.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GestorHeroes.Services
{
    /*
     * Author: Álvaro Naranjo Rodriguez
     * Descripción: Implemento la lógica de negocio. La validación de estructura JSON ahora la maneja el framework automáticamente.
     */
    public class PersonajeService : IPersonajeService
    {
        private readonly GameDbContext _context;

        // Inyecto el contexto de la base de datos
        public PersonajeService(GameDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Personaje>> GetAllAsync()
        {
            return await _context.Personajes.ToListAsync();
        }

        public async Task<Personaje?> GetByIdAsync(int id)
        {
            return await _context.Personajes.FindAsync(id);
        }

        // --- MÉTODOS DE CREACIÓN ---

        public async Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto)
        {
            // Valido que el nombre sea único en el sistema antes de crear
            await ValidarNombreUnicoAsync(dto.Nombre);

            var guerrero = new Guerrero
            {
                Nombre = dto.Nombre,
                Nivel = dto.Nivel,
                FechaCreacion = DateTime.UtcNow,
                Gremio = dto.Gremio,
                ArmaPrincipal = dto.ArmaPrincipal,
                Furia = dto.Furia,
                Rasgos = ConvertJson(dto.Rasgos)
            };
            _context.Guerreros.Add(guerrero);
            await _context.SaveChangesAsync();
            return guerrero;
        }

        public async Task<Mago> CreateMagoAsync(MagoCreateDto dto)
        {
            // Compruebo la disponibilidad del nombre
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
            // Verifico unicidad del nombre
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
            // Aseguro que el nombre no esté duplicado
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

            // Si el nombre cambia, verifico que el nuevo no esté ocupado
            if (!string.Equals(personaje.Nombre, dto.Nombre, StringComparison.CurrentCultureIgnoreCase))
            {
                await ValidarNombreUnicoAsync(dto.Nombre);
            }

            personaje.Nombre = dto.Nombre;
            personaje.Nivel = dto.Nivel;
            personaje.Gremio = dto.Gremio;

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

        public async Task<IEnumerable<Personaje>> GetByRasgoAsync(string claveRasgo)
        {
            var todos = await _context.Personajes.ToListAsync();
            return todos.Where(p => p.Rasgos != null &&
                               p.Rasgos.RootElement.EnumerateObject()
                               .Any(prop => prop.Name.Equals(claveRasgo, StringComparison.OrdinalIgnoreCase)));
        }

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

        private JsonDocument? ConvertJson(JsonElement? elemento)
        {
            if (!elemento.HasValue) return null;
            return JsonDocument.Parse(elemento.Value.GetRawText());
        }

        // --- VALIDACIONES PRIVADAS ---

        private async Task ValidarNombreUnicoAsync(string nombre)
        {
            bool existe = await _context.Personajes
                .AnyAsync(p => p.Nombre.ToLower() == nombre.ToLower());

            if (existe)
            {
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