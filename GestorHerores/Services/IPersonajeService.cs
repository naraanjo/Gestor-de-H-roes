using GestorHeroes.Models;
using GestorHerores.DTO;

namespace GestorHeroes.Services
{
    /*
     * Author: Álvaro Naranjo Rodriguez
     * Descripción: Defino el contrato que debe cumplir el servicio encargado de la lógica de negocio de los personajes.
     */
    public interface IPersonajeService
    {
        Task<IEnumerable<Personaje>> GetAllAsync();
        Task<Personaje?> GetByIdAsync(int id);

        Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto);
        Task<Mago> CreateMagoAsync(MagoCreateDto dto);
        Task<Arquero> CreateArqueroAsync(ArqueroCreateDto dto);
        Task<Clerigo> CreateClerigoAsync(ClerigoCreateDto dto);

        Task<bool> UpdateAsync(int id, PersonajeBaseDto dto);
        Task<bool> DeleteAsync(int id);

        Task<IEnumerable<Personaje>> GetByRasgoAsync(string claveRasgo);
        Task<object> GetEstadisticasPorGremioAsync();
    }
}