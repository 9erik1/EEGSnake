using System.Windows;
using System.Windows.Input;

namespace EEGfront
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {           
            this.WindowState = System.Windows.WindowState.Maximized;
            InitializeComponent();
      
        }                                       
        private void Wave_KeyDown(object sender, KeyEventArgs e)
        {
            var keyPres = e.Key;

            switch (keyPres)
            {
                case Key.Escape:
                    System.Windows.Application.Current.Shutdown();
                    break;
                case Key.A:
                    break;
                case Key.B:
                    break;
                case Key.C:
                    break;
            }
        }
    }
}
