using GestorHeroes.Models;
using GestorHerores.DTO;

namespace GestorHeroes.Services
{
    /*
     Author: Álvaro Naranjo Rodriguez
     Descripción: Interfaz para gestionar la lógica de negocio de los personajes.
    */
    public interface IPersonajeService
    {
        // --- CRUD Básico ---

        // Obtener todos los personajes (mezcla polimórfica)
        Task<IEnumerable<Personaje>> GetAllAsync();

        // Obtener un personaje por su ID
        Task<Personaje?> GetByIdAsync(int id);

        // --- Creación (Específica por tipo para TPT) ---

        Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto);
        Task<Mago> CreateMagoAsync(MagoCreateDto dto);
        Task<Arquero> CreateArqueroAsync(ArqueroCreateDto dto);
        Task<Clerigo> CreateClerigoAsync(ClerigoCreateDto dto);

        // --- Actualizar y Borrar ---

        // Actualizar datos comunes y el JSON
        Task<bool> UpdateAsync(int id, PersonajeBaseDto dto);

        // Borrar personaje
        Task<bool> DeleteAsync(int id);

        // --- Consultas Complejas ---

        // 1. Filtrado Profundo por JSON (Buscar por clave en 'Rasgos')
        Task<IEnumerable<Personaje>> GetByRasgoAsync(string nombreRasgo);

        // 2. Agrupación Polimórfica (Estadísticas por Gremio)
        Task<object> GetEstadisticasPorGremioAsync();
    }
}