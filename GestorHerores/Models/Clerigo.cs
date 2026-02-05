using System.ComponentModel.DataAnnotations.Schema;

namespace GestorHeroes.Models
{
    /*
     Author: Álvaro Naranjo Rodriguez
    */


    [Table("Clerigos")] // Tabla propia
    public class Clerigo : Personaje
    {
        public string Deidad { get; set; } = string.Empty;
        public int PuntosSanacion { get; set; }
    }
}