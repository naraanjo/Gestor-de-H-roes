using GestorHeroes.Models;
using GestorHerores.DTO;
using GestorHeroes.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestorHeroes.Controllers
{
    // Controlador encargado de gestionar los personajes del sistema
    [ApiController]
    [Route("api/[controller]")]
    public class PersonajesController : ControllerBase
    {
        private readonly IPersonajeService _personajeService;

        // Inyecto la dependencia del servicio de personajes
        public PersonajesController(IPersonajeService personajeService)
        {
            _personajeService = personajeService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var personajes = await _personajeService.GetAllAsync();
            return Ok(personajes);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var personaje = await _personajeService.GetByIdAsync(id);
            if (personaje == null) return NotFound();
            return Ok(personaje);
        }

        // --- MÉTODOS DE CREACIÓN ---

        [HttpPost("guerrero")]
        public async Task<IActionResult> CreateGuerrero([FromBody] GuerreroCreateDto dto)
        {
            try
            {
                var guerrero = await _personajeService.CreateGuerreroAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = guerrero.Id }, guerrero);
            }
            catch (NombreDuplicadoException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }

        [HttpPost("mago")]
        public async Task<IActionResult> CreateMago([FromBody] MagoCreateDto dto)
        {
            try
            {
                var mago = await _personajeService.CreateMagoAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = mago.Id }, mago);
            }
            catch (NombreDuplicadoException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }

        [HttpPost("arquero")]
        public async Task<IActionResult> CreateArquero([FromBody] ArqueroCreateDto dto)
        {
            try
            {
                var arquero = await _personajeService.CreateArqueroAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = arquero.Id }, arquero);
            }
            catch (NombreDuplicadoException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }

        [HttpPost("clerigo")]
        public async Task<IActionResult> CreateClerigo([FromBody] ClerigoCreateDto dto)
        {
            try
            {
                var clerigo = await _personajeService.CreateClerigoAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = clerigo.Id }, clerigo);
            }
            catch (NombreDuplicadoException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PersonajeBaseDto dto)
        {
            try
            {
                var actualizado = await _personajeService.UpdateAsync(id, dto);
                if (!actualizado) return NotFound();
                return NoContent();
            }
            catch (NombreDuplicadoException ex)
            {
                return Conflict(new { mensaje = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _personajeService.DeleteAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }

        [HttpGet("rasgo/{clave}")]
        public async Task<IActionResult> GetByRasgo(string clave)
        {
            var personajes = await _personajeService.GetByRasgoAsync(clave);
            return Ok(personajes);
        }

        [HttpGet("estadisticas/gremio")]
        public async Task<IActionResult> GetEstadisticasPorGremio()
        {
            var stats = await _personajeService.GetEstadisticasPorGremioAsync();
            return Ok(stats);
        }
    }
}