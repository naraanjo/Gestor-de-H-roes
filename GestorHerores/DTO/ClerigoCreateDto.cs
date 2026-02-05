using System.ComponentModel.DataAnnotations;

namespace GestorHerores.DTO
{
    public class ClerigoCreateDto : PersonajeBaseDto
    {
        [Required]
        public string Deidad { get; set; } = string.Empty;
        public int PuntosSanacion { get; set; }
    }
}
