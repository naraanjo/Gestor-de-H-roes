using System.ComponentModel.DataAnnotations;

namespace GestorHerores.DTO
{
    /*
     * DTO para la clase Mago
     * Autor: Adrian Dondarza
     */
    public class MagoCreateDto : PersonajeBaseDto
    {
        public int Mana { get; set; }
        [Required]
        public string ElementoPrincipal { get; set; } = string.Empty;
    }
}