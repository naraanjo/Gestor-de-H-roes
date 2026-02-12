using System.ComponentModel.DataAnnotations;

namespace GestorHerores.DTO
{
    /*
     * DTO para la clase Guerrero
     * Autor: Adrian Dondarza
     */
    public class GuerreroCreateDto : PersonajeBaseDto
    {
        [Required]
        public string ArmaPrincipal { get; set; } = string.Empty;
        public int Furia { get; set; }
    }
}