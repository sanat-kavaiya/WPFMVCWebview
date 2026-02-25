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
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var username = txtUsername.Text;
            var password = txtPassword.Password;
            if (string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Enter username");
                return;
            }

            if (string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Enter password");
                return;
            }

            if (AuthService.Login(username, password))
            {
                SessionManager.SaveSession(username);

                HomeWindow home = new HomeWindow();
                home.Show();

                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid login");
            }
        }

        private void Signup_Click(object sender, RoutedEventArgs e)
        {
            SignupWindow signup = new SignupWindow();
            signup.Show();
            this.Close();
        }
    }
}
