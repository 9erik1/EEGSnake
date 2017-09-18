using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace EEGfront
{
    [Serializable]
    public class TrainingInputManager 
    {

        public List<Tuple<double[], DateTime>> Up { get; set; }
        public List<Tuple<double[], DateTime>> Down { get; set; }
        public List<Tuple<double[], DateTime>> Left { get; set; }
        public List<Tuple<double[], DateTime>> Right { get; set; }

        public TrainingInputManager()
        {
            Up = new List<Tuple<double[], DateTime>>();
            Down = new List<Tuple<double[], DateTime>>();
            Left = new List<Tuple<double[], DateTime>>();
            Right = new List<Tuple<double[], DateTime>>();
        }

        public void AddUp(double[] ProcessedSample, DateTime timestamp)
        {
            Up.Add(new Tuple<double[], DateTime>(ProcessedSample, timestamp));
        }
        public void AddDown(double[] ProcessedSample, DateTime timestamp)
        {

        }
        public void AddLeft(double[] ProcessedSample, DateTime timestamp)
        {

        }
        public void AddRight(double[] ProcessedSample, DateTime timestamp)
        {

        }
    }
}
