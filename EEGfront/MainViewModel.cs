using System.Threading.Tasks;
using System.Threading;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Input;
using System;
using OpenTK;

namespace EEGfront
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private bool IsDraw = true;
        Thread draw;

        private EmotiveAquisition stream;
        private GLControl graphics;


        public MainViewModel()
        {
            //stream = EmotiveAquisition.Instance;

            menuItems = new ObservableCollection<string>();
            menuItems.Add("Game");
            menuItems.Add("Training");
            menuItems.Add("Data");

            currentOption = "Game";

            MenuItem.CollectionChanged += MenuItem_CollectionChanged;

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

        private void CurrentOptionChange(string s)
        {
            Console.WriteLine(s);
        }

        public string[] Title { get; private set; }
        //private ICommand optionChanged;
        //public ICommand OptionChanged
        //{
        //    get
        //    {
        //        if (optionChanged == null)
        //        {
        //            optionChanged = new RelayCommand(
        //                p => true,
        //                p => this.CurrentOptionChange(optionChanged));
        //        }
        //        return optionChanged;
        //    }
        //}
        private string currentOption;
        public string CurrentOption
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
        private IList<string> menuItems;
        public ObservableCollection<string> MenuItem
        {
            get
            {
                return (ObservableCollection<string>)menuItems;
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
