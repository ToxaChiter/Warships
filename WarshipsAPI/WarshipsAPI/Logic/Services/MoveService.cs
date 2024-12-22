using WarshipsAPI.Data.Database;
using WarshipsAPI.Data.Dtos;
using WarshipsAPI.Data.Models;
using WarshipsAPI.Logic.GameEntities;
using WarshipsAPI.Logic.Interfaces;

namespace WarshipsAPI.Logic.Services;

public class MoveService : IMoveService
{
    private readonly WarshipDbContext _dbContext;
    private readonly IGameService _gameService;

    public MoveService(WarshipDbContext dbContext, IGameService gameService)
    {
        _dbContext = dbContext;
        _gameService = gameService;
    }

    public async Task<MoveResultDto> ProcessMoveAsync(Guid gameId, string coordinate)
    {
        var game = await _gameService.GetGameByIdAsync(gameId);
        if (game == null || game.State != GameState.InProgress)
        {
            throw new InvalidOperationException("Invalid game state.");
        }

        // Проверка попадания
        Board board = game.CurrentPlayerId == game.Player1Id ? game.Player2Board : game.Player1Board;
        var (x, y) = GetIndexFromCoordinate(coordinate);
        bool isHit = board[x, y].State is CellState.Occupied;
        bool isSunk = false;
        if (isHit)
        {
            var ship = board.GetShip(x, y);
            if (ship is null) throw new Exception("Ship cannot be found");
            isSunk = ship.IsSunk;
        }

        // Обновление состояния поля
        board = UpdateBoard(board, coordinate, isHit);
        if (game.CurrentPlayerId == game.Player1Id)
        {
            game.Player2Board = board;
        }
        else
        {
            game.Player1Board = board;
        }

        // Сохранение хода
        var move = new Move
        {
            Id = Guid.NewGuid(),
            GameId = gameId,
            PlayerId = game.CurrentPlayerId,
            Coordinate = coordinate,
            IsHit = isHit,
            MadeAt = DateTime.UtcNow
        };

        _dbContext.Moves.Add(move);
        await _dbContext.SaveChangesAsync();

        // Возврат результата хода
        return new MoveResultDto
        {
            IsHit = isHit,
            IsSunk = 
            IsGameOver = await _gameService.CheckGameOverAsync(gameId)
        };
    }

    private void UpdateBoard(Board board, string coordinate, bool isHit)
    {
        var (x, y) = GetIndexFromCoordinate(coordinate);
        var cell = board[x, y];
        var newState = cell.State switch
        {
            CellState.Empty => CellState.Miss,
            CellState.Occupied => CellState.Hit,
            _ => throw new ArgumentException("Invalid cell state on hit")
        };
        cell.State = newState;
    }

    private (int X, int Y) GetIndexFromCoordinate(string coordinate)
    {
        // Преобразование координаты (например, "A1") в индекс строки
        char letter = coordinate[0];
        var digit = coordinate[1..];

        return (letter - 'A', int.Parse(digit));
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

