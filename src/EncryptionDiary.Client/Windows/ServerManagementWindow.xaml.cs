using EncryptionDiary.Client.Services;
using EncryptionDiary.Shared.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
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
using Key = EncryptionDiary.Shared.Models.Key;

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
        private Guid _userID;

        public ServerManagementWindow(byte[] authHash, ApiService apiService, Guid userID)
        {
            InitializeComponent();
            _authHash = authHash;
            _configService = new ServerConfigService();
            LoadServers();
            _apiService = apiService;
            _userID = userID;
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
            ApiService registerService = new ApiService(url);
            var result = registerService.Register(username, _authHash, _userID);
            if (result == null)
            {
                MessageBox.Show("Couldnot register user to the server", "error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //Hugsanlega meiri check!!!
            _servers[url] = new ServerConnection
            {
                Name = name,
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

        private async void btnSync_Click(object sender, RoutedEventArgs e)
        {
            var servers = _configService.Load();
            List<ApiService> apiServices = new List<ApiService>();

            foreach (var server in servers)
            {
                var api = new ApiService(server.Value.Url);
                var userResponse = await api.Login(server.Value.Username, _authHash);
                if (userResponse == null || userResponse.Username != server.Value.Username)
                {
                    //error
                }
                else
                {
                    apiServices.Add(api);
                }
            }
            //Tómt allir lykllar
            var allKeys = new Dictionary<Guid, (Key key, ApiService source)>();
            foreach (var api in apiServices)
            {
                var keys = await api.GetAllKeys(_userID);
                foreach (var key in keys)
                {
                    if (!allKeys.ContainsKey(key.ID.Value) || key.Updated > allKeys[key.ID.Value].key.Updated.Value)
                    {
                        allKeys[key.ID.Value] = (key, api);
                    }
                }
            }
            //setjum allt á serverana ef 
            foreach (var api in apiServices)
            {
                var serverKeys = await api.GetAllKeys(_userID);
                foreach (var (id, newest) in allKeys)
                {
                    var existing = serverKeys.FirstOrDefault(k => k.ID.Value == id);
                    if (existing == null)
                    {
                        await api.InsertKey(newest.key);
                    }
                    else if (newest.key.Updated.Value > existing.Updated.Value)
                    {
                        await api.UpdateKey(newest.key);

                    }
                }
            }
            //sama og lyklarnir
            var allDiaries = new Dictionary<Guid, (Diary diary, ApiService source)>();
            foreach (var api in apiServices)
            {
                var diaries = await api.GetAllDiaries(_userID);
                foreach (var diary in diaries)
                {
                    if (!allDiaries.ContainsKey(diary.ID.Value) || diary.Updated > allDiaries[diary.ID.Value].diary.Updated.Value)
                    {
                        allDiaries[diary.ID.Value] = (diary, api);
                    }
                }
            }
            //setjum allt á serverana ef 
            foreach (var api in apiServices)
            {
                var serverDiary = await api.GetAllDiaries(_userID);
                foreach (var (id, newest) in allDiaries)
                {
                    var existing = serverDiary.FirstOrDefault(k => k.ID.Value == id);
                    if (existing == null)
                    {
                        await api.InsertDiary(newest.diary);
                    }
                    else if (newest.diary.Updated.Value > existing.Updated.Value)
                    {
                        await api.UpdateDiary(newest.diary);

                    }
                }
            }
        }
    }
}