using GestorHeroes.Models;
using System.ComponentModel.DataAnnotations.Schema;

/// <summary>
/// Define una tabla creada a partir de la tabla 
/// base Personajes
/// Autor: <Pablo Rubio Prado>
/// </summary>

[Table("Magos")]
public class Mago : Personaje
{
    public int Mana { get; set; }
    public string ElementoPrincipal { get; set; } = string.Empty;
}