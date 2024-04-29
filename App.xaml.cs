using System.Configuration;
using System.Data;
using System.Windows;

namespace control
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Giriş ekranını göster
            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
        }

    }

}
