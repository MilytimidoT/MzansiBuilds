using Mzansi_Builds.Models;
using Mzansi_Builds.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Mzansi_Builds.Views
{
    public partial class FeedWindow : Window
    {
        private User _user;
        private List<string> _attachments = new List<string>();

        public FeedWindow(User user)
        {
            InitializeComponent();
            _user = user;

            UserEmailText.Text = _user.Email;
            WelcomeText.Text = $"Welcome, {_user.Name} 👋";

            LoadPosts();
        }

        // ================= LOAD POSTS =================
        private void LoadPosts()
        {
            PostsList.ItemsSource = null;
            PostsList.ItemsSource = DataService.Instance.GetAllPosts();
        }

        // ================= CREATE POST =================
        private void PostButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(ContentTextBox.Text) &&
                string.IsNullOrWhiteSpace(GithubTextBox.Text))
            {
                return;
            }

            var post = new Post
            {

                AuthorName = _user.Name,
                Content = ContentTextBox.Text,
                GithubLink = GithubTextBox.Text,
                AttachmentPaths = new List<string>(_attachments)
            };

            DataService.Instance.AddPost(post);

            // Clear UI
            ContentTextBox.Clear();
            GithubTextBox.Clear();
            _attachments.Clear();

            GithubChip.Visibility = Visibility.Collapsed;
            GithubChipText.Text = "";

            LoadPosts();
        }

        // ================= ATTACH FILES =================
        private void AttachButton_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new Microsoft.Win32.OpenFileDialog
            {
                Multiselect = true
            };

            if (dlg.ShowDialog() == true)
            {
                _attachments = dlg.FileNames.ToList();
            }
        }

        // ================= LIKE POST =================
        private void Like_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement btn && btn.Tag is Post post)
            {
                DataService.Instance.ToggleLike(post, _user.Email);
                LoadPosts();
            }
        }

        // ================= COMMENT =================
        private void Comment_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement btn && btn.Tag is Post post)
            {
                var container = FindParent<StackPanel>(btn);
                var textbox = container?
                    .Children
                    .OfType<TextBox>()
                    .FirstOrDefault();

                if (textbox != null && !string.IsNullOrWhiteSpace(textbox.Text))
                {
                    DataService.Instance.AddComment(post, new Comment
                    {
                        AuthorEmail = _user.Email,
                        Text = textbox.Text,
                        CreatedAt = DateTime.Now
                    });

                    textbox.Clear();
                    LoadPosts();
                }
            }
        }

        // ================= NAVIGATION =================
        private void OpenProfile_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new ProfileWindow(_user).Show();
            Close();
        }

        private void OpenNotifications(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new NotificationWindow(_user).Show();
            Close();
        }

        private void Logout_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }

        private void OpenFeed_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            LoadPosts();
        }

        // ================= GITHUB CHIP LOGIC =================
        private void GithubTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(GithubTextBox.Text))
            {
                GithubChip.Visibility = Visibility.Collapsed;
                GithubChipText.Text = "";
            }
            else
            {
                GithubChip.Visibility = Visibility.Visible;
                GithubChipText.Text = "GitHub Attached";
            }
        }


        private void GithubLink_Click(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement fe && fe.DataContext is Post post)
            {
                if (!string.IsNullOrWhiteSpace(post.GithubLink))
                {
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
            }
        }
        // ================= HELPER =================
        private static T FindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parent = VisualTreeHelper.GetParent(child);

            while (parent != null)
            {
                if (parent is T typed)
                    return typed;

                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        // ================= NOTIFICATION COUNT =================
        public int NotificationCount =>
            DataService.Instance.GetAllPosts()
                .SelectMany(p => p.Comments)
                .Count();
    }

}