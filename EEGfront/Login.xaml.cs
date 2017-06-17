using GateKeep;
using System;
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

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            //string answer = await loginModule.Shake(this.User.Text, this.Pass.Text);

            //try
            //{
            //    Console.WriteLine(answer);
            //    string userid = string.Empty;

            //    userid = answer.Split(',')[2].Split(':')[1];

            //    if (string.IsNullOrEmpty(userid))
            //        return;

            //    Console.WriteLine(userid.Substring(0, userid.Length - 1));


            //    if (!string.IsNullOrEmpty(userid))
            //    {
                    //var pca = new MainWindow();

         
                    //pca.DataContext = new MainViewModel("asd");

                    //pca.Show();
            //        //pca.Closing += Pca_Closed;
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Failed for basic reason: " + ex);
            //}
        }
    }
}
