using System.Collections.ObjectModel;

namespace Warships.Logic.GameEntities;

public class Board
{
    public int Size { get; set; } = 10; // Default size 10x10
    public bool AreAllShipsSunk => Ships.All(ship => ship.IsSunk);

    public ObservableCollection<Cell> Field { get; }

    public Cell[,] Cells { get; private set; }
    private List<Ship> Ships { get; set; }

    public Board()
    {
        Cells = new Cell[Size, Size];
        for (int x = 0; x < Size; x++)
        {
            for (int y = 0; y < Size; y++)
            {
                Cells[x, y] = new Cell { State = CellState.Empty };
            }
        }

        Ships = [];
        Field = new ObservableCollection<Cell>(Cells.Cast<Cell>());
    }

    public void Initialize(List<(int x1, int y1, int x2, int y2)> coords)
    {
        foreach (var (x1, y1, x2, y2) in coords)
        {
            var list = new List<Cell>();
            for (int x = x1; x <= x2; x++)
            {
                for(int y = y1; y <= y2; y++)
                {
                    list.Add(Cells[x, y]);
                }
            }

            Ships.Add(new Ship(list));
        }
    }

    public Cell this[int x, int y]
    {
        get 
        { 
            return Cells[x, y];
        }
        set
        {
            Cells[x, y] = value;
        }
    }

    public Ship GetShip(int x, int y)
    {
        var cell = Cells[x, y];

        foreach (var ship in Ships)
        {
            if (ship.Coordinates.Contains(cell)) return ship;
        }

        return default;
    }

    public (int X, int Y) GetCoords(Cell cell)
    {
        for (int i = 0; i < Size; i++)
        {
            for (int j = 0; j < Size; j++)
            {
                if (Cells[i, j] == cell) return (i, j);
            }
        }

        return (-1, -1);
    }
}

