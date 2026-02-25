using System.Configuration;
using System.Data;
using System.Windows;
using WpfLoginApp.Helpers;
using WpfLoginApp.Views;

namespace WpfLoginApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            if (SessionManager.IsLoggedIn())
            {
                new HomeWindow().Show();
            }
            else
            {
                new LoginWindow().Show();
            }
        }
    }

}
