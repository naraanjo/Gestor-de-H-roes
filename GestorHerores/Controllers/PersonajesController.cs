using GestorHeroes.Models;
using GestorHerores.DTO;
using GestorHeroes.Services;
using Microsoft.AspNetCore.Mvc;

namespace GestorHeroes.Controllers
{
    // Aquí defino la clase controladora que se encarga de gestionar las peticiones relacionadas con los personajes
    // Me aseguro de heredar de ControllerBase y decoro la clase para que funcione como un controlador de API
    [ApiController]
    [Route("api/[controller]")]
    public class PersonajesController : ControllerBase
    {
        private readonly IPersonajeService _personajeService;

        // En este constructor inyecto la dependencia del servicio de personajes para poder utilizar sus métodos
        public PersonajesController(IPersonajeService personajeService)
        {
            _personajeService = personajeService;
        }

        // Cuando recibo una petición GET genérica, solicito al servicio todos los personajes y los devuelvo en una respuesta exitosa
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var personajes = await _personajeService.GetAllAsync();
            return Ok(personajes);
        }

        // Busco un personaje específico por su identificador; si no lo encuentro devuelvo un error NotFound, de lo contrario devuelvo el personaje
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var personaje = await _personajeService.GetByIdAsync(id);
            if (personaje == null) return NotFound();
            return Ok(personaje);
        }

        // A continuación agrupo los métodos de creación donde controlo explícitamente las excepciones de negocio

        // Intento crear un guerrero envolviendo la llamada en un bloque try para capturar posibles errores
        [HttpPost("guerrero")]
        public async Task<IActionResult> CreateGuerrero([FromBody] GuerreroCreateDto dto)
        {
            try
            {
                var guerrero = await _personajeService.CreateGuerreroAsync(dto);
                // Si todo sale bien, indico dónde se puede consultar el nuevo recurso creado
                return CreatedAtAction(nameof(GetById), new { id = guerrero.Id }, guerrero);
            }
            catch (NombreDuplicadoException ex)
            {
                // Si capturo la excepción de nombre duplicado, devuelvo un conflicto HTTP con el mensaje de error
                return Conflict(new { mensaje = ex.Message });
            }
        }

        // Realizo el mismo proceso para crear un mago, controlando que el nombre no esté repetido
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
                // Aquí también devuelvo un conflicto si detecto que el nombre ya existe
                return Conflict(new { mensaje = ex.Message });
            }
        }

        // Gestiono la creación de un arquero bajo la misma lógica de control de excepciones
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

        // Finalmente gestiono la creación de un clérigo asegurándome de informar si hay conflicto de nombres
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

        // Intento actualizar la información de un personaje existente
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] PersonajeBaseDto dto)
        {
            try
            {
                var actualizado = await _personajeService.UpdateAsync(id, dto);

                // Si el servicio me indica que no pudo actualizar porque no existe el ID, devuelvo NotFound
                if (!actualizado) return NotFound();

                // Si la actualización fue exitosa, devuelvo NoContent
                return NoContent();
            }
            catch (NombreDuplicadoException ex)
            {
                // Incluso al actualizar, si el nuevo nombre entra en conflicto con otro, devuelvo el error correspondiente
                return Conflict(new { mensaje = ex.Message });
            }
        }

        // Solicito la eliminación de un personaje por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var eliminado = await _personajeService.DeleteAsync(id);
            // Verifico si la eliminación fue posible, si no encuentro el recurso devuelvo NotFound
            if (!eliminado) return NotFound();
            return NoContent();
        }

        // Busco y devuelvo los personajes que contengan un rasgo específico en su JSON
        [HttpGet("rasgo/{clave}")]
        public async Task<IActionResult> GetByRasgo(string clave)
        {
            var personajes = await _personajeService.GetByRasgoAsync(clave);
            return Ok(personajes);
        }

        // Solicito al servicio las estadísticas agrupadas por gremio y las devuelvo al cliente
        [HttpGet("estadisticas/gremio")]
        public async Task<IActionResult> GetEstadisticasPorGremio()
        {
            var stats = await _personajeService.GetEstadisticasPorGremioAsync();
            return Ok(stats);
        }
    }
}