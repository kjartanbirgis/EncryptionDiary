using EncryptionDiary.Client.Services;
using EncryptionDiary.Client.Windows;
using EncryptionDiary.Shared.Helper;
using EncryptionDiary.Shared.Models;
using System.Net.Http.Headers;
using System.Printing;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Key = EncryptionDiary.Shared.Models.Key;

namespace EncryptionDiary.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _sortAscending = true;
        private string _lastSortColumn = "";

        private Services.ApiService _apiService = null;
        private byte[] _hashEncPassword = null;
        private List<DiaryEntry> _decryptedEntries = new List<DiaryEntry>();
        private Guid _userID;
        private DiaryEntry _entry = new DiaryEntry();
        private string _username;
        public MainWindow( Guid userID, byte[] hashEncPassword,  ApiService apiService, string username)
        {
            InitializeComponent();
            _hashEncPassword = hashEncPassword;
            _userID = userID;
            _apiService = apiService;
            _username = username;
            LoadKeys();
            LoadDiaries();
        }

        private void mnuKeyManagement_Click(object sender, RoutedEventArgs e)
        {
            var keyWindow = new KeyManagementWindow(_userID,_hashEncPassword,_apiService,_username);
            keyWindow.ShowDialog();
            LoadKeys();
        }

        private void mnuLogout_Click(object sender, RoutedEventArgs e)
        {
            var login = new LoginWindow();
            login.Show();
            this.Close();
        }

        private void mnuExit_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void mnuAddServer_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void mnuSync_Click(object sender, RoutedEventArgs e)
        {
            // TODO
        }

        private void btnNewEntry_Click(object sender, RoutedEventArgs e)
        {
            ClearBoxes();
            
        }

        private void btnDeleteEntry_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_entry.ID == null)
            {
                //TODO: New entry
                _entry.EntryDate = dpEntryDate.SelectedDate.Value;
                _entry.Title = txtTitle.Text;
                _entry.Content = txtContent.Text;
                var key = (Shared.Models.Key)cmbKeys.Items[ cmbKeys.SelectedIndex];
                _entry.KeyID = key.ID.Value;
                var rawKey = CryptoServices.DecryptAES_GCM(_hashEncPassword, key.KeyNonce, key.EncKey, key.KeyTag);
                var jsonObject = JsonSerializer.Serialize(_entry);
                var plainBytes = Encoding.UTF8.GetBytes(jsonObject);
                var output = CryptoServices.EncryptAES_GCM(rawKey, plainBytes);
                var dateNow = DateTime.UtcNow;
                var dS = new Diary()
                {
                    ID = Guid.NewGuid(),
                    EncDiaryData = output.ciphertext,
                    DiaryNonce = output.nonce,
                    DiaryTag = output.tag,
                    KeyID = key.ID.Value,
                    Created = dateNow,
                    Updated = dateNow,
                    UserID = _userID
                };
                var respnose = await _apiService.InsertDiary(dS);
                if ( respnose == false)
                { 
                    MessageBox.Show("Failed to save diary entry","Error",MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                _decryptedEntries.Add(_entry);
                lstEntries.ItemsSource = null;
                lstEntries.ItemsSource = _decryptedEntries;

            }
            else
            {
                var selectedKey = (Key)cmbKeys.SelectedItem;
                if(_entry.Title == txtTitle.Text &&
                    _entry.Content == txtContent.Text &&
                    _entry.EntryDate == dpEntryDate.SelectedDate &&
                    _entry.KeyID == selectedKey.ID)
                { 
                    //nothing changed
                    return;
                }
                _entry.EntryDate = dpEntryDate.SelectedDate.Value;
                _entry.Title = txtTitle.Text;
                _entry.Content = txtContent.Text;
                var key = (Shared.Models.Key)cmbKeys.Items[cmbKeys.SelectedIndex];
                _entry.KeyID = key.ID.Value;
                var rawKey = CryptoServices.DecryptAES_GCM(_hashEncPassword, key.KeyNonce, key.EncKey, key.KeyTag);
                var jsonObject = JsonSerializer.Serialize(_entry);
                var plainBytes = Encoding.UTF8.GetBytes(jsonObject);
                var output = CryptoServices.EncryptAES_GCM(rawKey, plainBytes);
                var dateNow = DateTime.UtcNow;
                var dS = new Diary()
                {
                    ID = _entry.ID,
                    EncDiaryData = output.ciphertext,
                    DiaryNonce = output.nonce,
                    DiaryTag = output.tag,
                    KeyID = key.ID.Value,
                    Updated = dateNow,
                    UserID = _userID
                };
                var respnose = await _apiService.UpdateDiary(dS);
                if (respnose == false)
                {
                    MessageBox.Show("Failed to save diary entry", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                
                lstEntries.ItemsSource = null;
                lstEntries.ItemsSource = _decryptedEntries;

            }
        }

        private void btnAttachment_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (lstEntries.SelectedItem is DiaryEntry entry)
            {
                _entry = entry;
                txtTitle.Text = entry.Title;
                txtContent.Text = entry.Content;
                dpEntryDate.SelectedDate = entry.EntryDate;
                for (int i = 0; i < cmbKeys.Items.Count; i++)
                {
                    var key = (Shared.Models.Key)cmbKeys.Items[i];
                    if (key.ID == entry.KeyID)
                    {
                        cmbKeys.SelectedIndex = i;
                        break;
                    }
                }
            }
        }

        private async void LoadKeys()
        {
            var keys = await _apiService.GetAllKeys(_userID);
            cmbKeys.ItemsSource = null;
            cmbKeys.Items.Clear();
            cmbKeys.ItemsSource = keys;
            if (keys.Count > 0)
            {
                cmbKeys.SelectedIndex = 0;
            }
        }

        private async void LoadDiaries()
        {
            var diaries = await _apiService.GetAllDiaries(_userID);
            var keys = await _apiService.GetAllKeys(_userID);
            _decryptedEntries.Clear();

            foreach (var diary in diaries)
            {
                var key = keys.FirstOrDefault(k => k.ID == diary.KeyID);
                if (key == null)
                { continue; }
                var rawKey = CryptoServices.DecryptAES_GCM(_hashEncPassword, key.KeyNonce, key.EncKey, key.KeyTag);

                var plainBytes = CryptoServices.DecryptAES_GCM(rawKey, diary.DiaryNonce, diary.EncDiaryData, diary.DiaryTag);
                var json = Encoding.UTF8.GetString(plainBytes);
                var entry = JsonSerializer.Deserialize<DiaryEntry>(json);
                entry.ID = diary.ID;
                entry.KeyID = diary.KeyID;
                _decryptedEntries.Add(entry);

            }
            lstEntries.ItemsSource = _decryptedEntries;
        }

        private void ClearBoxes()
        {
            txtContent.Clear();
            txtTitle.Clear();
            dpEntryDate.SelectedDate = DateTime.Now;
            _entry = new();
        }

        private void lstEntries_HeaderClick(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader header && header.Column != null)
            {
                var column = header.Column.Header.ToString();

                if (_lastSortColumn == column)
                    _sortAscending = !_sortAscending;
                else
                    _sortAscending = true;

                _lastSortColumn = column;

                lstEntries.ItemsSource = column switch
                {
                    "Title" => _sortAscending
                        ? _decryptedEntries.OrderBy(e => e.Title).ToList()
                        : _decryptedEntries.OrderByDescending(e => e.Title).ToList(),
                    "Date" => _sortAscending
                        ? _decryptedEntries.OrderBy(e => e.EntryDate).ToList()
                        : _decryptedEntries.OrderByDescending(e => e.EntryDate).ToList(),
                    _ => _decryptedEntries
                };
            }
        }
    }
}