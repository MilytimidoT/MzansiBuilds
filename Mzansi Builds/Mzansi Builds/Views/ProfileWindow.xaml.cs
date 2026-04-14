using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Mzansi_Builds.Models;
using Mzansi_Builds.Services;

namespace Mzansi_Builds.Views
{
    public partial class ProfileWindow : Window
    {
        private User _user;

        public ProfileWindow(User user)
        {
            InitializeComponent();
            _user = user;

            // Populate user info
            UserEmailText.Text = _user.Email;
            NameText.Text = _user.Name;
            EmailText.Text = _user.Email;
            CityText.Text = _user.City;

            LoadPosts();
        }

        private void LoadPosts()
        {
            var posts = DataService.Instance
                .GetAllPosts()
                .Where(p => p.AuthorEmail != null &&
                            p.AuthorEmail.Equals(_user.Email, StringComparison.OrdinalIgnoreCase))
                .ToList();

            // 🔥 fallback (ensures UI is never empty while testing)
            if (!posts.Any())
            {
                posts = DataService.Instance.GetAllPosts().Take(3).ToList();
            }

            UserPosts.ItemsSource = posts;
        }

        // 🔗 GitHub click
        private void GithubLink_Click(object sender, MouseButtonEventArgs e)
        {
            var element = sender as FrameworkElement;
            var post = element?.DataContext as Post;

            if (post == null || string.IsNullOrWhiteSpace(post.GithubLink))
            {
                MessageBox.Show("No GitHub link available.");
                return;
            }

            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = post.GithubLink,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("Invalid GitHub link.");
            }
        }

        // 🔁 Navigation
        private void OpenFeed_Click(object sender, MouseButtonEventArgs e)
        {
            new FeedWindow(_user).Show();
            Close();
        }

        private void OpenNotifications(object sender, MouseButtonEventArgs e)
        {
            new NotificationWindow(_user).Show();
            Close();
        }

        private void Logout_Click(object sender, MouseButtonEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }

        private void OpenProfile_Click(object sender, MouseButtonEventArgs e)
        {
            // already here
        }

    }
}