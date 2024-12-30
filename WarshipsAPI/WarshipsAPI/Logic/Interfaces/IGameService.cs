using WarshipsAPI.Data.Models;

namespace WarshipsAPI.Logic.Interfaces;

public interface IGameService
{
    static List<Game> Games { get; }

    Task<Game> CreateGameAsync(Guid player1Id, Guid player2Id);
    Task<Game?> GetGameByIdAsync(Guid gameId);
    Task<bool> JoinGameAsync(Guid gameId, Guid playerId);
    Task<bool> CheckGameOverAsync(Guid gameId);
    Task EndGameAsync(Guid gameId, Guid winnerId);
    Task InitializeBoardAsync(Guid id, Guid hostPlayerId, List<(int x1, int y1, int x2, int y2)> hostShips);
}

