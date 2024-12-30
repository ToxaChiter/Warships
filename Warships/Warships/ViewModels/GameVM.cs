using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using System.ComponentModel;
using System.Windows.Media;
using Warships.API.SignalR;
using Warships.Logic.Converters;
using Warships.Logic.GameEntities;

namespace Warships.ViewModels;

public class GameVM : BaseViewModel
{
    public GameHubClient Hub { get; set; }

    public Board PlayerBoard { get; }
    public Board EnemyBoard { get; }

    public bool IsMyTurn { get; set; } = false;

    public List<string> History { get; }

    public GameVM()
    {
        var stringBoard =
            """
            10
            OOO.O.....
            .......OO.
            OOOO......
            ......O...
            .O....O..O
            ......O...
            .OO.......
            .......O..
            .......O..
            ..O.......
            """;

        var playerBoard = BoardConverter.StringToBoard(stringBoard);
        var enemyBoard = BoardConverter.StringToBoard(stringBoard);

        InitBoardCells(playerBoard);
        InitBoardCells(enemyBoard);

        PlayerBoard = playerBoard;
        EnemyBoard = enemyBoard;

        History = [];
        //for (var i = 0; i < 100; i++)
        //{
        //    History.Add("Enemy hit B4 - miss!");
        //}
    }

    public void UpdateBoard(Board board, string boardString)
    {
        var newBoard = BoardConverter.StringToBoard(boardString);
        for (var i = 0; i < board.Size; i++)
        {
            for (var j = 0; j < board.Size; j++)
            {
                board.Cells[i, j].State = newBoard.Cells[i, j].State;
            }
        }
    }

    private void InitBoardCells(Board board)
    {
        foreach (var cell in board.Cells)
        {
            cell.Brush = cell.State switch
            {
                CellState.Occupied => Brushes.LightGreen,
                CellState.Miss => Brushes.DarkBlue,
                CellState.Hit => Brushes.Red,
                CellState.Empty or _ => Brushes.Transparent,
            };

            cell.CellCommand = new RelayCommand(async () =>
            {
                //cell.State = cell.State switch
                //{
                //    CellState.Occupied => CellState.Hit,
                //    CellState.Miss => CellState.Miss,
                //    CellState.Hit => CellState.Hit,
                //    CellState.Empty or _ => CellState.Miss,
                //};

                //cell.Brush = cell.State switch
                //{
                //    CellState.Occupied => Brushes.LightGreen,
                //    CellState.Miss => Brushes.DarkBlue,
                //    CellState.Hit => Brushes.Red,
                //    CellState.Empty or _ => Brushes.Transparent,
                //};

                for (int i = 0; i < EnemyBoard.Size; i++)
                {
                    for (int j = 0; j < EnemyBoard.Size; j++)
                    {
                        if (EnemyBoard.Cells[i, j] == cell)
                        {
                            await Hub.SendMoveAsync($"{j},{i}");
                        }
                    }
                }
            });
        }
    }

    public void ChangeCurrentPlayer()
    {
        IsMyTurn = !IsMyTurn;
        Notify(nameof(IsMyTurn));
    }

    private void OnCellClicked()
    {
        // Обработка клика по клетке
        // parameter содержит данные клетки
    }
}
