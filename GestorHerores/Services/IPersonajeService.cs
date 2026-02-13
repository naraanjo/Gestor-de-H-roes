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
        // Declaro los métodos necesarios para recuperar el listado completo o un personaje individual por su identificador
        Task<IEnumerable<Personaje>> GetAllAsync();
        Task<Personaje?> GetByIdAsync(int id);

        // Establezco las firmas para la creación de cada tipo específico de héroe respetando la herencia
        Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto);
        Task<Mago> CreateMagoAsync(MagoCreateDto dto);
        Task<Arquero> CreateArqueroAsync(ArqueroCreateDto dto);
        Task<Clerigo> CreateClerigoAsync(ClerigoCreateDto dto);

        // Defino las operaciones que permiten modificar los datos de un personaje o eliminarlo del sistema
        Task<bool> UpdateAsync(int id, PersonajeBaseDto dto);
        Task<bool> DeleteAsync(int id);

        // Incluyo los métodos para realizar búsquedas específicas por rasgos y obtener cálculos estadísticos
        Task<IEnumerable<Personaje>> GetByRasgoAsync(string claveRasgo);
        Task<object> GetEstadisticasPorGremioAsync();
    }
}