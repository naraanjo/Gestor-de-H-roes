using System.ComponentModel.DataAnnotations.Schema;

namespace GestorHeroes.Models
{
    /*
     Author: Álvaro Naranjo Rodriguez
    */


    [Table("Arqueros")] // Tabla propia
    public class Arquero : Personaje
    {
        public double Precision { get; set; }
        public bool TieneMascota { get; set; }
    }
}