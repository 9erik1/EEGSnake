using OxyPlot;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace EEGfront
{
    public class StatsViewModel : INotifyPropertyChanged
    {
        private bool isDraw = true;
        Thread draw;
        private DataProcessingPlant mathServ;



        private EmotiveAquisition stream;
        private IList<DataPoint>[] points;
        public ObservableCollection<DataPoint>[] Points
        {
            get { return (ObservableCollection<DataPoint>[])this.points; }
            set
            {
                if (value != this.points)
                {
                    this.points = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public enum ScienceState
        {
            Raw,
            ButtersWorth,
            Pca,
            PcaComponents,
            FFT,
            Custom,
            SavedRaw
        }

        public StatsViewModel()
        {
            mathServ = DataProcessingPlant.Instance;
            stream = EmotiveAquisition.Instance;

            points = new ObservableCollection<DataPoint>[4];
            points[0] = new ObservableCollection<DataPoint>();
            points[1] = new ObservableCollection<DataPoint>();
            points[2] = new ObservableCollection<DataPoint>();
            points[3] = new ObservableCollection<DataPoint>();

            scienceViews = new ObservableCollection<ScienceState>();
            scienceViews.Add(ScienceState.Raw);
            scienceViews.Add(ScienceState.ButtersWorth);
            scienceViews.Add(ScienceState.Pca);
            scienceViews.Add(ScienceState.PcaComponents);
            scienceViews.Add(ScienceState.FFT);
            scienceViews.Add(ScienceState.Custom);
            scienceViews.Add(ScienceState.SavedRaw);

            draw = new Thread(new ThreadStart(Draw));
            draw.Start();

            Title = new string[4];
            Title[0] = "AF3";
            Title[1] = "AF4";
            Title[2] = "O1";
            Title[3] = "O2";

            OxyAxisLabels = new string[Title.Length * 2];
            OxyAxisLabels[0] = "Frequency (Hz)";       // 1st x-axis            
            OxyAxisLabels[1] = "Signal (ADUs)";        // 1st y-axis            
            OxyAxisLabels[2] = "Frequency (Hz)";       // 2nd x-axis            
            OxyAxisLabels[3] = "Signal (ADUs)";        // 2nd y-axis            
            OxyAxisLabels[4] = "Frequency (Hz)";       // 3rd x-axis            
            OxyAxisLabels[5] = "Signal (ADUs)";        // 3rd y-axis            
            OxyAxisLabels[6] = "Frequency (Hz)";       // 4th x-axis
            OxyAxisLabels[7] = "Signal (ADUs)";        // 4th y-axis

            mathServ = DataProcessingPlant.Instance;

            CompositionTarget.Rendering += (s, a) =>
            {
                fpswpfVal++;
            };

            DispatcherTimer fpsTimer = new DispatcherTimer();
            fpsTimer.Interval = TimeSpan.FromSeconds(1);
            fpsTimer.Tick += (s, a) =>
            {
                //m_fpsText.Text = string.Format("FPS:{0}", m_frames);
                fpswpf = string.Format("WPF FPS:{0}", fpswpfVal);
                FPSWPF = fpswpf;
                fpswpfVal = 0;
                isDongleConnected = string.Format("Dongle Connection: {0}", stream.isConnected);
                IsDongleConnected = isDongleConnected;
                isConnected = string.Format("Emotive Connection: {0}", stream.isActive);
                IsConnected = isConnected;
                svmClass = string.Format("SVM");
                SVMClass = svmClass;
            };
            fpsTimer.Start();
        }

        private void DrawFirstPlot()
        {
            points[0].Clear();
            var x = stream.DataWindow[0].ToArray();
            //x = mathServ.BW_hi_5(x);
            for (int i = 0; i < x.Length - 1; i++)
            {
                points[0].Add(new DataPoint(i, x[i]));
            }
        }

        private void DrawDataWindow()
        {
            Queue<double>[] data = stream.DataWindow;
            int i = data.Length - 1;
            foreach (Queue<double> dw in data)
            {
                points[i].Clear();

                double[] rawData = dw.ToArray();

                if (currentScienceState == ScienceState.ButtersWorth)
                    rawData = mathServ.BW_hi_5(rawData);
                else if (currentScienceState == ScienceState.FFT)
                {
                    rawData = mathServ.BW_hi_5(rawData);
                    rawData = mathServ.Conversion_fft(rawData);
                }

                int j = 0;
                foreach (double d in rawData)
                {
                    if (currentScienceState == ScienceState.FFT)
                        points[i].Add(new DataPoint(stream.FrequencyBins[j], d));
                    else
                        points[i].Add(new DataPoint(j, d));
                    j++;
                }
                i--;
            }
        }

        private async void Draw()
        {
            while (isDraw)
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    DrawDataWindow();
                }));
                await Task.Delay(250);
            }
        }

        public void Shutdown()
        {
            isDraw = false;
        }

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

        private ScienceState currentScienceState;
        public ScienceState CurrentScienceState
        {
            get { return currentScienceState; }
            set
            {
                if (value != currentScienceState)
                {
                    this.currentScienceState = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private IList<ScienceState> scienceViews;
        public ObservableCollection<ScienceState> ScienceViews
        {
            get { return (ObservableCollection<ScienceState>)scienceViews; }
            set
            {
                if (value != scienceViews)
                {
                    this.scienceViews = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private int fpswpfVal;

        private string fpswpf;
        public string FPSWPF
        {
            get { return fpswpf; }
            set
            {
                fpswpf = value;
                NotifyPropertyChanged();
            }
        }

        private string isDongleConnected;
        public string IsDongleConnected
        {
            get { return isDongleConnected; }
            set
            {
                isDongleConnected = value;
                NotifyPropertyChanged();
            }
        }

        private string isConnected;
        public string IsConnected
        {
            get { return isConnected; }
            set
            {
                isConnected = value;
                NotifyPropertyChanged();
            }
        }

        private string svmClass;
        public string SVMClass
        {
            get { return svmClass; }
            set
            {
                svmClass = value;
                NotifyPropertyChanged();
            }
        }

        public string DisplayedImage
        {
            get { return "/img/Background.jpg"; }
        }

        public string[] Title { get; private set; }
        public string[] OxyAxisLabels { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
