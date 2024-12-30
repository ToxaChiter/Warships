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
using Warships.API.SignalR;
using Warships.ViewModels;

namespace Warships.Views;
/// <summary>
/// Логика взаимодействия для GameView.xaml
/// </summary>
public partial class GameView : Page
{
    public GameView()
    {
        InitializeComponent();
        DataContext = new GameVM();

        
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            button.IsEnabled = false;
        }

    }

    private async void Page_Loaded(object sender, RoutedEventArgs e)
    {
        var gameVM = (GameVM)DataContext;
        gameVM.Hub = new GameHubClient("https://localhost:7174/testhub/");
        var hub = gameVM.Hub;
        await hub.ConnectAsync(gameVM);
        await hub.RegisterAsync(App.PlayerId.ToString());
        await hub.CreateOrJoinGameAsync();
    }
}
