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
        private Dictionary<string, ServerConnection> _servers;
        private readonly byte[] _authHash;

        public ServerManagementWindow(byte[] authHash)
        {
            InitializeComponent();
            _authHash = authHash;
            _configService = new ServerConfigService();
            LoadServers();
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
                string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(url) || string.IsNullOrWhiteSpace(username) )
            {
                MessageBox.Show("Please fill in all fields.","Error",MessageBoxButton.OK, MessageBoxImage.Error);
            }
            if (_servers.ContainsKey(name))
            {
                MessageBox.Show("Server already exist","Warning",MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            //register User to the Database

        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnTest_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
