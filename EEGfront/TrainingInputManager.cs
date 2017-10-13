using System;
using System.Collections.Generic;

namespace EEGfront
{
    [Serializable]
    public class TrainingInputManager
    {

        public List<Tuple<Queue<double>[], DateTime>> Up { get; set; }
        public List<Tuple<Queue<double>[], DateTime>> Down { get; set; }
        public List<Tuple<Queue<double>[], DateTime>> Left { get; set; }
        public List<Tuple<Queue<double>[], DateTime>> Right { get; set; }

        public TrainingInputManager()
        {
            Up = new List<Tuple<Queue<double>[], DateTime>>();
            Down = new List<Tuple<Queue<double>[], DateTime>>();
            Left = new List<Tuple<Queue<double>[], DateTime>>();
            Right = new List<Tuple<Queue<double>[], DateTime>>();
        }

        public void AddUp(Queue<double>[] ProcessedSample, DateTime timestamp)
        {
            Up.Add(new Tuple<Queue<double>[], DateTime>(ProcessedSample, timestamp));
        }
        public void AddDown(Queue<double>[] ProcessedSample, DateTime timestamp)
        {
            Down.Add(new Tuple<Queue<double>[], DateTime>(ProcessedSample, timestamp));
        }
        public void AddLeft(Queue<double>[] ProcessedSample, DateTime timestamp)
        {
            Left.Add(new Tuple<Queue<double>[], DateTime>(ProcessedSample, timestamp));
        }
        public void AddRight(Queue<double>[] ProcessedSample, DateTime timestamp)
        {
            Right.Add(new Tuple<Queue<double>[], DateTime>(ProcessedSample, timestamp));
        }
    }
}
