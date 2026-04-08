using EncryptionDiary.Client.Services;
using EncryptionDiary.Client.Windows;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace EncryptionDiary.Client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Services.ApiService _apiService = null;
        private byte[] _hashEncPassword = null;
        private Guid _userID;
        public MainWindow( Guid userID, byte[] hashEncPassword,  ApiService apiService)
        {
            InitializeComponent();
            _hashEncPassword = hashEncPassword;
            _userID = userID;
            _apiService = apiService;
        }

        private void mnuKeyManagement_Click(object sender, RoutedEventArgs e)
        {
            var keyWindow = new KeyManagementWindow(_userID,_hashEncPassword,_apiService);
            keyWindow.ShowDialog();
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

    }
}