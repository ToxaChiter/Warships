using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Warships
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            App.MainWindow = this;
            ChangePage(new Uri("Pages/LoginPage.xaml", UriKind.Relative));
            //ChangePage(new Uri("Views/TestView.xaml", UriKind.Relative));
        }

        public void ChangePage(Uri source)
        {
            MainFrame.Navigate(source);
        }
    }
}