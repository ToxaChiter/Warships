namespace WarshipsAPI.Data.Dtos;

public class MoveResultDto
{
    public bool IsHit { get; set; }
    public bool IsSunk { get; set; }
    public bool IsGameOver { get; set; }
}
