using GestorHeroes.Models;
using GestorHerores.DTO;

namespace GestorHeroes.Services
{
    /*
     * Author: Álvaro Naranjo Rodriguez
     * Descripción: Interfaz para gestionar la lógica de negocio de los personajes.
     */
    public interface IPersonajeService
    {
        // --- CRUD Básico ---
        Task<IEnumerable<Personaje>> GetAllAsync();
        Task<Personaje?> GetByIdAsync(int id);

        // --- Creación (Específica por tipo para TPT) ---
        Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto);
        Task<Mago> CreateMagoAsync(MagoCreateDto dto);
        Task<Arquero> CreateArqueroAsync(ArqueroCreateDto dto);
        Task<Clerigo> CreateClerigoAsync(ClerigoCreateDto dto);

        // --- Actualizar y Borrar ---
        Task<bool> UpdateAsync(int id, PersonajeBaseDto dto);
        Task<bool> DeleteAsync(int id);

        // --- Consultas Complejas ---
        Task<IEnumerable<Personaje>> GetByRasgoAsync(string claveRasgo);
        Task<object> GetEstadisticasPorGremioAsync();
    }
}