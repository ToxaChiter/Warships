using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Warships.Views;
/// <summary>
/// Логика взаимодействия для TestView.xaml
/// </summary>
public partial class TestView : Page
{
    public TestView()
    {
        InitializeComponent();
    }

    private void ExitButton_Click(object sender, RoutedEventArgs e)
    {
        App.Current.Shutdown();
    }

    private void GameButton_Click(object sender, RoutedEventArgs e)
    {
        App.MainWindow.ChangePage(new Uri("Views/GameView.xaml", UriKind.Relative));
    }
}
