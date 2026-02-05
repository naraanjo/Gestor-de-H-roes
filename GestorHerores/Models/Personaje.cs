using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json; // Necesario para JsonDocument

namespace GestorHeroes.Models
{
    /*
     Author: Álvaro Naranjo Rodriguez
    */

    // Define la tabla base para la estrategia TPT (Table Per Type)
    // Se fuerza el nombre de la tabla a "Personajes"
    [Table("Personajes")]
    public class Personaje
    {
        // Id (PK, Autoincremental)
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Nombre (String, requerido, máximo de 50 caracteres)
        [Required]
        [MaxLength(50)]
        public string Nombre { get; set; } = string.Empty;

        // Nivel (Int, rango 1-100)
        [Range(1, 100)]
        public int Nivel { get; set; }

        // FechaCreacion (DateTime, obligatorio)
        [Required]
        public DateTime FechaCreacion { get; set; }

        // Gremio (String, opcional)
        // El "?" indica que permite nulos en la base de datos
        public string? Gremio { get; set; }

        // Rasgos (JSON optimizado para búsquedas - tipo jsonb en Postgres)
        // Se usa JsonDocument como sugiere la guía
        [Column(TypeName = "jsonb")]
        public JsonDocument? Rasgos { get; set; }
    }
}