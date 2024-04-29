using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace control
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>

    public partial class LoginWindow : Window
    {
        public LoginWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Kullanıcı adı ve şifreyi kontrol edin
            string username = UsernameTextBox.Text;
            string password = PasswordBox.Password;

            // Burada doğrulama kodunu ekleyin
            if (IsLoginValid(username, password))
            {
                // Giriş başarılı
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();

                this.Close(); // Giriş ekranını kapat
            }
            else
            {
                // Giriş başarısız, kullanıcıya hata göster
                MessageBox.Show("Login failed. Please check your username and password.");
            }
        }

        private bool IsLoginValid(string username, string password)
        {
            // Doğrulama mekanizmanızın kodunu buraya ekleyin
            // Örneğin, sabit bir kullanıcı adı ve şifre kontrolü
            return username == "user" && password == "pass";
        }
    }
}
