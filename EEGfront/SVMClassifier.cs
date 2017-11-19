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
        public void UpdateSVM(Queue<Double>[] rawStream, int output)//Know your output
        {
            Queue<Double>[] aggregateData = mathServ.ApplyPCAue(rawStream);
            aggregateData = mathServ.Conversion_fft(aggregateData);
            double[][] inputs =
            {
                aggregateData[1].ToArray(),
                aggregateData[2].ToArray(),
                aggregateData[3].ToArray()
            };
            int[] outputs =
            {
                output,
                output,
                1
            };

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


            Learn = teacher.Learn(inputs, outputs);

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
