using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace EEGfront
{
    class TrainingInputManager :ISerializable
    {
        void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
        {

        }

        List<Tuple<double[], DateTime>> Up { get; set; }
        List<Tuple<double[], DateTime>> Down { get; set; }
        List<Tuple<double[], DateTime>> Left { get; set; }
        List<Tuple<double[], DateTime>> Right { get; set; }

        public TrainingInputManager()
        {
            Up = new List<Tuple<double[], DateTime>>();
            Down = new List<Tuple<double[], DateTime>>();
            Left = new List<Tuple<double[], DateTime>>();
            Right = new List<Tuple<double[], DateTime>>();
        }

        public void AddUp(double[] ProcessedSample, DateTime timestamp)
        {

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
