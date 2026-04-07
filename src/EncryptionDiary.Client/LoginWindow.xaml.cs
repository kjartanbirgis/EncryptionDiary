using EncryptionDiary.Client.Services;
using EncryptionDiary.Shared.Helper;
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

namespace EncryptionDiary.Client
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


            private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var serverUrl = txtServerUrl.Text;
            var username = txtUsername.Text;
            var password = txtPassword.Password;

            var clientAuthHash = PasswordHelper.ClientAuthHash(password, username);
            var clientEncHash = PasswordHelper.ClientEncHash(password, username);
            var api = new ApiService(serverUrl);

            if (await api.Login(username, clientAuthHash))
            {
                var mainWindow = new MainWindow(username, clientEncHash, api);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Login failed");
            }

        }

        private async void btnRegister_Click(object sender, RoutedEventArgs e)
        {
            var serverUrl = txtServerUrl.Text;
            var username = txtUsername.Text;
            var password = txtPassword.Password;

            var clientAuthHash = PasswordHelper.ClientAuthHash(password, username);
            var clientEncHash = PasswordHelper.ClientEncHash(password, username);
            var api = new ApiService(serverUrl);

            if (await api.Register(username, clientAuthHash))
            {
                var mainWindow = new MainWindow(username, clientEncHash, api);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Registration failed");
            }
        }
    }
}
