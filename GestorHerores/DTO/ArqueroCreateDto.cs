using System.ComponentModel.DataAnnotations;

namespace GestorHerores.DTO
{
    /*
     * Autor: Adrian Dondarza
     * Descripción: Defino el DTO específico para crear arqueros heredando la validación base.
     */
    public class ArqueroCreateDto : PersonajeBaseDto
    {
        [Required]
        public double Precision { get; set; }
        public bool TieneMascota { get; set; }
    }
}