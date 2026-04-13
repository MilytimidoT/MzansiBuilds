using System.Windows;
using Mzansi_Builds.Models;
using Mzansi_Builds.Services;
using System;
using System.Linq;
using System.Windows;

namespace Mzansi_Builds.Views
{
    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            var email = EmailBox.Text?.Trim();
            var password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both email and password.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var user = DataService.Instance.GetAllUsers()
                .FirstOrDefault(u => string.Equals(u.Email?.Trim(), email, StringComparison.OrdinalIgnoreCase)
                                     && u.Password == password);

            if (user != null)
            {
                // Open feed (simple placeholder window)
                var feed = new FeedWindow(user);
                feed.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Invalid email or password.", "Login failed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GoToRegister_Click(object sender, RoutedEventArgs e)
        {
            var register = new RegisterWindow();
            register.Show();
            this.Close();
        }
    }
}