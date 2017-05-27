using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using OpenTK;
using System.Windows;
using System.Net;
using System.IO;
using GateKeep;


namespace EEGfront
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public enum AppState
        {
            Game,
            Train,
            Stats
        }


        private bool IsDraw = true;
        Thread draw;

        private EmotiveAquisition stream;
        private GLControl graphics;

        private HandShake loginModule;


        public MainViewModel()
        {
            // because we use untrusted ssl ;)
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            //using of the login module
            loginModule = new HandShake();


            //stream = EmotiveAquisition.Instance;

            menuItems = new ObservableCollection<AppState>();
            menuItems.Add(AppState.Game);
            menuItems.Add(AppState.Train);
            menuItems.Add(AppState.Stats);

            currentOption = AppState.Game;
            CurrentOptionChange(currentOption);

            MenuItem.CollectionChanged += MenuItem_CollectionChanged;

            gameToggle = Visibility.Visible;

            //OptionChanged = new RelayCommand

            draw = new Thread(new ThreadStart(Draw));
            draw.Start();
        }

        private void MenuItem_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("Max Plebian");
        }

        public void Shutdown()
        {
            IsDraw = false;
        }


        private async void Draw()
        {
            while (IsDraw)
            {

                await Task.Delay(2000);
            }
        }

        private void CurrentOptionChange(AppState s)
        {
            Console.WriteLine("Warning! App State Change: " + s);
            if (s == AppState.Train)
            {
                GameToggle = Visibility.Hidden;
                TrainToggle = Visibility.Visible;
            }
            if (s == AppState.Game)
            {
                GameToggle = Visibility.Visible;
                TrainToggle = Visibility.Hidden;
            }
        }

        private async void Clickey()
        {
            try
            {

                Console.WriteLine(await loginModule.Shake());
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed for basic reason: " + e);
            }
        }


        public string[] Title { get; private set; }
        private ICommand optionChanged;
        public ICommand OptionChanged
        {
            get
            {
                if (optionChanged == null)
                {
                    optionChanged = new RelayCommand(
                        p => true,
                        p => this.Clickey());
                }
                return optionChanged;
            }
        }
        private Visibility trainToggle;
        public Visibility TrainToggle
        {
            get { return trainToggle; }
            set
            {
                if (value != trainToggle)
                {
                    trainToggle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Visibility gameToggle;
        public Visibility GameToggle
        {
            get { return gameToggle; }
            set
            {
                if (value != gameToggle)
                {
                    gameToggle = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private AppState currentOption;
        public AppState CurrentOption
        {
            get { return currentOption; }
            set
            {
                if (value != currentOption)
                {
                    currentOption = value;
                    CurrentOptionChange(currentOption);
                    NotifyPropertyChanged();
                }
            }
        }
        private IList<AppState> menuItems;
        public ObservableCollection<AppState> MenuItem
        {
            get
            {
                return (ObservableCollection<AppState>)menuItems;
            }
            set
            {
                if (value != menuItems)
                {
                    this.menuItems = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
