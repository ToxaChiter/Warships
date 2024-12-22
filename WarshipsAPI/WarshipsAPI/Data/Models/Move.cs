namespace WarshipsAPI.Data.Models;

public class Move
{
    public Guid Id { get; set; }
    public Guid GameId { get; set; }
    public Game Game { get; set; } = null!;
    public Guid PlayerId { get; set; }
    public User Player { get; set; } = null!;
    public string Coordinate { get; set; } = string.Empty;
    public bool IsHit { get; set; }
    public DateTime MadeAt { get; set; } = DateTime.UtcNow;

    // Новое поле для времени на ход
    public TimeSpan TimeTaken { get; set; } // Время, потраченное на этот ход
}

