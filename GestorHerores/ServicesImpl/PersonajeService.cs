using DandDSoft.Infrastructure.Data;
using GestorHeroes.Models;
using GestorHerores.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GestorHeroes.Services
{
    /*
     * Author: Álvaro Naranjo Rodriguez
     * Descripción: Implemento la lógica de negocio asegurándome de validar nombres únicos y atributos desconocidos.
     */
    public class PersonajeService : IPersonajeService
    {
        private readonly GameDbContext _context;

        // Inyecto el contexto de la base de datos para realizar operaciones de persistencia
        public PersonajeService(GameDbContext context)
        {
            _context = context;
        }

        // Recupero el listado completo de personajes almacenados
        public async Task<IEnumerable<Personaje>> GetAllAsync()
        {
            return await _context.Personajes.ToListAsync();
        }

        // Busco un personaje particular por su identificador único
        public async Task<Personaje?> GetByIdAsync(int id)
        {
            return await _context.Personajes.FindAsync(id);
        }

        // --- MÉTODOS DE CREACIÓN CON DOBLE VALIDACIÓN ---

        public async Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto)
        {
            // Primero verifico que no se hayan enviado atributos que no correspondan a un Guerrero
            ValidarAtributosDesconocidos(dto.DatosExtra, "Guerrero");

            // Valido que el nombre sea único en el sistema
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
            // Valido la estructura del JSON y la unicidad del nombre antes de crear el Mago
            ValidarAtributosDesconocidos(dto.DatosExtra, "Mago");
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
            // Me aseguro de que los datos sean estrictamente de Arquero y el nombre sea único
            ValidarAtributosDesconocidos(dto.DatosExtra, "Arquero");
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
            // Realizo las validaciones de negocio y estructura antes de guardar el Clérigo
            ValidarAtributosDesconocidos(dto.DatosExtra, "Clerigo");
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
            // También valido en la actualización para evitar que envíen campos que no se pueden actualizar por esta vía
            ValidarAtributosDesconocidos(dto.DatosExtra, "Personaje (Base)");

            var personaje = await _context.Personajes.FindAsync(id);
            if (personaje == null) return false;

            // Valido unicidad solo si el nombre ha cambiado
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
            // Filtro en memoria los personajes que contienen la clave solicitada en su JSON de rasgos
            var todos = await _context.Personajes.ToListAsync();
            return todos.Where(p => p.Rasgos != null &&
                               p.Rasgos.RootElement.EnumerateObject()
                               .Any(prop => prop.Name.Equals(claveRasgo, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<object> GetEstadisticasPorGremioAsync()
        {
            // Calculo estadísticas agrupadas por gremio
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

        // Compruebo si ya existe un personaje con el mismo nombre en la base de datos
        private async Task ValidarNombreUnicoAsync(string nombre)
        {
            bool existe = await _context.Personajes
                .AnyAsync(p => p.Nombre.ToLower() == nombre.ToLower());

            if (existe)
            {
                throw new NombreDuplicadoException($"El nombre '{nombre}' ya está en uso por otro héroe.");
            }
        }

        // Verifico si el DTO ha capturado propiedades que no deberían estar ahí
        private void ValidarAtributosDesconocidos(Dictionary<string, object>? extras, string tipo)
        {
            if (extras != null && extras.Count > 0)
            {
                string camposInvalidos = string.Join(", ", extras.Keys);
                throw new AtributosNoValidosException($"Error: El tipo '{tipo}' NO admite los campos: [{camposInvalidos}]. Por favor revisa el JSON.");
            }
        }
    }

    // --- EXCEPCIONES PERSONALIZADAS ---

    public class NombreDuplicadoException : Exception
    {
        public NombreDuplicadoException(string message) : base(message) { }
    }

    // Añado esta excepción para controlar errores de estructura en la petición
    public class AtributosNoValidosException : Exception
    {
        public AtributosNoValidosException(string message) : base(message) { }
    }
}