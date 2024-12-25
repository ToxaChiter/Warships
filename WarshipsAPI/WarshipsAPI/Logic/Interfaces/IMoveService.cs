using WarshipsAPI.Data.Dtos;

namespace WarshipsAPI.Logic.Interfaces;

public interface IMoveService
{
    Task<MoveResultDto> ProcessMoveAsync(Guid gameId, Guid playerId, string coordinate);
}

