using System.ComponentModel.DataAnnotations;

namespace GestorHerores.DTO
{
    /*
     * Autor: Adrian Dondarza
     * Descripción: Defino el DTO específico para crear guerreros heredando la validación base.
     */
    public class GuerreroCreateDto : PersonajeBaseDto
    {
        [Required]
        public string ArmaPrincipal { get; set; } = string.Empty;
        public int Furia { get; set; }
    }
}