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
using System.Runtime.Serialization.Formatters.Binary;
using Accord.Statistics.Analysis;
using System.Linq;
using Accord.Math;
using System.Numerics;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;

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
        private Rest restService;
        private GLControl graphics;

        //private Cloud rest;


        public MainViewModel(string idTag)
        {

            //rest = Cloud.Instance;
            restService = Rest.Instance;
            stream = EmotiveAquisition.Instance;

            // Example 
            //await restService.Get("https://192.168.0.173:5900/rest/");
            //await restService.PostCurrent("8");
            //await restService.PostPrev("8");

            Console.WriteLine("user id success in view model: " + idTag);

            Dir = 0;
            Trials = 0;
            // because we use untrusted ssl ;)
            ServicePointManager.ServerCertificateValidationCallback += (sender, certificate, chain, sslPolicyErrors) => true;

            //using of the login module



            //stream = EmotiveAquisition.Instance;

            menuItems = new ObservableCollection<AppState>();
            menuItems.Add(AppState.Game);
            menuItems.Add(AppState.Train);
            menuItems.Add(AppState.Stats);

            currentOption = AppState.Game;
            CurrentOptionChange(currentOption);

            MenuItem.CollectionChanged += MenuItem_CollectionChanged;

            gameToggle = Visibility.Visible;
            upToggle = Visibility.Visible;
            downToggle = Visibility.Visible;
            leftToggle = Visibility.Visible;
            rightToggle = Visibility.Visible;

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
                UpToggle = Visibility.Hidden;
                DownToggle = Visibility.Hidden;
                LeftToggle = Visibility.Hidden;
                RightToggle = Visibility.Hidden;
            }
            if (s == AppState.Game)
            {
                GameToggle = Visibility.Visible;
                TrainToggle = Visibility.Hidden;
                UpToggle = Visibility.Visible;
                DownToggle = Visibility.Visible;
                LeftToggle = Visibility.Visible;
                RightToggle = Visibility.Visible;
            }
        }

        private async void Clickey()
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

                // filter, pca
                PrincipalComponentMethod pca_method = PrincipalComponentMethod.Center;
                PrincipalComponentAnalysis pcaLib = new PrincipalComponentAnalysis(pca_method);

                var pca = new double[4][];
                pca[0] = stream.dataWindow[0].ToArray();
                pca[1] = stream.dataWindow[1].ToArray();
                pca[2] = stream.dataWindow[2].ToArray();
                pca[3] = stream.dataWindow[3].ToArray();

                for (int p = 0; p < pca.Count(); p++)
                    pca[p] = BW_hi_5(pca[p]);

                // Apply PCA
                var x = pcaLib.Learn(pca.Transpose());
                double[][] actual = pcaLib.Transform(pca.Transpose());


                // Apply Reverse PCA to Time-Series
                double[][] removedComp = new double[2][];
                removedComp[0] = pcaLib.ComponentVectors[0];
                removedComp[1] = pcaLib.ComponentVectors[1];

                actual = actual.Dot(pcaLib.ComponentVectors);
                actual = actual.Transpose();


                // Apply FFT
                Complex[][] transformed_data = new Complex[actual.Count()][];
                for (int f = 0; f < actual.Count(); f++)
                {
                    transformed_data[f] = new Complex[actual[f].Count()];
                    transformed_data[f] = Conversion_fft(actual[f]);
                }

                double[][] inputs =
{
    //               input         output
    actual[0], //  0 
    actual[0], //  0




    actual[3], //  2
    actual[3], //  2
    actual[3], //  2
    actual[3],//2
    actual[3],
    actual[3],//  2
    actual[3], //  2
    actual[3], //  2
    actual[3], //  2
    actual[3],//2
    actual[3],
    actual[3],//  2
    actual[3], //  2
    actual[3], //  2
    actual[3], //  2
    actual[3],//2
    actual[3],
    actual[3],//  2
    actual[3], //  2
    actual[3], //  2
    actual[3], //  2
    actual[3],//2
    actual[3],
    actual[3]//  2
};
                int[] outputs = // those are the class labels
{
    0, 0,

    1, 1, 1, 1, 1,1,1, 1, 1, 1, 1,1,1, 1, 1, 1, 1,1,1, 1, 1, 1, 1,1
};

                double[][] testinput =
{
                //               input         output
    actual[3], //  2
    actual[0], //  0 
    actual[0], //  0
    actual[0], //  0
    actual[0], //  0

    actual[3], //  2

    actual[3], //  2
    actual[3] //  0

 };

                var teacher = new MulticlassSupportVectorLearning<Gaussian>()
                {
                    // Configure the learning algorithm to use SMO to train the
                    //  underlying SVMs in each of the binary class subproblems.
                    Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                    {
                        // Estimate a suitable guess for the Gaussian kernel's parameters.
                        // This estimate can serve as a starting point for a grid search.
                        UseKernelEstimation = true
                    }
                };

                // Learn a machine
                var machine = teacher.Learn(inputs, outputs);


                // Create the multi-class learning algorithm for the machine
                var calibration = new MulticlassSupportVectorLearning<Gaussian>()
                {
                    Model = machine, // We will start with an existing machine

                    // Configure the learning algorithm to use SMO to train the
                    //  underlying SVMs in each of the binary class subproblems.
                    Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>()
                    {
                        Model = param.Model // Start with an existing machine
                    }
                };


                // Configure parallel execution options
                calibration.ParallelOptions.MaxDegreeOfParallelism = 1;

                // Learn a machine
                calibration.Learn(inputs, outputs);

                // Obtain class predictions for each sample
                int[] predicted = machine.Decide(testinput);

                Console.WriteLine(predicted);



                //serilization of data
                Stream DataWindowStream = new MemoryStream();
                BinaryFormatter serializer = new BinaryFormatter();
                serializer.Serialize(DataWindowStream, stream);
                await restService.UpdateModel("8", DataWindowStream);//post call
                DataWindowStream.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed for basic reason: " + e);
            }
        }

        private async void AutoTest()
        {
            try
            {

                Console.WriteLine("Auto Test");
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed for basic reason: " + e);
            }
        }

        private void makedowaves()
        {

        }


        #region filters

        /////////////////////////////////////////////////////
        ///// CONVERT DOUBLE TO COMPLEX, THEN FFT ///////////
        ///// Currently only uses BW_hi_5 ///////////////////
        /////////////////////////////////////////////////////
        private Complex[] Conversion_fft(double[] raw_proxy)
        {
            //var p = raw; // redundant
            //Complex[] complex = new Complex[p.Count()]; // redundant

            Complex[] complex_raw_proxy = new Complex[raw_proxy.Count()];  // INITIALIZING FOR CONVERSION OF THE double INPUT TO a Complex variable

            ///// CONVERT TO COMPLEX VALUES /////
            int buffer_window = 0;                                                  // ignoring this many data points due to startup of data acquisition (not necessary for sim data)
            for (int j = buffer_window; j < complex_raw_proxy.Count(); j++)
            {
                //complex_raw[j] = new Complex(p[j], 0); // redundant
                complex_raw_proxy[j] = new Complex(raw_proxy[j], 0);
            }
            Accord.Math.Transforms.FourierTransform2.FFT(complex_raw_proxy, FourierTransform.Direction.Forward);

            return complex_raw_proxy;
        }

        /////////////////////////////////////////////////////
        ///// BUTTERWORTH HIGH PASS FILTER, f_c = 5 Hz //////
        ///// References: [1], [2] //////////////////////////
        /////////////////////////////////////////////////////
        public double[] BW_hi_5(double[] dd8)
        {
            double GAIN = 1.379370774e+00;
            double[] xv, yv;
            xv = new double[5];
            yv = new double[5];

            double[] dd7 = dd8;

            for (int i = 0; i < dd8.Count(); i++)
            {
                xv[0] = xv[1];
                xv[1] = xv[2];
                xv[2] = xv[3];
                xv[3] = xv[4];
                xv[4] = dd8[i] / GAIN;
                yv[0] = yv[1];
                yv[1] = yv[2];
                yv[2] = yv[3];
                yv[3] = yv[4];
                yv[4] = (xv[0] + xv[4]) - 4 * (xv[1] + xv[3]) + 6 * xv[2]
                             + (-0.5255789745 * yv[0]) + (2.4389986574 * yv[1])
                             + (-4.2755090771 * yv[2]) + (3.3594051013 * yv[3]);

                dd7[i] = yv[4];
            }

            return dd7;

        }

        #endregion

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

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
