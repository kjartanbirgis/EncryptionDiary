using EncryptionDiary.Client.Services;
using EncryptionDiary.Shared.Helper;
using EncryptionDiary.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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
using System.Xml.Linq;

namespace EncryptionDiary.Client
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private ServerConfigService _configService = new ServerConfigService();
        private Dictionary<string, ServerConnection> _savedServers;

        public LoginWindow()
        {
            InitializeComponent();
            LoadServers();
        }

        private void LoadServers()
        {
            _savedServers = _configService.Load();
            cmbServers.Items.Clear();
            cmbServers.Items.Add("-- New Server --");
            foreach (var key in _savedServers.Keys)
            {
                cmbServers.Items.Add(key);
            }
            cmbServers.SelectedIndex = 0;
        }

        private void cmbServers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selected = cmbServers.SelectedItem?.ToString();
            if (selected != null && _savedServers.ContainsKey(selected))
            {
                var server = _savedServers[selected];
                txtServerUrl.Text = server.Url;
                txtUsername.Text = server.Username;
            }
            else
            {
                txtServerUrl.Text = "";
                txtUsername.Text = "";
            }
        }



        private async void btnLogin_Click(object sender, RoutedEventArgs e)
        {
            var serverUrl = txtServerUrl.Text;
            var username = txtUsername.Text;
            var password = txtPassword.Password;

            var clientAuthHash = PasswordHelper.ClientAuthHash(password, username);
            var clientEncHash = PasswordHelper.ClientEncHash(password, username);
            var api = new ApiService(serverUrl);

            var response = await api.Login(username, clientAuthHash);
            if (response!=null)
            {
                CheckSavedURL(serverUrl, username, serverUrl);
                var mainWindow = new MainWindow(response.ID, clientEncHash, api,username,clientAuthHash);
                mainWindow.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Login failed");
            }

        }
        private void CheckSavedURL(string url, string username,string name)
        {
            if (!_savedServers.ContainsKey(url))
            {
                _savedServers[txtServerUrl.Text] = new ServerConnection
                {
                    Name = name,
                    Url = url,
                    Username = username
                };
                _configService.Save(_savedServers);
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
            var registerResponse = await api.Register(username, clientAuthHash);
            if (registerResponse != null)
            {
                CheckSavedURL(serverUrl, username, serverUrl);
                var mainWindow = new MainWindow(registerResponse.ID, clientEncHash, api,username, clientAuthHash);
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
