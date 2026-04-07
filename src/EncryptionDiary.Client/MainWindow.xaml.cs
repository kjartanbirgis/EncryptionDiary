using EncryptionDiary.Client.Services;
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
        private string _username;
        public MainWindow( string username, byte[] hashEncPassword,  ApiService apiService)
        {
            InitializeComponent();
            _hashEncPassword = hashEncPassword;
            _username = username;
            _apiService = apiService;
        }
    }
}