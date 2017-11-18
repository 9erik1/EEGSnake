﻿using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using System.Windows;
using System.IO;
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Accord.IO;
using System.Windows.Media;

namespace EEGfront
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private string secretidtag = "";
        //these max values are hard coded into xaml grid and can not be changed
        //in future remove reliance on hard coded xaml
        private static readonly int XMAX = 20;
        private static readonly int YMAX = 20;


        public enum AppState
        {
            Game,
            Train,
            Stats
        }

        //The collected raws for the current user
        TrainingInputManager T_I_M;

        private bool IsDraw = true;

        private EmotiveAquisition stream;
        private Rest restService;
        private SVMClassifier machineStudent;
        MulticlassSupportVectorMachine<Gaussian> currentClassifier;
        private Snake snakeLogic;

        public MainViewModel(string idTag)
        {
            snakeLogic = new Snake();
            restService = Rest.Instance;
            stream = EmotiveAquisition.Instance;
            stream.ReverseKill();
            machineStudent = new SVMClassifier();

            T_I_M = new TrainingInputManager();

            Task.Run(async () =>
            {
                T_I_M = await restService.PostCurrentRaw(idTag);
                Console.WriteLine(T_I_M);
                if (T_I_M == null)
                    T_I_M = new TrainingInputManager();
            });

            //Dispatcher.BeginInvoke((Action)(() =>
            //{
            //    Console.WriteLine("asd");
            //}));

            Task.Run(async () => currentClassifier = await restService.PostCurrent(idTag));

            // Example 
            //await restService.Get("https://zotac.mikedev.ca:5900/rest/");
            //await restService.PostCurrent("8");
            //await restService.PostPrev("8");

            Console.WriteLine("user id success in view model: " + idTag);

            Dir = 0;
            Trials = 0;

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

            for (int i = 0; i < XMAX; i++)
            {
                snakeGame[i] = new SolidColorBrush[XMAX];
                snakeGameProx[i] = new SolidColorBrush[YMAX];
            }

            for (int j = 0; j < XMAX; j++)
            {
                for (int k = 0; k < XMAX; k++)
                {
                    snakeGameProx[j][k] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));
                }
            }
            snakeGameProx[0][0] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

            var efrg = snakeLogic.GetApplePos();
            snakeGameProx[(int)efrg.X][(int)efrg.Y] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));
            SnakeGame = (SolidColorBrush[][])snakeGameProx.Clone();

            Task.Run(async () =>
            {
                currentClassifier = await restService.PostCurrent(idTag);
            });
        }

        public void LymeCanada()
        {
            snakeLogic.MoveSnake();
            snakeLogic.Detect();
            RedrawBoard();
        }

        private void RedrawBoard()
        {
            // repaints board red
            for (int j = 0; j < XMAX; j++)
                for (int k = 0; k < XMAX; k++)
                    snakeGameProx[j][k] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 0, 0));

            // draw snake
            foreach (SnakeCube v in snakeLogic.GetSnakeList())
                snakeGameProx[v.Y][v.X] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

            // draw apple
            var efrg = snakeLogic.GetApplePos();
            snakeGameProx[efrg.X][efrg.Y] = new SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 255, 255));

            // bind it to xaml
            SnakeGame = (SolidColorBrush[][])snakeGameProx.Clone();
        }

        public void Up()
        {
            Console.WriteLine("Up");
            snakeLogic.MoveUp();
            //Console.WriteLine("X: " + SnakeX + " Y: " + SnakeY + " AX: " + appleX + " AY: " + appleY);
        }

        public void Down()
        {
            Console.WriteLine("Down");
            snakeLogic.MoveDown();
           //Console.WriteLine("X: " + SnakeX + " Y: " + SnakeY + " AX: " + appleX + " AY: " + appleY);
        }

        public void Left()
        {
            Console.WriteLine("Left");
            snakeLogic.MoveLeft();
            //Console.WriteLine("X: " + SnakeX + " Y: " + SnakeY + " AX: " + appleX + " AY: " + appleY);
        }

        public void Right()
        {
            Console.WriteLine("Right");
            snakeLogic.MoveRight();
            //Console.WriteLine("X: " + SnakeX + " Y: " + SnakeY + " AX: " + appleX + " AY: " + appleY);
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

        private async void SnakeUpdate()
        {
            while (IsDraw)
            {
                snakeLogic.MoveSnake();
                RedrawBoard();
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
            machineStudent.UpdateSVM(stream.DataWindow, dir);

            var payload = new MemoryStream();

            Serializer.Save(machineStudent.Learn, payload);
            payload.Position = 0;
            await restService.UpdateModelRaw(secretidtag, payload);
            payload.Close();
        }
        private async void PackRaw(int dir)
        {
            IFormatter formatter = new BinaryFormatter();
            Stream s = new MemoryStream();

            switch (dir)
            {
                case 0:
                    T_I_M.AddUp(stream.DataWindow, DateTime.Now);
                    break;
                case 1:
                    T_I_M.AddRight(stream.DataWindow, DateTime.Now);
                    break;
                case 2:
                    T_I_M.AddDown(stream.DataWindow, DateTime.Now);
                    break;
                case 3:
                    T_I_M.AddLeft(stream.DataWindow, DateTime.Now);
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }

            formatter.Serialize(s, T_I_M);
            s.Position = 0;
            await restService.UpdateModelRaw(secretidtag, s);
            s.Close();
        }

        private void AutoTest()
        {
            try
            {
                //var x = await restService.PostCurrent("8");

                var y = machineStudent.AnswerSVM(stream.DataWindow);

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
