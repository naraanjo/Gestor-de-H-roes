namespace GestorHerores.DTO
{
    /*
     * DTO para la clase Arquero
     * Autor: Adrian Dondarza
     */
    public class ArqueroCreateDto : PersonajeBaseDto
    {
        public double Precision { get; set; }
        public bool TieneMascota { get; set; }
    }
}