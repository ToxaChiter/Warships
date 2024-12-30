using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System.Windows.Media;
using Warships.Models;

namespace Warships.Logic.GameEntities;


public enum CellState
{
    Empty,
    Miss,
    Hit,
    Occupied,
}

//public class Cell
//{
//    public CellState State { get; set; } // Enum: Empty, Miss, Hit, Occupied
//    public SolidColorBrush Brush { get; set; }
//    public ICommand CellCommand { get; set; }
//}

public class Cell : INotifyPropertyChanged
{
    private CellState _state;
    private SolidColorBrush _brush;
    private ICommand _cellCommand;

    public event PropertyChangedEventHandler PropertyChanged;

    public CellState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;

                Brush = _state switch
                {
                    CellState.Occupied => Brushes.LightGreen,
                    CellState.Miss => Brushes.DarkBlue,
                    CellState.Hit => Brushes.Red,
                    CellState.Empty or _ => Brushes.Transparent,
                };

                OnPropertyChanged();
            }
        }
    }

    public SolidColorBrush Brush
    {
        get => _brush;
        set
        {
            if (_brush != value)
            {
                _brush = value;
                OnPropertyChanged();
            }
        }
    }

    public ICommand CellCommand
    {
        get => _cellCommand;
        set
        {
            if (_cellCommand != value)
            {
                _cellCommand = value;
                OnPropertyChanged();
            }
        }
    }

    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
