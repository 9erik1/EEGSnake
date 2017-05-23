using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace EEGfront
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool IsDraw = true;
        Thread draw;

        private EmotiveAquisition stream;

        public MainViewModel()
        {
            stream = EmotiveAquisition.Instance;

            draw = new Thread(new ThreadStart(Draw));
            draw.Start();
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

       
        public string[] Title { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
