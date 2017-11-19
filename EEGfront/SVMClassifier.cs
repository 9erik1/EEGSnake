
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math;
using Accord.Statistics.Analysis;
using Accord.Statistics.Kernels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

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
            double[][] aggregateData = mathServ.ApplyPCArr(rawStream);
            double[][] inputs =
            {
                aggregateData[1],
                aggregateData[2],
                aggregateData[3]
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

            if (Learn == null)
            {
                Learn = teacher.Learn(inputs, outputs);
            }
            else
            {
                //Learn = Learn.Concatenate(teacher.Learn(inputs, outputs))[0];
            }
        }
        /// <summary>AnswerSVM is a method in the SVMClassifier class.
        /// <para>Takes rawStream and outputs class label as an array of ints.</para>
        /// <seealso cref="SVMClassifier.cs"/>
        /// </summary>
        public int[] AnswerSVM(Queue<Double>[] rawStream)
        {
            double[][] aggregateData = mathServ.ApplyPCArr(rawStream);
            double[][] testinput =
            {
                aggregateData[1],
                aggregateData[2]
            };
            int[] predicted = Learn.Decide(testinput);
            return predicted;
        }
    }
}
