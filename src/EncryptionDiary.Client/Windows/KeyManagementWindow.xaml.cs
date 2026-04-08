using EncryptionDiary.Client.Services;
using EncryptionDiary.Shared.Helper;
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
    /// Interaction logic for KeyManagementWindow.xaml
    /// </summary>
    public partial class KeyManagementWindow : Window
    {
        private Guid _userID;
        private byte[] _hashEncPassword;
        private ApiService _apiService;

        public KeyManagementWindow(Guid userID, byte[] hashEncPassword, ApiService apiService)
        {
            InitializeComponent();
            _userID = userID;
            _hashEncPassword = hashEncPassword;
            _apiService = apiService;
            LoadKeys();

        }

        private async void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var selectedItem = (ComboBoxItem)cmbKeySize.SelectedItem;
            var keySize = int.Parse(selectedItem.Tag.ToString());
            var description = txtDescription.Text;
            var isLocal = rbLocal.IsChecked == true;

            // Generate the key
            var rawKey = CryptoServices.GenerateKey(keySize);

            // Encrypt the key with the enc hash (KEK)
            var (nonce, encryptedKey, tag) = CryptoServices.EncryptAES_GCM(_hashEncPassword, rawKey);

            if (isLocal)
            {
                // TODO: save locally
            }
            else
            {
                // Send to server
                var key = new Shared.Models.Key
                {
                    UserID = _userID,
                    EncKey = encryptedKey,
                    KeyNonce = nonce,
                    KeyTag = tag,
                    Description = description,
                    Shared = false
                };
                await _apiService.InsertKey(key);
            }

            LoadKeys();
        }
        private async void LoadKeys()
        {
            var keys = await _apiService.GetAllKeys(_userID);
            lstKeys.ItemsSource = keys;
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
