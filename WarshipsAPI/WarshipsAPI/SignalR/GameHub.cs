using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace WarshipsAPI.SignalR;

public class GameHub : Hub
{
    // Метод вызывается при подключении клиента
    public override async Task OnConnectedAsync()
    {
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
        await base.OnConnectedAsync();
    }

    // Метод вызывается при отключении клиента
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
        await base.OnDisconnectedAsync(exception);
    }

    // Присоединение клиента к конкретной игровой группе (комнаты)
    public async Task JoinGame(Guid gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId.ToString());
        await Clients.Group(gameId.ToString()).SendAsync("PlayerJoined", Context.ConnectionId);
    }

    // Отправка хода текущего игрока
    public async Task MakeMove(Guid gameId, int x, int y)
    {
        await Clients.Group(gameId.ToString()).SendAsync("MoveMade", new { PlayerId = Context.ConnectionId, X = x, Y = y });
    }

    // Уведомление о смене хода
    public async Task NotifyTurnChange(Guid gameId, Guid currentPlayerId)
    {
        await Clients.Group(gameId.ToString()).SendAsync("TurnChanged", new { CurrentPlayerId = currentPlayerId });
    }

    // Уведомление об окончании игры
    public async Task NotifyGameEnded(Guid gameId, Guid winnerId)
    {
        await Clients.Group(gameId.ToString()).SendAsync("GameEnded", new { WinnerId = winnerId });
    }
}
