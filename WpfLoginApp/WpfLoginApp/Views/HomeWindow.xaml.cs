using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfLoginApp.Views
{
    public partial class HomeWindow : Window
    {
        private List<MenuItemData> _menuItems = new List<MenuItemData>();
        private StackPanel _menuPanel;
        private bool _isSitesPageActive = false;
        public HomeWindow()
        {
            InitializeComponent();
            Loaded += Dashboard_Click;
            _menuPanel = SidebarPanel; // Using the x:Name from XAML
            MainFrame.Navigated += MainFrame_Navigated;
        }

        private void MainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            // Check if current page is SitesPage
            _isSitesPageActive = e.Content is SitesPage;

            if (!_isSitesPageActive)
            {
                // Remove all dynamic menu items when not on Sites page
                RemoveDynamicMenuItems();
            }
        }

        private void RemoveDynamicMenuItems()
        {
            if (_menuPanel == null) return;

            // Remove all dynamically added menu items (keep Dashboard, Sites, and Logout)
            for (int i = _menuPanel.Children.Count - 1; i >= 0; i--)
            {
                var child = _menuPanel.Children[i];
                if (child is Button btn)
                {
                    string content = btn.Content.ToString();
                    if (content != "📊 Dashboard" &&
                        content != "🌐 Sites" &&
                        content != "🚪 Logout" &&
                        content != "🕸️ Legacy Sites")
                    {
                        _menuPanel.Children.RemoveAt(i);
                    }
                }
            }
        }
        private void Dashboard_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new DashboardPage());
        }

        private async void Sites_Click(object sender, RoutedEventArgs e)
        {
            var sitesPage = new SitesPage();
            sitesPage.MenuItemsExtracted += OnMenuItemsExtracted;
            MainFrame.Navigate(sitesPage);
        }

        private void OnMenuItemsExtracted(object sender, List<MenuItemData> menuItems)
        {
            _menuItems = menuItems;

            // Dispatch to UI thread
            Dispatcher.Invoke(() =>
            {
                RenderMenuItems();
            });
        }

        private void RenderMenuItems()
        {
            if (_menuPanel == null) return;

            // Only render if we're on Sites page
            if (!_isSitesPageActive) return;

            // Clear existing dynamic menu items first
            RemoveDynamicMenuItems();

            // Add new menu items from MVC
            int insertIndex = 2; // After Dashboard (index 0) and Sites (index 1)

            foreach (var item in _menuItems)
            {
                var button = new Button
                {
                    Content = $"{(string.IsNullOrEmpty(item.Icon) ? "🔗" : GetIconFromClass(item.Icon))} {item.Title}",
                    Style = (Style)FindResource("SidebarButtonStyle"),
                    Tag = item.Url,
                    FontWeight = item.IsMainMenu ? FontWeights.SemiBold : FontWeights.Normal,
                    Margin = new Thickness(item.IsMainMenu ? 0 : 20, 0, 0, 2)
                };

                button.Click += MvcMenuItem_Click;
                _menuPanel.Children.Insert(insertIndex++, button);
            }
        }

        private string GetIconFromClass(string iconClass)
        {
            // Map Font Awesome classes to emoji or symbols
            if (iconClass.Contains("fa-home")) return "🏠";
            if (iconClass.Contains("fa-chart-line")) return "📊";
            if (iconClass.Contains("fa-file-alt")) return "📄";
            if (iconClass.Contains("fa-file")) return "📁";
            if (iconClass.Contains("fa-shield-alt")) return "🛡️";
            if (iconClass.Contains("fa-user")) return "👤";
            return "🔗";
        }

        private void MvcMenuItem_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var url = button?.Tag as string;

            // Navigate the WebView in SitesPage to this URL
            if (MainFrame.Content is SitesPage sitesPage)
            {
                sitesPage.NavigateToUrl(url);
            }
        }

        private async void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Are you sure you want to logout?",
                "Confirm",
                MessageBoxButton.YesNo);

            if (result == MessageBoxResult.Yes)
            {
                if (MainFrame.Content is SitesPage sitesPage)
                {
                    await sitesPage.ClearCookiesAsync();
                }

                // Assuming SessionManager exists
                // SessionManager.ClearSession();

                new LoginWindow().Show();
                this.Close();
            }
        }

        private void LegacySites_Click(object sender, RoutedEventArgs e)
        {
            // Create a new SitesPage but don't subscribe to menu extraction
            var sitesPage = new LegacySitesPage();
            MainFrame.Navigate(sitesPage);
        }
    }
}