using Microsoft.EntityFrameworkCore;
using WarshipsAPI.Data.Database;
using WarshipsAPI.Data.Models;
using WarshipsAPI.Logic.Interfaces;

namespace WarshipsAPI.Logic.Services;

public class GameService : IGameService
{
    private readonly WarshipDbContext _dbContext;

    public GameService(WarshipDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Game> CreateGameAsync(Guid player1Id, Guid player2Id)
    {
        var game = new Game
        {
            Id = Guid.NewGuid(),
            Player1Id = player1Id,
            Player2Id = player2Id,
            State = GameState.Waiting,
            StartedAt = DateTime.UtcNow
        };

        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();
        return game;
    }

    public async Task<Game?> GetGameByIdAsync(Guid gameId)
    {
        return await _dbContext.Games
            .Include(g => g.Player1)
            .Include(g => g.Player2)
            .Include(g => g.Moves)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<bool> CheckGameOverAsync(Guid gameId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null) throw new KeyNotFoundException("Game not found");

        // Пример логики завершения
        var player1ShipsDestroyed = CheckAllShipsDestroyed(game.Player2Board);
        var player2ShipsDestroyed = CheckAllShipsDestroyed(game.Player1Board);

        return player1ShipsDestroyed || player2ShipsDestroyed;
    }

    public async Task EndGameAsync(Guid gameId, Guid winnerId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game == null) throw new KeyNotFoundException("Game not found");

        game.FinishedAt = DateTime.UtcNow;
        game.WinnerId = winnerId;
        game.State = GameState.Finished;

        await _dbContext.SaveChangesAsync();
    }

    private bool CheckAllShipsDestroyed(string field)
    {
        // Простая проверка на уничтожение всех кораблей
        return !field.Contains("S"); // 'S' обозначает корабль
    }

    public async Task<MoveResult> MakeMoveAsync(string gameId, MoveRequest request)
    {
        var game = await _gameRepository.GetGameAsync(gameId);
        if (game == null) throw new Exception("Game not found");

        if (game.Turn != request.PlayerId)
            return new MoveResult { Success = false, Errors = new[] { "Not your turn!" } };

        var cell = game.Board.FirstOrDefault(c => c.X == request.X && c.Y == request.Y);
        if (cell == null || cell.State != CellState.Empty)
            return new MoveResult { Success = false, Errors = new[] { "Invalid move!" } };

        if (cell.HasShip)
        {
            cell.State = CellState.Hit;
            // Дополнительная логика потопления корабля
        }
        else
        {
            cell.State = CellState.Miss;
        }

        // Проверка окончания игры
        var isGameOver = CheckGameOver(game);
        if (isGameOver)
        {
            game.Status = GameStatus.Finished;
            game.WinnerId = request.PlayerId;
        }
        else
        {
            // Передача хода
            game.Turn = GetNextPlayerId(game);
        }

        await _gameRepository.UpdateGameAsync(game);

        return new MoveResult { Success = true, UpdatedBoard = game.Board };
    }

}

