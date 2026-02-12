using System.ComponentModel.DataAnnotations;

namespace GestorHerores.DTO
{
    /*
     * DTO para la clase Clerigo
     * Autor: Adrian Dondarza
     */
    public class ClerigoCreateDto : PersonajeBaseDto
    {
        [Required]
        public string Deidad { get; set; } = string.Empty;
        public int PuntosSanacion { get; set; }
    }
}