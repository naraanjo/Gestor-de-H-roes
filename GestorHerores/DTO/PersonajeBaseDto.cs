using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GestorHerores.DTO
{
    public abstract class PersonajeBaseDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "El nivel debe estar entre 1 y 100")]
        public int Nivel { get; set; }

        public string? Gremio { get; set; }

        // Campo para los rasgos dinámicos (JSONB)
        public JsonElement? Rasgos { get; set; }
    }
}
