using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using System.Windows.Navigation;

using Mzansi_Builds.Models;
using Mzansi_Builds.Services;

namespace Mzansi_Builds.Views
{
    public partial class FeedWindow : Window
    {
        private readonly User _user;
        private List<string> _attachments = new List<string>();

        private const string PlaceholderText = "Share something with the community...";

        public FeedWindow(User user)
        {
            InitializeComponent();

            _user = user;

            // ✅ Navbar + Welcome text
            UserEmailText.Text = _user?.Email;
            WelcomeText.Text = $"Welcome back, {_user?.Name ?? "Developer"} 👋";

            // ✅ Set placeholder manually (WPF doesn't support PlaceholderText)
            ContentTextBox.Text = PlaceholderText;
            ContentTextBox.GotFocus += RemovePlaceholder;
            ContentTextBox.LostFocus += AddPlaceholder;

            Loaded += FeedWindow_Loaded;
        }

        private void FeedWindow_Loaded(object sender, RoutedEventArgs e)
        {
            RefreshPosts();
        }

        // 🔄 Load posts
        private void RefreshPosts()
        {
            var posts = DataService.Instance.GetAllPosts();
            PostsList.ItemsSource = posts;
        }

        // 📷 Attach files
        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Multiselect = true,
                Title = "Select attachments",
                Filter = "Image files (*.png;*.jpg;*.jpeg)|*.png;*.jpg;*.jpeg|All files (*.*)|*.*"
            };

            if (dlg.ShowDialog() == true)
            {
                _attachments = dlg.FileNames.ToList();

                AttachedFilesText.Text = _attachments.Any()
                    ? string.Join(", ", _attachments.Select(f => System.IO.Path.GetFileName(f)))
                    : "No attachments";
            }
        }

        // ❌ Cancel post
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            ContentTextBox.Text = PlaceholderText;
            _attachments.Clear();
            AttachedFilesText.Text = "No attachments";
        }

        // 🚀 Create post
        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            var content = ContentTextBox.Text?.Trim();

            // Remove placeholder from submission
            if (content == PlaceholderText)
                content = string.Empty;

            if (string.IsNullOrWhiteSpace(content) && !_attachments.Any())
            {
                MessageBox.Show("Please add text or attachments.",
                                "Empty post",
                                MessageBoxButton.OK,
                                MessageBoxImage.Warning);
                return;
            }

            var post = new Post
            {
                Content = content,
                GithubLink = null, // ❌ removed textbox → no error
                AttachmentPaths = new List<string>(_attachments),
                AuthorEmail = _user?.Email
            };

            DataService.Instance.AddPost(post);

            // reset UI
            CancelButton_Click(null, null);

            // refresh feed
            RefreshPosts();
        }

        // 🔗 Open GitHub link (still works if present in data)
        private void GithubLink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = e.Uri.AbsoluteUri,
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show("Unable to open link.");
            }

            e.Handled = true;
        }

        // 🚪 Logout
        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        // ✨ Placeholder behavior (WPF workaround)

        private void RemovePlaceholder(object sender, RoutedEventArgs e)
        {
            if (ContentTextBox.Text == PlaceholderText)
            {
                ContentTextBox.Text = "";
                ContentTextBox.Foreground = System.Windows.Media.Brushes.Black;
            }
        }

        private void AddPlaceholder(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ContentTextBox.Text))
            {
                ContentTextBox.Text = PlaceholderText;
                ContentTextBox.Foreground = System.Windows.Media.Brushes.Gray;
            }
        }
        private void OpenFriends_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var friends = new FriendsWindow(_user);
            friends.Show();
            this.Close();
        }
    }
}