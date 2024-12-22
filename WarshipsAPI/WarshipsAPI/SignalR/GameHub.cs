using Microsoft.AspNetCore.SignalR;
using System.Text.RegularExpressions;

namespace WarshipsAPI.SignalR;

public class GameHub : Hub
{
    // Подключение к игре
    public async Task JoinGame(string gameId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, gameId);
    }

    // Передача хода
    public async Task SendMove(string gameId, string coordinate)
    {
        // Рассылаем всем в группе (игрокам матча)
        await Clients.Group(gameId).SendAsync("ReceiveMove", coordinate);
    }

    // Уведомление о завершении
    public async Task NotifyGameEnd(string gameId, string winner)
    {
        await Clients.Group(gameId).SendAsync("GameEnded", winner);
    }
}
