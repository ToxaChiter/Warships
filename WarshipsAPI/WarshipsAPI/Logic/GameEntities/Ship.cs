namespace WarshipsAPI.Logic.GameEntities;

public class Ship
{
    public List<Cell> Coordinates { get; private set; }

    private bool isSunk = false;
    public bool IsSunk
    {
        get
        {
            if (isSunk) return true;

            isSunk = Coordinates.All(cell => cell.State is CellState.Hit);
            return isSunk;
        }
    }

    public Ship(List<Cell> coordinates)
    {
        Coordinates = coordinates;
    }
}

