using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using WarshipsAPI.Data.Database;
using WarshipsAPI.Logic.Converters;
using WarshipsAPI.Logic.Interfaces;
using WarshipsAPI.Logic.Services;

namespace WarshipsAPI.SignalR;

public class TestHub : Hub
{
    private static ConcurrentDictionary<string, string> players = new(); // Игроки: ID клиента => ID игрока
    private static ConcurrentDictionary<string, (GameSession Session, Guid GameId)> sessions = new(); // Игровые сессии: ID сессии => Сессия
    private static ConcurrentQueue<string> lobbySessions = new(); // Игровые сессии: ID сессии

    private IGameService _gameService;
    private IMoveService _moveService;

    private readonly WarshipDbContext _dbContext;

    public TestHub(IGameService gameService, IMoveService moveService, WarshipDbContext dbContext)
    {
        _gameService = gameService;
        _moveService = moveService;
        _dbContext = dbContext;
    }

    // Подключение игрока
    public override async Task OnConnectedAsync()
    {
        string connectionId = Context.ConnectionId;
        await Clients.Caller.SendAsync("Connected", connectionId);
        Console.WriteLine($"Connected: {connectionId}");
    }

    // Отключение игрока
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string connectionId = Context.ConnectionId;
        players.Remove(connectionId, out var _);
        await Clients.All.SendAsync("PlayerDisconnected", connectionId);
        Console.WriteLine($"Disconnected: {connectionId}");
    }

    // Регистрация игрока
    public async Task RegisterPlayer(string playerId)
    {
        string connectionId = Context.ConnectionId;
        players[connectionId] = playerId;
        //await Clients.Caller.SendAsync("PlayerRegistered", playerId);

        Console.WriteLine($"Registered: {playerId}");
    }

    // Создание игровой сессии
    public async Task CreateOrJoinGameSession()
    {
        if (lobbySessions.IsEmpty)
        {
            await CreateGameSession();
        }
        else
        {
            lobbySessions.TryDequeue(out var sessionId);
            await JoinGameSession(sessionId);
        }
    }

    // Создание игровой сессии
    public async Task CreateGameSession()
    {
        string connectionId = Context.ConnectionId;
        var session = new GameSession { HostId = connectionId };
        var sessionId = Guid.NewGuid().ToString();
        sessions[sessionId] = (session, Guid.Empty);
        lobbySessions.Enqueue(sessionId);
        await Clients.Caller.SendAsync("GameSessionCreated", sessionId);
        await Groups.AddToGroupAsync(connectionId, sessionId);
        await Clients.Caller.SendAsync("TurnChanged");
        Console.WriteLine($"Created session: {sessionId}");
    }

    // Присоединение к игровой сессии
    public async Task JoinGameSession(string sessionId)
    {
        string connectionId = Context.ConnectionId;
        if (sessions.TryGetValue(sessionId, out var session))
        {
            session.Session.OpponentId = connectionId;
            await Clients.Caller.SendAsync("GameSessionCreated", sessionId);
            var hostPlayerId = Guid.Parse(players[session.Session.HostId]);
            var opponentPlayerId = Guid.Parse(players[session.Session.OpponentId]);

            await Groups.AddToGroupAsync(connectionId, sessionId);

            var game = await _gameService.CreateGameAsync(hostPlayerId, opponentPlayerId);
            await _gameService.InitializeBoardAsync(game.Id, hostPlayerId, ConstantFields.HostShips);
            await _gameService.InitializeBoardAsync(game.Id, opponentPlayerId, ConstantFields.OpponentShips);

            game.State = Data.Models.GameState.InProgress;
            game.CurrentPlayerId = game.Player1Id.Value;

            await _dbContext.SaveChangesAsync();

            session.GameId = game.Id;
            sessions[sessionId] = session;

            string hostBoard = BoardConverter.BoardToString(game.Player1Board, false);
            string opponentBoard = BoardConverter.BoardToString(game.Player2Board);

            await Clients.Client(session.Session.HostId).SendAsync("GameStarted", hostBoard, opponentBoard);

            hostBoard = BoardConverter.BoardToString(game.Player1Board);
            opponentBoard = BoardConverter.BoardToString(game.Player2Board, false);

            await Clients.Client(session.Session.OpponentId).SendAsync("GameStarted", opponentBoard, hostBoard);
        }
        else
        {
            await Clients.Caller.SendAsync("Error", "Session not found");
        }
    }

    // Отправка хода
    public async Task MakeMove(string sessionId, string playerId, string move)
    {
        if (sessions.TryGetValue(sessionId, out var sessionPair))
        {
            var (session, gameId) = sessionPair;
            var guidPlayerId = Guid.Parse(playerId);

            var moveResult = await _moveService.ProcessMoveAsync(gameId, guidPlayerId, move);
            var game = GameService.Games.Find(game => game.Id == gameId);

            if (!moveResult.IsHit)
            {
                await NotifyTurnChange(sessionId);
            }


            string hostBoard = BoardConverter.BoardToString(game.Player1Board, false);
            string opponentBoard = BoardConverter.BoardToString(game.Player2Board);

            await Clients.Client(session.HostId).SendAsync("MoveMade", hostBoard, opponentBoard);

            hostBoard = BoardConverter.BoardToString(game.Player1Board);
            opponentBoard = BoardConverter.BoardToString(game.Player2Board, false);

            await Clients.Client(session.OpponentId).SendAsync("MoveMade", opponentBoard, hostBoard);
        }
        else
        {
            await Clients.Caller.SendAsync("Error", "Session not found");
        }
    }



    // Уведомление о смене хода
    public async Task NotifyTurnChange(string sessionId)
    {
        await Clients.Group(sessionId).SendAsync("TurnChanged");
    }
}

// Класс для игровой сессии
public class GameSession
{
    public string HostId { get; set; }
    public string? OpponentId { get; set; }
}


public static class ConstantFields
{
    public static readonly List<(int x1, int y1, int x2, int y2)> HostShips =
        [
            (1, 0, 4, 0),

            (0, 2, 0, 4),
            (5, 5, 5, 7),

            (5, 2, 6, 2),
            (1, 7, 1, 8),
            (8, 8, 8, 9),

            (8, 1, 8, 1),
            (2, 4, 2, 4),
            (7, 5, 7, 5),
            (3, 9, 3, 9),
        ];

    public static readonly List<(int x1, int y1, int x2, int y2)> OpponentShips =
        [
            (1, 0, 4, 0),

            (0, 2, 0, 4),
            (5, 5, 5, 7),

            (5, 2, 6, 2),
            (1, 7, 1, 8),
            (8, 8, 8, 9),

            (8, 1, 8, 1),
            (2, 4, 2, 4),
            (7, 5, 7, 5),
            (3, 9, 3, 9),
        ];
}