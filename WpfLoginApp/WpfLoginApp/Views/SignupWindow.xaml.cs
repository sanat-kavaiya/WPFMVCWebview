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
using WpfLoginApp.Helpers;
using WpfLoginApp.Services;

namespace WpfLoginApp.Views
{
    /// <summary>
    /// Interaction logic for SignupWindow.xaml
    /// </summary>
    public partial class SignupWindow : Window
    {
        public SignupWindow()
        {
            InitializeComponent();
        }
        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text;
            var password = txtPassword.Password;

            var result = AuthService.Signup(username, password);

            if (result)
            {
                SessionManager.SaveSession(username);

                MessageBox.Show("Signup successful");

                HomeWindow home = new HomeWindow();
                home.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("User already exists");
            }
        }

        private void BackToLogin_Click(object sender, RoutedEventArgs e)
        {
            // Close signup window and show login window
            var loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }
    }
}
