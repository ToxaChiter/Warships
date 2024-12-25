﻿using Microsoft.EntityFrameworkCore;
using WarshipsAPI.Data.Database;
using WarshipsAPI.Data.Dtos;
using WarshipsAPI.Data.Models;
using WarshipsAPI.Logic.GameEntities;
using WarshipsAPI.Logic.Interfaces;
using WarshipsAPI.SignalR;

namespace WarshipsAPI.Logic.Services;

public class GameService : IGameService
{
    private readonly WarshipDbContext _dbContext;
    private readonly GameHub _gameHub;

    public GameService(WarshipDbContext dbContext, GameHub gameHub)
    {
        _dbContext = dbContext;
        _gameHub = gameHub;
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

    public async void InitializeBoardAsync(Guid gameId, Guid playerId, List<(int x1, int y1, int x2, int y2)> ships)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game is null) throw new KeyNotFoundException("Game not found");

        Board board;
        if (playerId == game.Player1Id)
        {
            board = game.Player1Board;
        }
        else if (playerId == game.Player2Id) 
        { 
            board = game.Player2Board; 
        }
        else
        {
            throw new KeyNotFoundException("Unknown player in match");
        }

        board.Initialize(ships);
    }

    public async Task<Game?> GetGameByIdAsync(Guid gameId)
    {
        return await _dbContext.Games
            .Include(g => g.Player1)
            .Include(g => g.Player2)
            .Include(g => g.Moves)
            .FirstOrDefaultAsync(g => g.Id == gameId);
    }

    public async Task<bool> JoinGameAsync(Guid gameId, Guid playerId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game is null) throw new KeyNotFoundException("Game not found");

        if (game.Player2Id != Guid.Empty) return false;

        game.Player2Id = playerId;
        game.State = GameState.Initializing;
        await _dbContext.SaveChangesAsync();

        return true;
    }

    public async Task<bool> CheckGameOverAsync(Guid gameId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game is null) throw new KeyNotFoundException("Game not found");

        // Пример логики завершения
        var player1ShipsDestroyed = game.Player2Board.AreAllShipsSunk;
        var player2ShipsDestroyed = game.Player1Board.AreAllShipsSunk;

        return player1ShipsDestroyed || player2ShipsDestroyed;
    }

    public async Task EndGameAsync(Guid gameId, Guid winnerId)
    {
        var game = await GetGameByIdAsync(gameId);
        if (game is null) throw new KeyNotFoundException("Game not found");

        game.FinishedAt = DateTime.UtcNow;
        game.WinnerId = winnerId;
        game.State = GameState.Finished;

        await _dbContext.SaveChangesAsync();

        await _gameHub.NotifyGameEnded(gameId, winnerId);
    }
}

