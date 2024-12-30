namespace Warships.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Навигационные свойства
    public ICollection<Game> Games { get; set; } = new List<Game>();

    // Статистика
    public int GamesPlayed { get; set; } // Всего игр
    public int Wins { get; set; } // Побед
    public int Losses { get; set; } // Поражений
    public double HitAccuracy { get; set; } // Точность попаданий
    public TimeSpan AverageMoveTime { get; set; } // Среднее время на ход
}
