using EncryptionDiary.Client.Services;
using EncryptionDiary.Shared.Helper;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Xps;

namespace EncryptionDiary.Client.Windows
{
    /// <summary>
    /// Interaction logic for ShamirWindow.xaml
    /// </summary>
    public partial class ShamirWindow : Window
    {
        private Shared.Models.Key _key;
        private byte[] _encPasswordHash;
        private string _username;
        private ApiService _apiService;
        public ShamirWindow(Shared.Models.Key key, byte[] encPasswordHash, string username,ApiService apiService)
        {
            InitializeComponent();
            _key = key;
            _encPasswordHash = encPasswordHash;
            txtKeyDescription.Text = key.Description;
            _username = username;
            _apiService = apiService;
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new PasswordDialog();
            if (dialog.ShowDialog() == true)
            {
                var hash = PasswordHelper.ClientEncHash( dialog.Password, _username);
                if (hash.SequenceEqual(_encPasswordHash))
                {
                    int n, k = 0;
                    if (!int.TryParse(txtShares.Text, out n) ||
                         !int.TryParse(txtThreshold.Text, out k))
                    {
                        MessageBox.Show("Enter valid numbers", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    if (k > n)
                    {
                        MessageBox.Show("Threshold can't be greater than shares", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    if (k < 2)
                    {
                        MessageBox.Show("Threshold must be at least 2", "Error", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    lstKeys.ItemsSource = null;
                    lstKeys.Items.Clear();
                    lstKeys.ItemsSource = ShamirService.GenerateShares(n, k, _encPasswordHash, _key);
                    btnGenerate.IsEnabled = false;
                    txtShares.IsEnabled = false;
                    txtThreshold.IsEnabled = false;
               }
                else
                {
                    MessageBox.Show("Wrong password");
                }
            }
        }

        private void NumberOnly(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !int.TryParse(e.Text, out int num) || num < 0;
        }

        private async void btnSave_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.Filter = "Text Files (*.txt)|*.txt";
            saveDialog.FileName = $"SSSS_{_key.Description}_{DateTime.Now:yyyyMMdd}";

            if (saveDialog.ShowDialog() == true)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine($"Username: {_username}");
                sb.AppendLine("==== Shamir's Secret Sharing ====");
                sb.AppendLine($"Key: {_key.Description}");
                sb.AppendLine($"ID: {_key.ID}");
                sb.AppendLine($"SSSS generated at {DateTime.Now}");
                sb.AppendLine($"Threshold: {txtThreshold.Text} of {txtShares.Text}");

                sb.AppendLine();
                foreach(var item in lstKeys.Items)
                {  
                    sb.AppendLine(item.ToString()); 
                }
                File.WriteAllText(saveDialog.FileName, sb.ToString());
                var keys =  await _apiService.MarkAsShared(_key.ID.Value);

            }
        }
    }
}
