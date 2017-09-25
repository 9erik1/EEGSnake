using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace EEGfront
{
    public class StatsViewModel : INotifyPropertyChanged
    {
        private EmotiveAquisition stream;

        public enum ScienceState
        {
            Raw,
            ButtersWorth,
            Pca,
            PcaComponents,
            FFT,
            Custom
        }

        public StatsViewModel()
        {
            stream = EmotiveAquisition.Instance;

            scienceViews = new ObservableCollection<ScienceState>();
            scienceViews.Add(ScienceState.Raw);
            scienceViews.Add(ScienceState.ButtersWorth);
            scienceViews.Add(ScienceState.Pca);
            scienceViews.Add(ScienceState.PcaComponents);
            scienceViews.Add(ScienceState.FFT);
            scienceViews.Add(ScienceState.Custom);
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

        private IList<ScienceState> scienceViews;
        public ObservableCollection<ScienceState> ScienceViews
        {
            get
            {
                return (ObservableCollection<ScienceState>)scienceViews;
            }
            set
            {
                if (value != scienceViews)
                {
                    this.scienceViews = value;
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
