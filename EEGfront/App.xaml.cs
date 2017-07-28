﻿using System;
using System.Windows;


namespace EEGfront
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            if (System.Windows.Forms.Screen.AllScreens.Length > 1)
            {
            }

            var login = new Login();
            login.DataContext = new LoginViewModel();

            login.Show();



            //var pca = new MainWindow();
            //pca.DataContext = new MainViewModel("");

            //pca.Show();
            //pca.Closing += Pca_Closed;





        }
    }
}
