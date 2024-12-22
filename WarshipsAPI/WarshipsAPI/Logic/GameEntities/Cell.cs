using WarshipsAPI.Data.Models;

namespace WarshipsAPI.Logic.GameEntities;


public enum CellState
{
    Empty,
    Miss,
    Hit,
    Occupied,
}

public class Cell
{
    public CellState State { get; set; } // Enum: Empty, Miss, Hit, Occupied
}
