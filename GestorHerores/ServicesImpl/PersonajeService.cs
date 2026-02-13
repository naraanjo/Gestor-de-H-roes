using DandDSoft.Infrastructure.Data;
using GestorHeroes.Models;
using GestorHerores.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace GestorHeroes.Services
{
    /*
     * Author: Álvaro Naranjo Rodriguez
     * Descripción: Implemento la lógica de negocio asegurándome de validar que los nombres sean únicos.
     */
    public class PersonajeService : IPersonajeService
    {
        private readonly GameDbContext _context;

        // Inyecto el contexto de la base de datos para poder realizar las operaciones de persistencia
        public PersonajeService(GameDbContext context)
        {
            _context = context;
        }

        // Recupero el listado completo de personajes almacenados en la base de datos
        public async Task<IEnumerable<Personaje>> GetAllAsync()
        {
            return await _context.Personajes.ToListAsync();
        }

        // Busco un personaje en particular utilizando su identificador único
        public async Task<Personaje?> GetByIdAsync(int id)
        {
            return await _context.Personajes.FindAsync(id);
        }

        // A continuación implemento los métodos de creación validando previamente las reglas de negocio

        public async Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto)
        {
            // Primero valido que el nombre no exista ya para evitar duplicados
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
            // Verifico la disponibilidad del nombre antes de crear el objeto mago y persistirlo
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
            // Me aseguro de que el nombre sea único antes de proceder con la creación del arquero
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
            // Antes de guardar el clérigo compruebo que no exista otro héroe con el mismo nombre
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

            // Al actualizar compruebo si el nombre ha cambiado respecto al actual antes de validar unicidad
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
            // Recupero todos los personajes y filtro en memoria aquellos que contengan la clave de rasgo especificada en su estructura JSON
            var todos = await _context.Personajes.ToListAsync();
            return todos.Where(p => p.Rasgos != null &&
                               p.Rasgos.RootElement.EnumerateObject()
                               .Any(prop => prop.Name.Equals(claveRasgo, StringComparison.OrdinalIgnoreCase)));
        }

        public async Task<object> GetEstadisticasPorGremioAsync()
        {
            // Agrupo los personajes por su gremio y calculo métricas como la cantidad total y el nivel promedio de cada grupo
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
            // Transformo el elemento JSON recibido en un documento persistente si este contiene valor
            if (!elemento.HasValue) return null;
            return JsonDocument.Parse(elemento.Value.GetRawText());
        }

        // Defino este método privado para encapsular la verificación de nombres en la base de datos
        private async Task ValidarNombreUnicoAsync(string nombre)
        {
            // Compruebo en la base de datos si existe algún personaje con el mismo nombre ignorando mayúsculas y minúsculas
            bool existe = await _context.Personajes
                .AnyAsync(p => p.Nombre.ToLower() == nombre.ToLower());

            if (existe)
            {
                // Si encuentro coincidencias lanzo una excepción personalizada para detener el proceso
                throw new NombreDuplicadoException($"El nombre '{nombre}' ya está en uso por otro héroe.");
            }
        }
    }

    // Defino una excepción personalizada para notificar errores de conflicto de nombres
    public class NombreDuplicadoException : Exception
    {
        public NombreDuplicadoException(string message) : base(message) { }
    }
}