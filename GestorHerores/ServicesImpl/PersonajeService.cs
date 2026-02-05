using DandDSoft.Infrastructure.Data;
using GestorHeroes.Models; // Tus modelos
using GestorHerores.DTO;   // Tus DTOs
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GestorHeroes.Services
{
    /*
     Author: Álvaro Naranjo Rodriguez
     Descripción: Implementación de la lógica de negocio.
     Maneja mapeo manual de DTOs y lógica de JSONB.
    */
    public class PersonajeService : IPersonajeService
    {
        private readonly GameDbContext _context;

        public PersonajeService(GameDbContext context)
        {
            _context = context;
        }

        // 1. GET ALL: Obtener todos (mezcla polimórfica)
        // EF Core traerá automáticamente Guerreros, Magos, etc. gracias a TPT.
        public async Task<IEnumerable<Personaje>> GetAllAsync()
        {
            return await _context.Personajes.ToListAsync();
        }

        // 2. GET BY ID
        public async Task<Personaje?> GetByIdAsync(int id)
        {
            return await _context.Personajes.FindAsync(id);
        }

        // 3. CREATE (Métodos específicos por tipo)
        // Mapeamos manualmente de DTO a Entidad incluyendo el JSON.

        public async Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto)
        {
            var guerrero = new Guerrero
            {
                Nombre = dto.Nombre,
                Nivel = dto.Nivel,
                FechaCreacion = DateTime.UtcNow, // Obligatorio según modelo
                Gremio = dto.Gremio,
                ArmaPrincipal = dto.ArmaPrincipal,
                Furia = dto.Furia,
                Rasgos = ConvertJson(dto.Rasgos) // Conversión auxiliar
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

        // 4. UPDATE
        // Actualiza los datos base y el JSON.
        public async Task<bool> UpdateAsync(int id, PersonajeBaseDto dto)
        {
            var personaje = await _context.Personajes.FindAsync(id);
            if (personaje == null) return false;

            personaje.Nombre = dto.Nombre;
            personaje.Nivel = dto.Nivel;
            personaje.Gremio = dto.Gremio;

            // Actualizamos el JSON si viene nuevo dato
            if (dto.Rasgos.HasValue)
            {
                personaje.Rasgos = ConvertJson(dto.Rasgos);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        // 5. DELETE
        public async Task<bool> DeleteAsync(int id)
        {
            var personaje = await _context.Personajes.FindAsync(id);
            if (personaje == null) return false;

            _context.Personajes.Remove(personaje);
            await _context.SaveChangesAsync();
            return true;
        }

        // --- CONSULTAS COMPLEJAS ---

        // Consulta 1: Filtrado Profundo por JSON 
        // Busca personajes que tengan una clave específica en su JSON (ej. "MiedoA").
        public async Task<IEnumerable<Personaje>> GetByRasgoAsync(string claveRasgo)
        {
            // Nota: En EF Core 9 con Npgsql, esto se traduce a consultas JSONB nativas.
            return await _context.Personajes
                .Where(p => p.Rasgos != null && p.Rasgos.RootElement.GetProperty(claveRasgo).ValueKind != JsonValueKind.Undefined)
                .ToListAsync();
        }

        // Consulta 2: Agrupación (Estadísticas por Gremio) 
        // Cuenta personajes por gremio y saca la media de nivel.
        public async Task<object> GetEstadisticasPorGremioAsync()
        {
            var stats = await _context.Personajes
                .GroupBy(p => p.Gremio)
                .Select(g => new
                {
                    Gremio = g.Key ?? "Sin Gremio",
                    Cantidad = g.Count(),
                    NivelPromedio = g.Average(p => p.Nivel)
                })
                .ToListAsync();

            return stats;
        }

        // --- Helpers ---

        // Convierte de JsonElement (DTO) a JsonDocument (Entity/DB)
        private JsonDocument? ConvertJson(JsonElement? elemento)
        {
            if (!elemento.HasValue) return null;
            // Parseamos el contenido crudo para crear un documento independiente
            return JsonDocument.Parse(elemento.Value.GetRawText());
        }
    }
}