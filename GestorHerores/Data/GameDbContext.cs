using GestorHeroes.Models;
using Microsoft.EntityFrameworkCore;

namespace DandDSoft.Infrastructure.Data;

/// <summary>
/// DbContext principal del sistema gestor de héroes (logitrackdb)
/// Autor: <Pablo Rubio Prado>
/// </summary>
public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }

    public DbSet<Personaje> Personajes => Set<Personaje>();
    public DbSet<Guerrero> Guerreros => Set<Guerrero>();
    public DbSet<Mago> Magos => Set<Mago>();
    public DbSet<Arquero> Arqueros => Set<Arquero>();
    public DbSet<Clerigo> Clerigos => Set<Clerigo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Schema por defecto para TODO el modelo
        modelBuilder.HasDefaultSchema("gestorHeroes");

        base.OnModelCreating(modelBuilder);
    }
}