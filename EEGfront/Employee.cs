
using Accord.MachineLearning.VectorMachines;
using Accord.Statistics.Kernels;
using EEGfront;
using System;
using System.Runtime.Serialization;

namespace MyObjSerial
{
    [Serializable()]    //Set this attribute to all the classes that want to serialize
    public class Employee : ISerializable //derive your class from ISerializable
    {
        public int EmpId;
        public string EmpName;

        public int Count;
        public int NumberOfInputs;
        public int NumberOfOutputs;


        public Object Learn;

        //Default constructor
        public Employee()
        {
            Count = 0;
            NumberOfOutputs = 0;
            NumberOfInputs = 0;
            Learn = null;


            EmpId = 0;
            EmpName = null;
       
        }

        //Deserialization constructor.
        public Employee(SerializationInfo info, StreamingContext ctxt)
        {
            Learn = (Object)info.GetValue("Learn", typeof(Object));
            Count = (int)info.GetValue("Count", typeof(int));
            NumberOfOutputs = (int)info.GetValue("NumberOfOutputs", typeof(int));
            NumberOfInputs = (int)info.GetValue("NumberOfInputs", typeof(int));
            //Get the values from info and assign them to the appropriate properties
            EmpId = (int)info.GetValue("EmployeeId", typeof(int));
            EmpName = (String)info.GetValue("EmployeeName", typeof(string));
        }

        //Serialization function.
        public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
        {
            info.AddValue("Learn", Learn);
            info.AddValue("Count", Count);
            info.AddValue("NumberOfOutputs", NumberOfOutputs);
            info.AddValue("NumberOfInputs", NumberOfInputs);
            //You can use any custom name for your name-value pair. But make sure you
            // read the values with the same name. For ex:- If you write EmpId as "EmployeeId"
            // then you should read the same with "EmployeeId"
            info.AddValue("EmployeeId", EmpId);
            info.AddValue("EmployeeName", EmpName);
        }
    }
}