using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Navigation;
using Warships.API;


namespace Warships.ViewModels;

public class MainVM : BaseViewModel
{
    public string WelcomeMessage { get; private set; }

    public ICommand NavigateToGameCommand { get; }
    public ICommand NavigateToLeaderboardCommand { get; }
    public ICommand NavigateToSettingsCommand { get; }
    public ICommand ExitCommand { get; }

    public MainVM()
    {
        WelcomeMessage = "Добро пожаловать в Морской Бой!";

        //NavigateToGameCommand = new RelayCommand(NavigateToGame);
        //NavigateToLeaderboardCommand = new RelayCommand(NavigateToLeaderboard);
        //NavigateToSettingsCommand = new RelayCommand(NavigateToSettings);
        //ExitCommand = new RelayCommand(ExitApplication);
    }

    //private void NavigateToGame() => _navigationService.NavigateTo("Game");
    //private void NavigateToLeaderboard() => _navigationService.NavigateTo("Leaderboard");
    //private void NavigateToSettings() => _navigationService.NavigateTo("Settings");
    //private void ExitApplication() => System.Windows.Application.Current.Shutdown();
}
