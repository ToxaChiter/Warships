using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WarshipsAPI.Data.Database;
using WarshipsAPI.Data.Dtos;
using WarshipsAPI.Data.Models;
using WarshipsAPI.Logic.Interfaces;

namespace WarshipsAPI.Logic.Services;

public class UserService : IUserService
{
    private readonly WarshipDbContext _dbContext;
    private readonly IPasswordHasher<User> _passwordHasher;

    public UserService(WarshipDbContext dbContext, IPasswordHasher<User> passwordHasher)
    {
        _dbContext = dbContext;
        _passwordHasher = passwordHasher;
    }

    public async Task<User?> RegisterAsync(string username, string password)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = username,
            CreatedAt = DateTime.UtcNow,
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, password);
        var userEntity = _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return userEntity.Entity;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Username == username);
        if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Success)
        {
            return null;
        }
        return user;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _dbContext.Users.FindAsync(userId);
    }

    public async Task<UserStatsDto> GetUserStatisticsAsync(Guid userId)
    {
        var user = await _dbContext.Users.Include(u => u.Games).FirstOrDefaultAsync(u => u.Id == userId);
        if (user == null) throw new KeyNotFoundException("User not found");

        return new UserStatsDto
        {
            GamesPlayed = user.GamesPlayed,
            Wins = user.Wins,
            Losses = user.Losses,
            HitAccuracy = user.HitAccuracy,
            AverageMoveTime = user.AverageMoveTime,
        };
    }
}

