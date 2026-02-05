using System.ComponentModel.DataAnnotations;

namespace GestorHerores.DTO
{
    public class GuerreroCreateDto : PersonajeBaseDto
    {
        [Required]
        public string ArmaPrincipal { get; set; } = string.Empty;
        public int Furia { get; set; }
    }
}
