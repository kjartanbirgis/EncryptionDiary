using EncryptionDiary.Client.Services;
using EncryptionDiary.Shared.Models;
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

namespace EncryptionDiary.Client.Windows
{
    /// <summary>
    /// Interaction logic for ServerManagementWindow.xaml
    /// </summary>
    public partial class ServerManagementWindow : Window
    {
        private readonly ServerConfigService _configService;
        private ApiService _apiService;
        private Dictionary<string, ServerConnection> _servers;
        private readonly byte[] _authHash;

        public ServerManagementWindow(byte[] authHash, ApiService apiService)
        {
            InitializeComponent();
            _authHash = authHash;
            _configService = new ServerConfigService();
            LoadServers();
            _apiService = apiService;
        }
        private void LoadServers()
        {
            _servers = _configService.Load();
            lstServers.ItemsSource = _servers.Values.ToList();
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var name = txtName.Text;
            var url = txtUrl.Text;
            var username = txtUsername.Text;

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(url) || string.IsNullOrEmpty(username) ||
                string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(username))
            {
                MessageBox.Show("Please fill in all fields.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (_servers.ContainsKey(name))
            {
                MessageBox.Show("Server already exist", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            //register User to the Database
            var result = _apiService.Register(username, _authHash);
            if (result == null)
            {
                MessageBox.Show("Couldnot register user to the server", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //Hugsanlega meiri check!!!
            _servers[url] = new ServerConnection { Name = name, 
                Url = url, 
                Username = username 
            };    
            _configService.Save(_servers);
            txtName.Clear();
            txtUrl.Clear();
            txtUsername.Clear();



        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            //Spurning um að hard eyða öllum gögnum af servernum.!!

        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {
            //opna tengingar við alla serverana.

        }
    }
}
