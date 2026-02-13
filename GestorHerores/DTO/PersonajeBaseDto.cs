using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace GestorHerores.DTO
{
    /*
     * Autor: Adrian Dondarza
     * Descripción: Defino la estructura base para los DTOs de personajes.
     * Al usar .NET 8 UnmappedMemberHandling, no necesito ensuciar esta clase con diccionarios de validación.
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
    }
}