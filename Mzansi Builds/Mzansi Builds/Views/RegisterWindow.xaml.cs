using System.Windows;
using Mzansi_Builds.Models;
using Mzansi_Builds.Services;
using System;
using System.Linq;
using System.Windows;

namespace Mzansi_Builds.Views
{
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            var name = NameBox.Text?.Trim();
            var email = EmailBox.Text?.Trim();
            var city = CityBox.Text?.Trim();
            var password = PasswordBox.Password;
            var confirm = ConfirmPasswordBox.Password;

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Name, email and password are required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!email.Contains("@"))
            {
                MessageBox.Show("Please enter a valid email address.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (password != confirm)
            {
                MessageBox.Show("Passwords do not match.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (DataService.Instance.GetAllUsers().Any(u => string.Equals(u.Email?.Trim(), email, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("A user with that email already exists.", "Registration failed", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var user = new User
            {
                Name = name,
                Email = email,
                City = city,
                Password = password
            };

            DataService.Instance.TryAddUser(user, out var error);

            MessageBox.Show("Registered successfully. Please sign in.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void GoToLogin_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }
    }
}