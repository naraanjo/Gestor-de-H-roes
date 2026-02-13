using System.ComponentModel.DataAnnotations;

namespace GestorHerores.DTO
{
    /*
     * Autor: Adrian Dondarza
     * Descripción: Defino el DTO específico para crear magos heredando la validación base.
     */
    public class MagoCreateDto : PersonajeBaseDto
    {
        public int Mana { get; set; }
        [Required]
        public string ElementoPrincipal { get; set; } = string.Empty;
    }
}