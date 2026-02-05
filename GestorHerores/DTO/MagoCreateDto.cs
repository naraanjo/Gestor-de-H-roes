using System.ComponentModel.DataAnnotations;

namespace GestorHerores.DTO
{
    public class MagoCreateDto : PersonajeBaseDto
    {
        public int Mana { get; set; }
        [Required]
        public string ElementoPrincipal { get; set; } = string.Empty;
    }
}
