using Microsoft.EntityFrameworkCore;
using WarshipsAPI.Data.Models;

namespace WarshipsAPI.Data.Database;

public class WarshipDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Game> Games { get; set; } = null!;
    public DbSet<Move> Moves { get; set; } = null!;

    public WarshipDbContext()
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //Настройка связей
        modelBuilder.Entity<Game>()
            .HasOne(g => g.Player1)
            .WithMany()
            .HasForeignKey(g => g.Player1Id);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.Player2)
            .WithMany()
            .HasForeignKey(g => g.Player2Id);

        modelBuilder.Entity<Move>()
            .HasOne(m => m.Game)
            .WithMany(g => g.Moves)
            .HasForeignKey(m => m.GameId);

        modelBuilder.Entity<Move>()
            .HasOne(m => m.Player)
            .WithMany()
            .HasForeignKey(m => m.PlayerId);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Настройка подключения к БД
        optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=warships;Trusted_Connection=True;");
    }
}
