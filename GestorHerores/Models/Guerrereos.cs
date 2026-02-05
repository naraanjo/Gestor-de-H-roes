using System.ComponentModel.DataAnnotations.Schema;

namespace GestorHeroes.Models
{
    /*
     Author: Álvaro Naranjo Rodriguez
    */


    [Table("Guerreros")] // Tabla propia
    public class Guerrero : Personaje
    {
        public string ArmaPrincipal { get; set; } = string.Empty;
        public int Furia { get; set; }
    }
}