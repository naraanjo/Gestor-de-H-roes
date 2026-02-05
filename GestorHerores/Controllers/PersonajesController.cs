using GestorHeroes.Models;
using GestorHerores.DTO;
using GestorHeroes.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestorHeroes.Controllers
{
    /// <summary>
    /// Controlador encargado de gestionar los personajes del sistema.
    /// Proporciona endpoints CRUD y consultas avanzadas sobre personajes polimórficos.
    /// </summary>
    /// <author>
    /// Pablo Rubio Prado
    /// </author>
    [ApiController]
    [Route("api/[controller]")]
    public class PersonajesController : ControllerBase
    {
        private readonly IPersonajeService _personajeService;

        public PersonajesController(IPersonajeService personajeService)
        {
            _personajeService = personajeService;
        }

        /// <summary>
        /// Obtiene todos los personajes del sistema.
        /// </summary>
        /// <returns>Listado completo de personajes.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var personajes = await _personajeService.GetAllAsync();
            return Ok(personajes);
        }

        /// <summary>
        /// Obtiene un personaje por su identificador.
        /// </summary>
        /// <param name="id">ID del personaje.</param>
        /// <returns>Personaje encontrado o 404.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var personaje = await _personajeService.GetByIdAsync(id);

            if (personaje == null)
                return NotFound();

            return Ok(personaje);
        }

        /// <summary>
        /// Crea un nuevo guerrero.
        /// </summary>
        /// <param name="dto">Datos del guerrero.</param>
        /// <returns>Guerrero creado.</returns>
        [HttpPost("guerrero")]
        public async Task<IActionResult> CreateGuerrero([FromBody] GuerreroCreateDto dto)
        {
            var guerrero = await _personajeService.CreateGuerreroAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = guerrero.Id }, guerrero);
        }

        /// <summary>
        /// Crea un nuevo mago.
        /// </summary>
        /// <param name="dto">Datos del mago.</param>
        /// <returns>Mago creado.</returns>
        [HttpPost("mago")]
        public async Task<IActionResult> CreateMago([FromBody] MagoCreateDto dto)
        {
            var mago = await _personajeService.CreateMagoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = mago.Id }, mago);
        }

        /// <summary>
        /// Crea un nuevo arquero.
        /// </summary>
        /// <param name="dto">Datos del arquero.</param>
        /// <returns>Arquero creado.</returns>
        [HttpPost("arquero")]
        public async Task<IActionResult> CreateArquero([FromBody] ArqueroCreateDto dto)
        {
            var arquero = await _personajeService.CreateArqueroAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = arquero.Id }, arquero);
        }

        /// <summary>
        /// Crea un nuevo clérigo.
        /// </summary>
        /// <param name="dto">Datos del clérigo.</param>
        /// <returns>Clérigo creado.</returns>
        [HttpPost("clerigo")]
        public async Task<IActionResult> CreateClerigo([FromBody] ClerigoCreateDto dto)
        {
            var clerigo = await _personajeService.CreateClerigoAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = clerigo.Id }, clerigo);
        }

        /// <summary>
        /// Actualiza los datos básicos de un personaje.
        /// </summary>
        /// <param name="id">ID del personaje.</param>
        /// <param name="dto">Datos base actualizados.</param>
        /// <returns>Resultado de la operación.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PersonajeBaseDto dto)
        {
            var actualizado = await _personajeService.UpdateAsync(id, dto);

            if (!actualizado)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Elimina un personaje por su identificador.
        /// </summary>
        /// <param name="id">ID del personaje.</param>
        /// <returns>Resultado de la eliminación.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _personajeService.DeleteAsync(id);

            if (!eliminado)
                return NotFound();

            return NoContent();
        }

        /// <summary>
        /// Obtiene personajes que contienen un rasgo específico en su JSON.
        /// </summary>
        /// <param name="clave">Clave del rasgo a buscar.</param>
        /// <returns>Listado filtrado.</returns>
        [HttpGet("rasgo/{clave}")]
        public async Task<IActionResult> GetByRasgo(string clave)
        {
            var personajes = await _personajeService.GetByRasgoAsync(clave);
            return Ok(personajes);
        }

        /// <summary>
        /// Obtiene estadísticas agrupadas por gremio.
        /// </summary>
        /// <returns>Datos estadísticos.</returns>
        [HttpGet("estadisticas/gremio")]
        public async Task<IActionResult> GetEstadisticasPorGremio()
        {
            var stats = await _personajeService.GetEstadisticasPorGremioAsync();
            return Ok(stats);
        }
    }
}