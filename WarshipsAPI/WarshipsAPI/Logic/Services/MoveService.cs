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

    public async Task<MoveResultDto> ProcessMoveAsync(Guid gameId, Guid playerId, string coordinate)
    {
        var game = GameService.Games.Find(game => game.Id == gameId);
        if (game == null || game.State != GameState.InProgress)
        {
            throw new InvalidOperationException("Invalid game state.");
        }

        if (game.CurrentPlayerId != playerId) throw new ArgumentException("Not that player`s turn");

        // Проверка попадания
        Board board = game.CurrentPlayerId == game.Player1Id ? game.Player2Board : game.Player1Board;
        var (x, y) = GetIndexFromCoordinate(coordinate);
        bool isHit = board[x, y].State is CellState.Occupied;
        bool isSunk = false;
        bool isGameOver = false;

        // Обновление состояния поля
        board = await UpdateBoardAsync(board, coordinate);
        if (game.CurrentPlayerId == game.Player1Id)
        {
            game.Player2Board = board;
        }
        else
        {
            game.Player1Board = board;
        }

        if (isHit)
        {
            var ship = board.GetShip(x, y);
            if (ship is null) throw new Exception("Ship cannot be found");
            isSunk = ship.IsSunk;
        }
        if (isSunk)
        {
            isGameOver = await _gameService.CheckGameOverAsync(gameId);
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

        //await _gameHub.MakeMove(gameId, x, y);

        if (!isHit)
        {
            game.CurrentPlayerId = (playerId == game.Player1Id ? game.Player2Id : game.Player1Id).Value;
        }

        if (isGameOver)
        {
            await _gameService.EndGameAsync(gameId, playerId);
        }

        // Возврат результата хода
        return new MoveResultDto
        {
            IsHit = isHit,
            IsSunk = isSunk,
            IsGameOver = await _gameService.CheckGameOverAsync(gameId)
        };
    }

    private async Task<Board> UpdateBoardAsync(Board board, string coordinate)
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

        if (newState is CellState.Miss)
        {
            await _dbContext.SaveChangesAsync();
            return board;
        }

        var ship = board.GetShip(x, y);
        if (ship is null) throw new Exception("Ship cannot be found");

        if (!ship.IsSunk)
        {
            await _dbContext.SaveChangesAsync();
            return board;
        }

        List<(int X, int Y)> offsets =
        [
            (-1, -1), (-1, 0), (-1, 1),
            (0, -1), (0, 0), (0, 1),
            (1, -1), (1, 0), (1, 1),
        ];

        foreach (var coord in ship.Coordinates)
        {
            var point = board.GetCoords(coord);
            if (point == (-1, -1)) throw new Exception("Cell cannot be found");

            foreach (var offset in offsets)
            {
                var (X, Y) = (point.X + offset.X, point.Y + offset.Y);
                if (X < 0 || X >= board.Size || Y < 0 || Y >= board.Size)
                {
                    continue;
                }

                var square = board[X, Y];
                if (square.State is CellState.Empty)
                {
                    square.State = CellState.Miss;
                }
            }
        }

        await _dbContext.SaveChangesAsync();
        return board;
    }

    private static (int X, int Y) GetIndexFromCoordinate(string coordinate)
    {
        //// Преобразование координаты (например, "A1") в индекс строки
        //char letter = coordinate[0];
        //var digit = coordinate[1..];

        //return (letter - 'A', int.Parse(digit));

        var dimensions = coordinate.Split(',');
        return (int.Parse(dimensions[0]), int.Parse(dimensions[1]));
    }
}

