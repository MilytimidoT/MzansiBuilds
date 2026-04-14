
using System.Windows;
using Mzansi_Builds.Models;
using Mzansi_Builds.Services;
using Mzansi_Builds.ViewModels;

namespace Mzansi_Builds.Views
{
    public partial class ProfileWindow : Window
    {
        private readonly ProfileViewModel _vm;

        public ProfileWindow(User user)
        {
            InitializeComponent();

            DataService.Instance.Initialize();

            var stored = DataService.Instance.GetUserByEmail(user?.Email) ?? user;
            _vm = new ProfileViewModel(stored);
            DataContext = _vm;

            Loaded += async (s, e) => await _vm.LoadDataAsync();
        }
    }
}
