using WarshipsAPI.Data.Dtos;
using WarshipsAPI.Data.Models;

namespace WarshipsAPI.Logic.Interfaces;

public interface IUserService
{
    Task<User?> RegisterAsync(string username, string password);
    Task<User?> LoginAsync(string username, string password);
    Task<User?> GetUserByIdAsync(Guid userId);
    Task<UserStatsDto> GetUserStatisticsAsync(Guid userId);
}

