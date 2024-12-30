using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Warships.Logic.GameEntities;

namespace Warships.Pages;

public class CellStateToIsEnabledConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var state = value as CellState?;
        bool isEnabled = state switch
        {
            null => true,
            CellState.Hit => false,
            CellState.Miss => false,
            _ => true,
        };

        return isEnabled;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
