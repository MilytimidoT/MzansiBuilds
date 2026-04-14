using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Mzansi_Builds.Models;
using Mzansi_Builds.Services;

namespace Mzansi_Builds.Views
{
    public partial class NotificationWindow : Window, INotifyPropertyChanged
    {
        private User _user;

        private int _notificationCount;
        public int NotificationCount
        {
            get => _notificationCount;
            set
            {
                _notificationCount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasNotifications));
            }
        }

        public bool HasNotifications => NotificationCount > 0;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public NotificationWindow(User user)
        {
            InitializeComponent();
            _user = user;

            DataContext = this;

            UserEmailText.Text = _user.Email;

            LoadNotifications();
        }

        private void LoadNotifications()
        {
            var allPosts = DataService.Instance.GetAllPosts();

            var myPosts = allPosts
                .Where(p => p.AuthorEmail != null &&
                            p.AuthorEmail.Equals(_user.Email))
                .ToList();

            var notifications = new List<string>();

            foreach (var post in myPosts)
            {
                if (post.Comments != null)
                {
                    foreach (var c in post.Comments)
                    {
                        notifications.Add($"{c.AuthorEmail} commented: {c.Text}");
                    }
                }

                if (post.Likes != null)
                {
                    foreach (var l in post.Likes)
                    {
                        notifications.Add($"{l} liked your post");
                    }
                }
            }

            NotificationsList.ItemsSource = null;
            NotificationsList.ItemsSource = notifications;

            NotificationCount = notifications.Count;
            MessageBox.Show(
    $"My posts: {myPosts.Count}\n" +
    $"Notifications: {notifications.Count}");
        }

        // ================= NAVIGATION =================

        private void OpenFeed_Click(object sender, MouseButtonEventArgs e)
        {
            new FeedWindow(_user).Show();
            Close();
        }

        private void OpenProfile_Click(object sender, MouseButtonEventArgs e)
        {
            new ProfileWindow(_user).Show();
            Close();
        }

        private void Logout_Click(object sender, MouseButtonEventArgs e)
        {
            new LoginWindow().Show();
            Close();
        }
    }
}