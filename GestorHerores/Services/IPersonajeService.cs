using GestorHeroes.Models;
using GestorHerores.DTO;

namespace GestorHeroes.Services
{
    /*
     * Author: Álvaro Naranjo Rodriguez
     * Descripción: Interfaz que define el contrato para el servicio de lógica de negocio. 
     * Establece las operaciones necesarias para el manejo de la jerarquía polimórfica y datos dinámicos.
     */
    public interface IPersonajeService
    {
        // Recuperación de la colección completa de personajes (comportamiento polimórfico)
        Task<IEnumerable<Personaje>> GetAllAsync();

        // Obtención de un registro único mediante su identificador
        Task<Personaje?> GetByIdAsync(int id);

        // --- OPERACIONES DE CREACIÓN POR TIPO (ESTRATEGIA TPT) ---

        Task<Guerrero> CreateGuerreroAsync(GuerreroCreateDto dto);
        Task<Mago> CreateMagoAsync(MagoCreateDto dto);
        Task<Arquero> CreateArqueroAsync(ArqueroCreateDto dto);
        Task<Clerigo> CreateClerigoAsync(ClerigoCreateDto dto);

        // --- OPERACIONES DE PERSISTENCIA ---

        // Actualización de los datos base y el campo dinámico JSONB
        Task<bool> UpdateAsync(int id, PersonajeBaseDto dto);

        // Eliminación física del registro en la base de datos
        Task<bool> DeleteAsync(int id);

        // --- CONSULTAS ESPECIALIZADAS ---

        // Filtrado de personajes basado en la existencia de una clave en el campo JSONB
        Task<IEnumerable<Personaje>> GetByRasgoAsync(string claveRasgo);

        // Cálculo de métricas agregadas y estadísticas por agrupación de Gremios
        Task<object> GetEstadisticasPorGremioAsync();
    }
}