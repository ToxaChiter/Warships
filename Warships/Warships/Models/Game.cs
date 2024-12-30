using Warships.Logic.GameEntities;

namespace Warships.Models;

public enum GameState
{
    Waiting,    // Ожидание второго игрока
    Initializing, // Расстановка кораблей
    InProgress, // Игра идёт
    Finished    // Игра завершена
}

public class Game
{
    public Guid Id { get; set; }
    public Guid Player1Id { get; set; }
    public Guid Player2Id { get; set; }
    public User Player1 { get; set; } = null!;
    public User Player2 { get; set; } = null!;

    public Board Player1Board { get; set; }
    public Board Player2Board { get; set; }

    public GameState State { get; set; } = GameState.Waiting;
    public Guid CurrentPlayerId { get; set; }
    public DateTime StartedAt { get; set; } = DateTime.UtcNow;
    public DateTime? FinishedAt { get; set; }

    public ICollection<Move> Moves { get; set; } = new List<Move>();

    // Статистика матча
    public int TotalMoves { get; set; } // Общее количество ходов
    public TimeSpan TotalDuration => (FinishedAt ?? DateTime.UtcNow) - StartedAt; // Длительность матча
    public Guid? WinnerId { get; set; } // Победитель
}


