using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace WpfLoginApp.Views
{
    /// <summary>
    /// Interaction logic for LegacySitesPage.xaml
    /// </summary>
    public partial class LegacySitesPage : Page
    {
        public LegacySitesPage()
        {
            InitializeComponent();
            Loaded += LegacySitesPage_Loaded;
        }

        private async void LegacySitesPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {// Create a persistent user data folder
                string userDataFolder = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WpfLoginApp2",
                    "WebView2Data"
                );

                // Check folder age (optional cleanup)
                var dir = new DirectoryInfo(userDataFolder);
                if (dir.Exists && DateTime.Now - dir.LastWriteTime > TimeSpan.FromDays(30))
                {
                    Directory.Delete(userDataFolder, true);
                }
                // Ensure directory exists
                Directory.CreateDirectory(userDataFolder);

                // Create WebView2 environment with persistent storage
                var environment = await CoreWebView2Environment.CreateAsync(
                    userDataFolder: userDataFolder
                );

                // Initialize WebView with this environment
                await WebView.EnsureCoreWebView2Async(environment);

                // Navigate to your MVC site
                WebView.CoreWebView2.Navigate("https://localhost:7247/");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView: {ex.Message}");
            }
        }
    }
}
