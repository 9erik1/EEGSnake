using GateKeep;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;

namespace EEGfront
{
    public class StatsViewModel : INotifyPropertyChanged
    {

        public StatsViewModel()
        {

        }

        public void Shutdown()
        {

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

        public string DisplayedImage
        {
            get { return "/img/Background.jpg"; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
