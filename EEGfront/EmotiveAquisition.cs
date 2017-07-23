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

        private Dictionary<EdkDll.EE_DataChannel_t, double[]> dataAquired;
        private int fps;
        private double N;
        private Thread update;
        private EmoEngine engine;
        //All view modles have access to this
        public Queue<double>[] dataWindow;
        public bool isConnected;
        public bool isActive;
        public int fpsDes;

        private EmotiveAquisition()
        {
            Console.WriteLine("EMO Engine Ini");
            isConnected = false;
            isActive = false;

            double f_s = 128;
            N = 16 * f_s;

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
            Console.WriteLine("EMO Engine DataAquisition Attempt");
            await Task.Delay(5000);
            try
            {
                engine.DataAcquisitionEnable(0, true);
                await Task.Delay(2000);
                if(update==null)
                {
                    update = new Thread(new ThreadStart(Update));
                    update.Start();
                }
                
                Console.WriteLine("EMO Engine DataAquisition Success");
                isConnected = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("EMO Engine DataAquisition Fail" + " | Exteption: " + ex.Message);
            }
        }

        private async void Update()
        {
            while (true)
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
                catch (NullReferenceException nulley)
                {
                    pass = false;
                    isActive = false;
                    Console.WriteLine("Live Raw Packet Request Faield Nulley");
                }
                catch
                {
                    pass = false;
                    isConnected = false;
                    engine.Connect();
                    Console.WriteLine("Live Raw Packet Request Faield Dong");
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
                    foreach(Queue<double> windough in dataWindow)
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