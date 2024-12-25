namespace WarshipsAPI.Data.Dtos;

public class CreateGameRequest
{
    public Guid Player1Id { get; set; }
    public Guid Player2Id { get; set; }
}