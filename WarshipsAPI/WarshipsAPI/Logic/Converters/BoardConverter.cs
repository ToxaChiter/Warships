using System.Text;
using WarshipsAPI.Logic.GameEntities;

namespace WarshipsAPI.Logic.Converters;

public class BoardConverter
{
    public static string BoardToString(Board board, bool areShipsHidden = true)
    {
        var builder = new StringBuilder();
        builder.AppendLine(board.Size.ToString());

        for (int y = 0; y < board.Size; y++)
        {
            for (int x = 0; x < board.Size; x++)
            {
                var cell = board[x, y];
                char symbol = (cell.State, areShipsHidden) switch
                {
                    (CellState.Empty, _) => '.',
                    (CellState.Occupied, true) => '.',
                    (CellState.Occupied, false) => 'O',
                    (CellState.Hit, _) => 'X',
                    (CellState.Miss, _) => '~',
                    _ => throw new InvalidOperationException("Unknown cell state")
                };
                builder.Append(symbol);
            }
            builder.AppendLine(); // Переход на новую строку
        }

        return builder.ToString().Trim(); // Убираем лишний перевод строки в конце
    }

    public static Board StringToBoard(string boardString)
    {
        var lines = boardString.Split('\n');
        var size = int.Parse(lines[0]);
        var board = new Board { Size = size };

        if (lines.Length != size)
        {
            throw new InvalidOperationException("Invalid board size.");
        }

        for (int y = 1; y < size; y++)
        {
            if (lines[y].Length != size)
            {
                throw new InvalidOperationException($"Invalid line length at row {y}.");
            }

            for (int x = 0; x < size; x++)
            {
                var cell = board[x, y];
                cell.State = lines[y][x] switch
                {
                    '.' => CellState.Empty,
                    'O' => CellState.Occupied,
                    'X' => CellState.Hit,
                    '~' => CellState.Miss,
                    _ => throw new InvalidOperationException($"Unknown symbol '{lines[y][x]}' at ({x}, {y})")
                };
            }
        }

        return board;
    }
}
