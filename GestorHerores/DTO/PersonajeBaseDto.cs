using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GestorHerores.DTO
{
    /*
     * Autor: Adrian Dondarza
     * Descripción: Defino la estructura base para los DTOs de personajes e incluyo la capacidad de detectar atributos no válidos.
     */
    public class PersonajeBaseDto
    {
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "El nivel debe estar entre 1 y 100")]
        public int Nivel { get; set; }

        public string? Gremio { get; set; }

        // Campo para los rasgos dinámicos (JSONB)
        public JsonElement? Rasgos { get; set; }

        // Añado este diccionario para capturar cualquier propiedad del JSON que no coincida con la clase
        // Esto me permite validar posteriormente si el usuario envió datos incorrectos
        [JsonExtensionData]
        public Dictionary<string, object>? DatosExtra { get; set; }
    }
}