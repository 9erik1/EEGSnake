using GateKeep;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace EEGfront
{
    public class LoginViewModel : INotifyPropertyChanged
    {
        private HandShake loginModule;

        public LoginViewModel()
        {
            User = "Satan";
            Pass = "IN";

            //using of the login module
            loginModule = new HandShake();
        }

        public void Shutdown()
        {

        }

        private async void Clickey()
        {
            try
            {
                string answer = await loginModule.Shake(User, Pass);
                Err = answer;
                Console.WriteLine(answer);
                string userid = string.Empty;

                if (!string.IsNullOrEmpty(answer.Split(',')[2].Split(':')[1]))
                    userid = answer.Split(',')[2].Split(':')[1];
                Console.WriteLine(userid.Substring(0,userid.Length-1));


                if(!string.IsNullOrEmpty(userid))
                {
                    var pca = new MainWindow();
                    pca.DataContext = new MainViewModel("");

                    pca.Show();
                    //pca.Closing += Pca_Closed;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Failed for basic reason: " + e);
                //Err = e.Message;
            }
        }


        public string[] Title { get; private set; }

        private string err;
        public string Err
        {
            get { return err; }
            set
            {
                if (value != err)
                {
                    err = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string User { get; set; }
        public string Pass { get; set; }

        private ICommand optionChanged;
        public ICommand OptionChanged
        {
            get
            {
                if (optionChanged == null)
                {
                    optionChanged = new RelayCommand(
                        p => true,
                        p => Clickey());
                }
                return optionChanged;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
