using WarshipsAPI.Data.Models;

namespace WarshipsAPI.Logic.Interfaces;

public interface IGameService
{
    Task<Game> CreateGameAsync(Guid player1Id, Guid player2Id);
    Task<Game?> GetGameByIdAsync(Guid gameId);
    Task<bool> JoinGameAsync(Guid gameId, Guid playerId);
    Task<bool> CheckGameOverAsync(Guid gameId);
    Task EndGameAsync(Guid gameId, Guid winnerId);
}

