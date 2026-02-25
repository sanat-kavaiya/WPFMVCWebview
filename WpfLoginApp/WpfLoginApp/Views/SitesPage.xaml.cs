using Microsoft.Web.WebView2.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace WpfLoginApp.Views
{
    public partial class SitesPage : Page
    {
        // Event to pass menu items to HomeWindow
        public event EventHandler<List<MenuItemData>> MenuItemsExtracted;

        public SitesPage()
        {
            InitializeComponent();
            Loaded += SitesPage_Loaded;
        }

        private async void SitesPage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create a persistent user data folder
                string userDataFolder = System.IO.Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "WpfLoginApp",
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

                // Set up the NavigationCompleted handler
                WebView.CoreWebView2.NavigationCompleted += async (s, args) =>
                {
                    // Only proceed if navigation was successful
                    if (args.IsSuccess)
                    {
                        // Hide the sidebar using CSS injection
                        string hideSidebarScript = @"
                            // Hide the sidebar
                            var sidebar = document.querySelector('.sidebar');
                            if (sidebar) {
                                sidebar.style.display = 'none';
                            }
                            
                            // Adjust main content to full width
                            var mainContent = document.querySelector('.main-content');
                            if (mainContent) {
                                mainContent.style.marginLeft = '0';
                                mainContent.style.width = '100%';
                            }
                            
                            // Hide menu toggle button
                            var menuToggle = document.getElementById('menuToggle');
                            if (menuToggle) {
                                menuToggle.style.display = 'none';
                            }
                        ";

                        await WebView.CoreWebView2.ExecuteScriptAsync(hideSidebarScript);

                        // Extract menu items
                        await ExtractMenuItems();
                    }
                };
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error initializing WebView: {ex.Message}");
            }
        }

        private async Task ExtractMenuItems()
        {
            try
            {
                string script = @"
            (function() {
                var menuItems = [];
                
                // Find all menu items
                var menuLinks = document.querySelectorAll('.menu-item');
                
                menuLinks.forEach(function(link) {
                    var icon = link.querySelector('i');
                    var iconClass = icon ? icon.className : 'fas fa-link';
                    var text = link.textContent.trim();
                    var href = link.getAttribute('href');
                    
                    // Determine if it's a main menu or submenu based on parent
                    var isMainMenu = false;
                    var parentLabel = link.closest('.sidebar-menu')?.previousElementSibling;
                    if (parentLabel && parentLabel.classList.contains('menu-label')) {
                        isMainMenu = parentLabel.textContent.trim() === 'MAIN';
                    }
                    
                    menuItems.push({
                        Title: text,
                        Icon: iconClass,
                        Url: href,
                        IsMainMenu: isMainMenu
                    });
                });
                
                // Return the array directly - WebView2 will handle JSON serialization
                return menuItems;
            })();
        ";

                string result = await WebView.CoreWebView2.ExecuteScriptAsync(script);

                if (!string.IsNullOrEmpty(result) && result != "null" && result != "[]")
                {
                    // The result is already a JSON string, but might have extra escaping
                    // First, clean up the result if it has extra quotes
                    string cleanResult = result.Trim('"').Replace("\\\"", "\"");

                    var options = new System.Text.Json.JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var menuItems = System.Text.Json.JsonSerializer.Deserialize<List<MenuItemData>>(cleanResult, options);
                    MenuItemsExtracted?.Invoke(this, menuItems);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error extracting menu items: {ex.Message}");
            }
        }

        public void NavigateToUrl(string url)
        {
            if (WebView?.CoreWebView2 != null)
            {
                // Ensure we're navigating within the same domain
                if (!url.StartsWith("http"))
                {
                    url = "https://localhost:7247" + url;
                }
                WebView.CoreWebView2.Navigate(url);
            }
        }

        public async Task ClearCookiesAsync()
        {
            if (WebView?.CoreWebView2 != null)
            {
                var cookieManager = WebView.CoreWebView2.CookieManager;
                var cookies = await cookieManager.GetCookiesAsync("https://localhost:7247");

                foreach (var cookie in cookies)
                {
                    cookieManager.DeleteCookie(cookie);
                }
            }
        }
    }

    // Menu item data class
    public class MenuItemData
    {
        [System.Text.Json.Serialization.JsonPropertyName("Title")]
        public string Title { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("Icon")]
        public string Icon { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("Url")]
        public string Url { get; set; }

        [System.Text.Json.Serialization.JsonPropertyName("IsMainMenu")]
        public bool IsMainMenu { get; set; }
    }
}