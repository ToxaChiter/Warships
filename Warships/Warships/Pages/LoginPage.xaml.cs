using System;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace Warships.Pages;

/// <summary>
/// Логика взаимодействия для LoginPage.xaml
/// </summary>
public partial class LoginPage : Page
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private async void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        string login = LoginBox.Text;
        string password = PasswordBox.Password;

        Keyboard.ClearFocus();

        var client = App.ApiClient;
        try
        {
            App.PlayerId = await client.PostAsync<Guid>($"User/login", new { Email = login, Password = password });
            App.MainWindow.ChangePage(new Uri("Views/TestView.xaml", UriKind.Relative));
        }
        catch (Exception)
        {
            MessageBox.Show("Invalid login or password", "Login failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }


        //App.MainWindow.ChangePage(new Uri("Pages/MenuPage.xaml", UriKind.Relative));
    }

    private void Login_TextChanged(object sender, EventArgs e)
    {
        bool isBad = string.IsNullOrWhiteSpace(PasswordBox.Password) || PasswordBox.Password.Length < 8 ||
            string.IsNullOrWhiteSpace(LoginBox.Text) || LoginBox.Text.Length < 3;

        if (LoginButton.IsEnabled == isBad) 
        { 
            LoginButton.IsEnabled = !isBad; 
        }
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(passwordBox.Password?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        Login_TextChanged(sender, e);
    }

    private void RegistrationButton_Click(object sender, RoutedEventArgs e)
    {
        App.MainWindow.ChangePage(new Uri("Pages/RegistrationPage.xaml", UriKind.Relative));
    }
}
