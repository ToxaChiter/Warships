using System.Configuration;
using System.Data;
using System.Windows;
using Warships.API;

namespace Warships
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static ApiClient ApiClient { get; set; } = new ApiClient("https://localhost:7174/api/");
        public static MainWindow MainWindow { get; set; }
        public static Guid PlayerId { get; set; }
    }

}
