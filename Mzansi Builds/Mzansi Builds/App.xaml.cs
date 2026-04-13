using Mzansi_Builds.Views;
using System.Windows;

namespace Mzansi_Builds
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var login = new LoginWindow();
            login.Show();
        }
    }
}