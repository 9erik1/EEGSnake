using GateKeep;
using System.Net;
using System.Windows;


namespace EEGfront
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        private HandShake loginModule;
        public Login()
        {
            InitializeComponent();
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;
            loginModule = new HandShake();
        }
    }
}
