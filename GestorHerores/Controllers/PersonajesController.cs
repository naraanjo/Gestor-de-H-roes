using GestorHeroes.Models;
using GestorHerores.DTO;
using GestorHeroes.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestorHeroes.Controllers
{
    // Defino el controlador para gestionar las peticiones HTTP relacionadas con los personajes
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

        // Devuelvo todos los personajes registrados
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var personajes = await _personajeService.GetAllAsync();
            return Ok(personajes);
        }

        // Busco un personaje por ID y devuelvo NotFound si no existe
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var personaje = await _personajeService.GetByIdAsync(id);
            if (personaje == null) return NotFound();
            return Ok(personaje);
        }

        // --- MÉTODOS DE CREACIÓN CON MANEJO DE ERRORES ---

        // Intento crear un Guerrero gestionando posibles errores de validación
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
                // Si el nombre está duplicado devuelvo Conflict (409)
                return Conflict(new { mensaje = ex.Message });
            }
            catch (AtributosNoValidosException ex)
            {
                // Si hay atributos desconocidos devuelvo Bad Request (400)
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // Intento crear un Mago con el mismo control de excepciones
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
            catch (AtributosNoValidosException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // Intento crear un Arquero gestionando errores
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
            catch (AtributosNoValidosException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // Intento crear un Clérigo gestionando errores
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
            catch (AtributosNoValidosException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // Actualizo un personaje existente validando también los datos de entrada
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
            catch (AtributosNoValidosException ex)
            {
                return BadRequest(new { mensaje = ex.Message });
            }
        }

        // Elimino un personaje del sistema
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _personajeService.DeleteAsync(id);
            if (!eliminado) return NotFound();
            return NoContent();
        }

        // Busco personajes por un rasgo JSON específico
        [HttpGet("rasgo/{clave}")]
        public async Task<IActionResult> GetByRasgo(string clave)
        {
            var personajes = await _personajeService.GetByRasgoAsync(clave);
            return Ok(personajes);
        }

        // Obtengo las estadísticas del gremio
        [HttpGet("estadisticas/gremio")]
        public async Task<IActionResult> GetEstadisticasPorGremio()
        {
            var stats = await _personajeService.GetEstadisticasPorGremioAsync();
            return Ok(stats);
        }
    }
}