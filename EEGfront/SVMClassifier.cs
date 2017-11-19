using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Statistics.Kernels;
using System;
using System.Collections.Generic;

namespace EEGfront
{
    public class SVMClassifier
    {
        public MulticlassSupportVectorMachine<Gaussian> Learn { get; set; }
        private DataProcessingPlant mathServ;
        public SVMClassifier()
        {
            mathServ = DataProcessingPlant.Instance;
        }

        /// <summary>UpdateSVM is a method in the SVMClassifier class.
        /// <para>Takes rawStream and learns a guassian multi class Support Vector Machine</para>
        /// <seealso cref="SVMClassifier.cs"/>
        /// </summary>
        public void UpdateSVM(TrainingInputManager tim)//Know your output
        {
            var w = tim.Up;
            var x = tim.Right;
            var y = tim.Down;
            var z = tim.Left;
            List<int> outs = new List<int>();
            List<double[]> inns = new List<double[]>();


            foreach (Tuple<Queue<Double>[], DateTime> qd in w)
            {
                Queue<Double>[] aggregateData = mathServ.ApplyPCAue(qd.Item1);
                aggregateData = mathServ.Conversion_fft(aggregateData);
                outs.Add(0);
                outs.Add(0);
                inns.Add(aggregateData[1].ToArray());
                inns.Add(aggregateData[2].ToArray());
            }

            foreach (Tuple<Queue<Double>[], DateTime> qd in x)
            {
                Queue<Double>[] aggregateData = mathServ.ApplyPCAue(qd.Item1);
                aggregateData = mathServ.Conversion_fft(aggregateData);
                outs.Add(1);
                outs.Add(1);
                inns.Add(aggregateData[1].ToArray());
                inns.Add(aggregateData[2].ToArray());
            }

            foreach (Tuple<Queue<Double>[], DateTime> qd in y)
            {
                Queue<Double>[] aggregateData = mathServ.ApplyPCAue(qd.Item1);
                aggregateData = mathServ.Conversion_fft(aggregateData);
                outs.Add(2);
                outs.Add(2);
                inns.Add(aggregateData[1].ToArray());
                inns.Add(aggregateData[2].ToArray());
            }

            foreach (Tuple<Queue<Double>[], DateTime> qd in z)
            {
                Queue<Double>[] aggregateData = mathServ.ApplyPCAue(qd.Item1);
                aggregateData = mathServ.Conversion_fft(aggregateData);
                outs.Add(3);
                outs.Add(3);
                inns.Add(aggregateData[1].ToArray());
                inns.Add(aggregateData[2].ToArray());
            }

            MulticlassSupportVectorLearning<Gaussian> teacher = new MulticlassSupportVectorLearning<Gaussian>()
            {
                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new SequentialMinimalOptimization<Gaussian>()
                {
                    // Estimate a suitable guess for the Gaussian kernel's parameters.
                    // This estimate can serve as a starting point for a grid search.
                    UseKernelEstimation = true
                }
            };


            Learn = teacher.Learn(inns.ToArray(), outs.ToArray());

        }
        /// <summary>AnswerSVM is a method in the SVMClassifier class.
        /// <para>Takes rawStream and outputs class label as an array of ints.</para>
        /// <seealso cref="SVMClassifier.cs"/>
        /// </summary>
        public int[] AnswerSVM(Queue<Double>[] rawStream)
        {
            Queue<Double>[] aggregateData = mathServ.ApplyPCAue(rawStream);
            aggregateData = mathServ.Conversion_fft(aggregateData);
            double[][] testinput =
            {
                aggregateData[1].ToArray(),
                aggregateData[2].ToArray()
            };
            int[] predicted = Learn.Decide(testinput);
            return predicted;
        }
    }
}
