using Microsoft.AspNetCore.SignalR.Client;
using Warships.Logic.Converters;
using Warships.ViewModels;

namespace Warships.API.SignalR;

public class GameHubClient
{
    private readonly HubConnection _connection;
    private GameVM _gameVM;

    public string ConnectionId { get; private set; }
    public string GameSessionId { get; private set; }

    public GameHubClient(string hubUrl)
    {
        _connection = new HubConnectionBuilder()
            .WithUrl(hubUrl)
            .WithAutomaticReconnect()
            .Build();

        _connection.On<string>("Connected", (connectionId) =>
        {
            ConnectionId = connectionId;
        });

        _connection.On<string>("GameSessionCreated", (sessionId) =>
        {
            GameSessionId = sessionId;
        });

        _connection.On("GameStarted", (string myBoard, string opponentBoard) =>
        {
            _gameVM.UpdateBoard(_gameVM.PlayerBoard, myBoard);
            _gameVM.UpdateBoard(_gameVM.EnemyBoard, opponentBoard);
        });

        //_connection.On<string>("PlayerRegistered", (playerName) =>
        //{
        //    Console.WriteLine($"Player registered: {playerName}");
        //});

        _connection.On("MoveMade", (string myBoard, string opponentBoard) =>
        {
            _gameVM.UpdateBoard(_gameVM.PlayerBoard, myBoard);
            _gameVM.UpdateBoard(_gameVM.EnemyBoard, opponentBoard);
        });

        _connection.On("TurnChanged", () =>
        {
            _gameVM.ChangeCurrentPlayer();
        });
    }


    public async Task ConnectAsync(GameVM gameVM)
    {
        _gameVM = gameVM;
        await _connection.StartAsync();
    }

    public async Task RegisterAsync(string playerId) => await _connection.InvokeAsync("RegisterPlayer", playerId);
    public async Task CreateOrJoinGameAsync() => await _connection.InvokeAsync("CreateOrJoinGameSession");
    public async Task SendMoveAsync(string move) => await _connection.InvokeAsync("MakeMove", GameSessionId, App.PlayerId, move);
}

