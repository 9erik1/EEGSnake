using Emotiv;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace EEGfront
{
    public class EmotiveAquisition
    {
        private bool IsDraw = true;
        public void Kill()
        {
            IsDraw = false;
        }
        public void ReverseKill()
        {
            IsDraw = true;
        }

        private static EmotiveAquisition instance;
        public static EmotiveAquisition Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new EmotiveAquisition();
                }
                return instance;
            }
        }

        private double sampleFrequency = 128d;
        public double SampleFrequency
        {
            get { return sampleFrequency; }
        }

        private double[] frequencyBins;
        public double[] FrequencyBins
        {
            get { return frequencyBins; }
        }

        private Dictionary<EdkDll.EE_DataChannel_t, double[]> dataAquired;
        private int fps;
        private double N;//Max
        private Thread update;
        private EmoEngine engine;
        //All view modles have access to this
        private Queue<double>[] dataWindow;
        public Queue<double>[] DataWindow
        {
            get { return dataWindow.Clone() as Queue<double>[]; }
        }
        public bool isConnected;
        public bool isActive;
        public int fpsDes;

        private EmotiveAquisition()
        {
            Console.WriteLine("EMO Engine Init");

            isConnected = false;
            isActive = false;

            double f_s = 128;
            N = 4 * f_s;
            frequencyBins = new double[(int)N / 2];
            for (int k = 0; k < frequencyBins.Length; k++)
                frequencyBins[k] = k * f_s / N;

            engine = EmoEngine.Instance;
            engine.EmoEngineConnected += Engine_EmoEngineConnected;
            engine.EmoEngineDisconnected += Engine_EmoEngineDisconnected;
            engine.Connect();
            dataWindow = new Queue<double>[4];
            for (int j = 0; j < dataWindow.Count(); j++)
                dataWindow[j] = new Queue<double>();
            for (int i = 0; i < dataWindow.Count(); i++)
            {
                for (int k = 0; k < N; k++)
                {
                    dataWindow[i].Enqueue(0);
                }
            }

            DispatcherTimer fpsTimer = new DispatcherTimer();
            fpsTimer.Interval = TimeSpan.FromSeconds(1);
            fpsTimer.Tick += (s, a) =>
            {
                fpsDes = fps;
                fps = 0;
            };
            fpsTimer.Start();

        }

        private void Engine_EmoEngineDisconnected(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("EMO Engine Disconnected");
            isConnected = false;
            isActive = false;
        }

        private async void Engine_EmoEngineConnected(object sender, EmoEngineEventArgs e)
        {
            Console.WriteLine("EMO Engine Connected");
            Console.WriteLine("EMO Engine DataAcquisition Attempt");
            await Task.Delay(5000);
            try
            {
                engine.DataAcquisitionEnable(0, true);
                await Task.Delay(2000);
                if (update == null)
                {
                    update = new Thread(new ThreadStart(Update));
                    update.Start();
                }

                Console.WriteLine("EMO Engine DataAcquisition Success");
                isConnected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("EMO Engine DataAquisition Fail" + " | Exception: " + ex.Message);
                update = new Thread(new ThreadStart(SimulationUpdate));
                update.Start();
            }
        }

        private void SimulationUpdate()
        {
            double amplitude = 0.1d;
            double dataPoints = N;

            for (int i = 0; i < N; i++)
            {
                dataWindow[0].Enqueue(amplitude * Math.Sin(5 * 2 * Math.PI * i / sampleFrequency));
                if (dataWindow[0].Count > N)
                    dataWindow[0].Dequeue();
            }

            for (int i = 0; i < N; i++)
            {
                dataWindow[1].Enqueue(amplitude * Math.Sin(10 * 2 * Math.PI * i / sampleFrequency));
                if (dataWindow[1].Count > N)
                    dataWindow[1].Dequeue();
            }

            for (int i = 0; i < N; i++)
            {
                dataWindow[2].Enqueue(amplitude * Math.Sin(123 * 2 * Math.PI * i / sampleFrequency));
                if (dataWindow[2].Count > N)
                    dataWindow[2].Dequeue();
            }

            for (int i = 0; i < N; i++)
            {
                dataWindow[3].Enqueue(amplitude * Math.Sin(64 * 2 * Math.PI * i / sampleFrequency));
                if (dataWindow[3].Count > N)
                    dataWindow[3].Dequeue();
            }
        }

        private async void Update()
        {
            while (IsDraw)
            {
                bool pass;
                try
                {
                    dataAquired = engine.GetData(0);
                    double[] AF3 = dataAquired[EdkDll.EE_DataChannel_t.AF3];
                    double[] AF4 = dataAquired[EdkDll.EE_DataChannel_t.AF4];
                    double[] O1 = dataAquired[EdkDll.EE_DataChannel_t.O1];
                    double[] O2 = dataAquired[EdkDll.EE_DataChannel_t.O2];
                    pass = true;
                    isActive = true;
                    fps++;
                    //Console.WriteLine(fps);
                }
                catch (NullReferenceException)
                {
                    pass = false;
                    isActive = false;
                    Console.WriteLine("Live Raw Packet Request Failed; Null Reference Exception");
                }
                catch
                {
                    pass = false;
                    isConnected = false;
                    engine.Connect();
                    Console.WriteLine("Live Raw Packet Request Failed Dong");
                }

                if (pass)
                {
                    foreach (double point in dataAquired[EdkDll.EE_DataChannel_t.AF3])
                    {
                        dataWindow[0].Enqueue(point);
                        if (dataWindow[0].Count > N)
                            dataWindow[0].Dequeue();
                    }
                    foreach (double point in dataAquired[EdkDll.EE_DataChannel_t.AF4])
                    {
                        dataWindow[1].Enqueue(point);
                        if (dataWindow[1].Count > N)
                            dataWindow[1].Dequeue();
                    }
                    foreach (double point in dataAquired[EdkDll.EE_DataChannel_t.O1])
                    {
                        dataWindow[2].Enqueue(point);
                        if (dataWindow[2].Count > N)
                            dataWindow[2].Dequeue();
                    }
                    foreach (double point in dataAquired[EdkDll.EE_DataChannel_t.O2])
                    {
                        dataWindow[3].Enqueue(point);
                        if (dataWindow[3].Count > N)
                            dataWindow[3].Dequeue();
                    }
                    foreach (Queue<double> windough in dataWindow)
                    {
                        if (windough.Count > N)
                            windough.Dequeue();
                    }
                }
                await Task.Delay(250);
            }
        }
    }
}