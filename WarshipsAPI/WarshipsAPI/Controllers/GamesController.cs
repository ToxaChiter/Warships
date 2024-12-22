using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WarshipsAPI.Data.Models;
using WarshipsAPI.Logic.Interfaces;

namespace WarshipsAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _gamesService;
    private readonly IHubContext<GameHub> _hubContext;

    public GamesController(IGameService gamesService, IHubContext<GameHub> hubContext)
    {
        _gamesService = gamesService;
        _hubContext = hubContext;
    }

    [HttpPost]
    public async Task<IActionResult> CreateGame([FromBody] CreateGameRequest request)
    {
        var result = await _gamesService.CreateGameAsync(request);
        if (!result.Success)
            return BadRequest(result.Errors);

        // Notify players via SignalR
        await _hubContext.Clients.Group(result.GameId).SendAsync("GameStarted", result);

        return Ok(result);
    }

    [HttpGet("{gameId}")]
    public async Task<IActionResult> GetGame(string gameId)
    {
        var game = await _gamesService.GetGameByIdAsync(gameId);
        if (game == null)
            return NotFound();

        return Ok(game);
    }

    [HttpPost("{gameId}/move")]
    public async Task<IActionResult> MakeMove(string gameId, [FromBody] MoveRequest request)
    {
        var result = await _gamesService.MakeMoveAsync(gameId, request);
        if (!result.Success)
            return BadRequest(result.Errors);

        // Notify players via SignalR about the move
        await _hubContext.Clients.Group(gameId).SendAsync("MoveMade", result);

        return Ok(result);
    }
}
