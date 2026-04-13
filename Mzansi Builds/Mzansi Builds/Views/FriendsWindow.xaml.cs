using System.Linq;
using System.Windows;
using Mzansi_Builds.Models;
using Mzansi_Builds.Services;

namespace Mzansi_Builds.Views
{
    public partial class FriendsWindow : Window
    {
        private readonly User _user;

        public FriendsWindow(User user)
        {
            InitializeComponent();
            _user = user;

            LoadData();
        }

        private void LoadData()
        {
            // Users
            var users = DataService.Instance.SearchUsers(_user.Email);

            UsersList.ItemsSource = users.Select(u => new
            {
                u.Email,
                Action = "Add"
            });

            // Requests
            var requests = DataService.Instance.GetPendingRequests(_user.Email);

            RequestsList.ItemsSource = requests;

            // Friends
            var friends = DataService.Instance.GetFriends(_user.Email);

            FriendsList.ItemsSource = friends;
        }
    }
}