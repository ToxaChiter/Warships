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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Warships.Pages;
/// <summary>
/// Логика взаимодействия для RegistrationPage.xaml
/// </summary>
public partial class RegistrationPage : Page
{
    public RegistrationPage()
    {
        InitializeComponent();
    }

    private void Register_TextChanged(object sender, EventArgs e)
    {
        bool isBad = 
            string.IsNullOrWhiteSpace(PasswordBox.Password) || PasswordBox.Password.Length < 8 || 
            !string.Equals(PasswordBox.Password, ConfirmPasswordBox.Password, StringComparison.OrdinalIgnoreCase) ||
            string.IsNullOrWhiteSpace(UsernameBox.Text) || UsernameBox.Text.Length < 3;

        if (RegisterButton.IsEnabled == isBad) RegisterButton.IsEnabled = !isBad;
    }

    private void LoginButton_Click(object sender, RoutedEventArgs e)
    {
        App.MainWindow.ChangePage(new Uri("Pages/LoginPage.xaml", UriKind.Relative));
    }

    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(passwordBox.Password?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        Register_TextChanged(sender, e);
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (sender is PasswordBox passwordBox)
        {
            ConfirmPasswordPlaceholder.Visibility = string.IsNullOrEmpty(passwordBox.Password?.ToString()) ? Visibility.Visible : Visibility.Collapsed;
        }

        Register_TextChanged(sender, e);
    }

    private async void RegisterButton_Click(object sender, RoutedEventArgs e)
    {
        string login = UsernameBox.Text;
        string password = PasswordBox.Password;

        Keyboard.ClearFocus();

        var client = App.ApiClient;
        try
        {
            App.PlayerId = await client.PostAsync<Guid>($"User/register", new { Email = login, Password = password });
            App.MainWindow.ChangePage(new Uri("Pages/LoginPage.xaml", UriKind.Relative));
        }
        catch (Exception)
        {
            MessageBox.Show("Something went wrong", "Registration failed", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
