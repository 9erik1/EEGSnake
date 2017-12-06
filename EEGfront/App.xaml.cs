using System.Windows;


namespace EEGfront
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var login = new Login();
            login.DataContext = LoginViewModel.Instance;
            login.Show();
        }
    }
}
