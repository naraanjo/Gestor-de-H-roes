using System.ComponentModel.DataAnnotations;

namespace GestorHerores.DTO
{
    /*
     * Autor: Adrian Dondarza
     * Descripción: Defino el DTO específico para crear clérigos heredando la validación base.
     */
    public class ClerigoCreateDto : PersonajeBaseDto
    {
        [Required]
        public string Deidad { get; set; } = string.Empty;
        public int PuntosSanacion { get; set; }
    }
}