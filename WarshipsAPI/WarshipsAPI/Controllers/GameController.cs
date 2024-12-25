using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WarshipsAPI.Data.Dtos;
using WarshipsAPI.Data.Models;
using WarshipsAPI.Logic.Interfaces;
using WarshipsAPI.SignalR;

namespace WarshipsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameController : ControllerBase
{
    private readonly IGameService _gameService;
    private readonly IMoveService _moveService;
    private readonly IHubContext<GameHub> _hubContext;

    public GameController(IGameService gameService, IMoveService moveService, IHubContext<GameHub> hubContext)
    {
        _gameService = gameService;
        _moveService = moveService;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
    {
        var game = await _gameService.CreateGameAsync(request.Player1Id, request.Player2Id);
        if (game is null)
            return BadRequest("Game not created");

        // Notify players via SignalR
        await _hubContext.Clients.Group(game.Id.ToString()).SendAsync("GameStarted", game);

        return Ok(game.Id);
    }

    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetGame(Guid gameId)
    {
        var game = await _gameService.GetGameByIdAsync(gameId);
        if (game is null)
            return NotFound();

        return Ok(game);
    }

    [HttpPost("{gameId}/join")]
    public async Task<IActionResult> JoinGame(Guid gameId, Guid playerId)
    {
        await _gameService.JoinGameAsync(gameId, playerId);
        return Ok();
    }

    [HttpPost("{gameId}/move")]
    public async Task<IActionResult> MakeMove(Guid gameId, Guid playerId, [FromBody] MoveRequest request)
    {
        var result = await _moveService.ProcessMoveAsync(gameId, playerId, request.Move);
        if (result is null)
            return BadRequest("Move not processed");

        // Notify players via SignalR about the move
        await _hubContext.Clients.Group(gameId.ToString()).SendAsync("MoveMade", result);

        return Ok(result);
    }
}
