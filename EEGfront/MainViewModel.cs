using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using System.Windows;
using System.Net;
using System.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Accord.IO;
using System.Drawing;
using System.Windows.Media;

namespace EEGfront
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private static MainViewModel instance;
        public static MainViewModel Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new MainViewModel("");
                }
                return instance;
            }
        }
        public enum AppState
        {
            Game,
            Train,
            Stats
        }

        private int xPos = 0;
        private int yPos = 0;

        //The collected raws for the current user
        TrainingInputManager T_I_M;

        private bool IsDraw = true;
        Thread draw;

        private EmotiveAquisition stream;
        private Rest restService;
        private SVMClassifier machineStudent;

        MulticlassSupportVectorMachine<Gaussian> currentClassifier;
        private MainViewModel(string idTag)
        {
            restService = Rest.Instance;
            stream = EmotiveAquisition.Instance;
            stream.ReverseKill();
            machineStudent = new SVMClassifier();

            T_I_M = new TrainingInputManager();

            Task.Run(async () =>
            {
                T_I_M = await restService.PostCurrentRaw("8");
                Console.WriteLine(T_I_M);
                if (T_I_M == null)
                    T_I_M = new TrainingInputManager();
            });

            //Dispatcher.BeginInvoke((Action)(() =>
            //{
            //    Console.WriteLine("asd");
            //}));

            Task.Run(async () => currentClassifier = await restService.PostCurrent("13"));

            // Example 
            //await restService.Get("https://192.168.0.173:5900/rest/");
            //await restService.PostCurrent("8");
            //await restService.PostPrev("8");

            Console.WriteLine("user id success in view model: " + idTag);

            Dir = 0;
            Trials = 0;
            // because we use untrusted ssl ;)
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            //stream = EmotiveAquisition.Instance;

            menuItems = new ObservableCollection<AppState>();
            menuItems.Add(AppState.Game);
            menuItems.Add(AppState.Train);
            menuItems.Add(AppState.Stats);

            currentOption = AppState.Game;
            CurrentOptionChange(currentOption);

            gameToggle = Visibility.Visible;
            upToggle = Visibility.Visible;
            downToggle = Visibility.Visible;
            leftToggle = Visibility.Visible;
            rightToggle = Visibility.Visible;


            draw = new Thread(new ThreadStart(Draw));
            draw.Start();
            //private Brush _colorr = Brushes.Red;
            //SnakeGame = Brushes.Green
            //snakeGame = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            //int[,] array6 = new int[10, 10];

            for(int i = 0; i < 20; i++)
            {
                snakeGame[i] = new SolidColorBrush[20];
                snakeGameProx[i] = new SolidColorBrush[20];
            }

            for (int j = 0; j < 20; j++)
            {
                for (int k = 0; k < 20; k++)
                {
                    snakeGame[j][k] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                    snakeGameProx[j][k] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                }
            }
            snakeGame[0][0] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
        }

        public void Up()
        {
            Console.WriteLine("Up");
            snakeGameProx[yPos][xPos] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
            yPos++;
            snakeGameProx[yPos][xPos] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            SnakeGame = (SolidColorBrush[][])snakeGameProx.Clone();
        }

        public void Down()
        {
            Console.WriteLine("Down");            
            snakeGameProx[yPos][xPos] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
            yPos--;
            snakeGameProx[yPos][xPos] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            SnakeGame = (SolidColorBrush[][])snakeGameProx.Clone();
        }

        public void Left()
        {
            Console.WriteLine("Left");
            snakeGameProx[yPos][xPos] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
            xPos--;
            snakeGameProx[yPos][xPos] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            SnakeGame = (SolidColorBrush[][])snakeGameProx.Clone();
        }

        public void Right()
        {
            Console.WriteLine("Right");
            snakeGameProx[yPos][xPos] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
            xPos++;
            snakeGameProx[yPos][xPos] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            SnakeGame = (SolidColorBrush[][])snakeGameProx.Clone();
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
                UpToggle = Visibility.Hidden;
                DownToggle = Visibility.Hidden;
                LeftToggle = Visibility.Hidden;
                RightToggle = Visibility.Hidden;
                return;
            }
            if (s == AppState.Game)
            {
                GameToggle = Visibility.Visible;
                TrainToggle = Visibility.Hidden;
                UpToggle = Visibility.Visible;
                DownToggle = Visibility.Visible;
                LeftToggle = Visibility.Visible;
                RightToggle = Visibility.Visible;
                return;
            }
            if (s == AppState.Stats)
            {
                var stat = new StatsWindow();
                stat.DataContext = new StatsViewModel();

                stat.Show();
                stat.Closing += Stat_Closing;
            }
        }

        private void Stat_Closing(object sender, CancelEventArgs e)
        {
            StatsWindow statWin = sender as StatsWindow;
            StatsViewModel statVM = statWin.DataContext as StatsViewModel;
            statVM.Shutdown();
        }

        private void Clickey()
        {
            try
            {
                //Console.WriteLine(await loginModule.Shake(User,Pass));
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed for basic reason: " + e);
            }
        }

        private async void ManTest()
        {
            try
            {
                Console.WriteLine("Manual Test. Paramaters are " + Dir.ToString() + " and " + Trials.ToString());

                switch (Dir)
                {
                    case 0:
                        UpToggle = Visibility.Visible;
                        break;
                    case 1:
                        RightToggle = Visibility.Visible;
                        break;
                    case 2:
                        DownToggle = Visibility.Visible;
                        break;
                    case 3:
                        LeftToggle = Visibility.Visible;
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }
                await Task.Delay(Trials * 1000);
                switch (Dir)
                {
                    case 0:
                        UpToggle = Visibility.Hidden;
                        break;
                    case 1:
                        RightToggle = Visibility.Hidden;
                        break;
                    case 2:
                        DownToggle = Visibility.Hidden;
                        break;
                    case 3:
                        LeftToggle = Visibility.Hidden;
                        break;
                    default:
                        Console.WriteLine("Default case");
                        break;
                }

                PackRaw(Dir);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed for basic reason: " + e);
            }
        }
        private async void PackLearn(int dir)
        {
            var x = machineStudent.MachineLearn(stream.dataWindow);

            machineStudent.UpdateSVM(stream.dataWindow, dir);

            var payload = new MemoryStream();

            Serializer.Save(machineStudent.Learn, payload);
            payload.Position = 0;
            await restService.UpdateModelRaw("8", payload);
            payload.Close();
        }
        private async void PackRaw(int dir)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream s = new MemoryStream();

            switch (dir)
            {
                case 0:
                    T_I_M.AddUp(stream.dataWindow, DateTime.Now);
                    break;
                case 1:
                    T_I_M.AddRight(stream.dataWindow, DateTime.Now);
                    break;
                case 2:
                    T_I_M.AddDown(stream.dataWindow, DateTime.Now);
                    break;
                case 3:
                    T_I_M.AddLeft(stream.dataWindow, DateTime.Now);
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            formatter.Serialize(s, T_I_M);
            s.Position = 0;
            await restService.UpdateModelRaw("8", s);
            s.Close();
        }

        private async void AutoTest()
        {
            try
            {
                //var x = await restService.PostCurrent("8");

                var y = machineStudent.AnswerSVM(stream.dataWindow);

                Console.WriteLine("Auto Test" + y);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed for basic reason: " + e);
            }
        }

        public string[] Title { get; private set; }

        public int Dir { get; set; }
        public int Trials { get; set; }

        private ICommand autoCommand;
        public ICommand AutoCommand
        {
            get
            {
                if (autoCommand == null)
                {
                    autoCommand = new RelayCommand(
                        p => true,
                        p => this.AutoTest());
                }
                return autoCommand;
            }
        }

        private ICommand manualCommand;
        public ICommand ManualCommand
        {
            get
            {
                if (manualCommand == null)
                {
                    manualCommand = new RelayCommand(
                        p => true,
                        p => this.ManTest());
                }
                return manualCommand;
            }
        }

        #region Dir Toggles  
        private Visibility upToggle;
        public Visibility UpToggle
        {
            get { return upToggle; }
            set
            {
                if (value != upToggle)
                {
                    upToggle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Visibility downToggle;
        public Visibility DownToggle
        {
            get { return downToggle; }
            set
            {
                if (value != downToggle)
                {
                    downToggle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Visibility leftToggle;
        public Visibility LeftToggle
        {
            get { return leftToggle; }
            set
            {
                if (value != leftToggle)
                {
                    leftToggle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private Visibility rightToggle;
        public Visibility RightToggle
        {
            get { return rightToggle; }
            set
            {
                if (value != rightToggle)
                {
                    rightToggle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

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


        private SolidColorBrush[][] snakeGameProx = new SolidColorBrush[20][];
        private SolidColorBrush[][] snakeGame = new SolidColorBrush[20][];
        public SolidColorBrush[][] SnakeGame
        {
            get
            {
                return snakeGame;
            }
            set
            {
                if (value != snakeGame)
                {
                    this.snakeGame = value;
                    NotifyPropertyChanged();
                }
            }
        }


        public string DisplayedImage
        {
            get { return "/img/BackgroundInvert.jpg"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
