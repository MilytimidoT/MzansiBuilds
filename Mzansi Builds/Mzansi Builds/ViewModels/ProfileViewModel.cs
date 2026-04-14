
using Mzansi_Builds.Models;
using Mzansi_Builds.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Mzansi_Builds.ViewModels
{
    public class ProfileViewModel : INotifyPropertyChanged
    {
        private readonly DataService _data = DataService.Instance;

        public User CurrentUser { get; }
        public ObservableCollection<Post> Posts { get; } = new ObservableCollection<Post>();

        public ProfileViewModel(User user)
        {
            CurrentUser = user;
        }

        public async Task LoadDataAsync()
        {
            await Task.Run(() =>
            {
                var posts = _data.GetAllPosts().Where(p => p.AuthorEmail == CurrentUser.Email).OrderByDescending(p => p.CreatedAt).ToList();
                App.Current.Dispatcher.Invoke(() =>
                {
                    Posts.Clear();
                    foreach (var p in posts) Posts.Add(p);
                });
            });
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
