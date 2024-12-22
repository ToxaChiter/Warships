namespace WarshipsAPI.Data.Dtos;

public class UserStatsDto
{
    // Статистика
    public int GamesPlayed { get; set; } // Всего игр
    public int Wins { get; set; } // Побед
    public int Losses { get; set; } // Поражений
    public double HitAccuracy { get; set; } // Точность попаданий
    public TimeSpan AverageMoveTime { get; set; } // Среднее время на ход
}
