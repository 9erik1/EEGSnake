using Accord.MachineLearning;
using Accord.MachineLearning.VectorMachines;
using Accord.MachineLearning.VectorMachines.Learning;
using Accord.Math.Optimization.Losses;
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
                //outs.Add(0);
                inns.Add(mathServ.NormalizeData(aggregateData[1].ToArray(), 0, 1));
                //inns.Add(mathServ.NormalizeData(aggregateData[2].ToArray(), 0, 1));
            }

            foreach (Tuple<Queue<Double>[], DateTime> qd in x)
            {
                Queue<Double>[] aggregateData = mathServ.ApplyPCAue(qd.Item1);
                aggregateData = mathServ.Conversion_fft(aggregateData);
                outs.Add(1);
                //outs.Add(1);
                inns.Add(mathServ.NormalizeData(aggregateData[1].ToArray(), 0, 1));
                //inns.Add(mathServ.NormalizeData(aggregateData[2].ToArray(), 0, 1));
            }

            foreach (Tuple<Queue<Double>[], DateTime> qd in y)
            {
                Queue<Double>[] aggregateData = mathServ.ApplyPCAue(qd.Item1);
                aggregateData = mathServ.Conversion_fft(aggregateData);
                outs.Add(2);
                //outs.Add(2);
                inns.Add(mathServ.NormalizeData(aggregateData[1].ToArray(), 0, 1));
                //inns.Add(mathServ.NormalizeData(aggregateData[2].ToArray(), 0, 1));
            }

            foreach (Tuple<Queue<Double>[], DateTime> qd in z)
            {
                Queue<Double>[] aggregateData = mathServ.ApplyPCAue(qd.Item1);
                aggregateData = mathServ.Conversion_fft(aggregateData);
                outs.Add(3);
                //outs.Add(3);
                inns.Add(mathServ.NormalizeData(aggregateData[1].ToArray(), 0, 1));
                //inns.Add(mathServ.NormalizeData(aggregateData[2].ToArray(), 0, 1));
            }

            //experimental
            // Ensure results are reproducible
            Accord.Math.Random.Generator.Seed = 0;

            // Example binary data
            double[][] inputs =
            {
                new double[] { -1, -1 },
                new double[] { -1,  1 },
                new double[] {  1, -1 },
                new double[] {  1,  1 }
            };

            int[] xor = // xor labels
            {
                -1, 1, 1, -1
            };

            // Instantiate a new Grid Search algorithm for Kernel Support Vector Machines
            var gridsearch = new Accord.MachineLearning.Performance.GridSearch<SupportVectorMachine<Accord.Statistics.Kernels.Polynomial>, double[], int>()
            {
                // Here we can specify the range of the parameters to be included in the search
                ParameterRanges = new Accord.MachineLearning.GridSearchRangeCollection()
                {
                    new GridSearchRange("complexity", new double[] { 0.00000001, 5.20, 0.30, 0.50 } ),
                    new GridSearchRange("degree",     new double[] { 1, 10, 2, 3, 4, 5 } ),
                    new GridSearchRange("constant",   new double[] { 0, 1, 2 } )
                },

                // Indicate how learning algorithms for the models should be created
                Learner = (p) => new SequentialMinimalOptimization<Accord.Statistics.Kernels.Polynomial>
                {
                    Complexity = p["complexity"],
                    Kernel = new Accord.Statistics.Kernels.Polynomial((int)p["degree"], p["constant"])
                },

                // Define how the performance of the models should be measured
                Loss = (actual, expected, m) => new ZeroOneLoss(expected).Loss(actual)
            };

            // If needed, control the degree of CPU parallelization
            gridsearch.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Search for the best model parameters
            var result = gridsearch.Learn(inputs, xor);

            // Get the best SVM found during the parameter search
            SupportVectorMachine<Accord.Statistics.Kernels.Polynomial> svm = result.BestModel;

            // Get an estimate for its error:
            double bestError = result.BestModelError;

            // Get the best values found for the model parameters:
            double bestC = result.BestParameters["complexity"].Value;
            Console.WriteLine("bestC: "+ bestC + "------------");
            double bestDegree = result.BestParameters["degree"].Value;
            Console.WriteLine("bestDegree: " + bestDegree + "------------");
            double bestConstant = result.BestParameters["constant"].Value;
            Console.WriteLine("bestConstant: " + bestConstant + "------------");

            //
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

            teacher.ParallelOptions.MaxDegreeOfParallelism = 1;

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
                mathServ.NormalizeData(aggregateData[1].ToArray(), 0, 1)
                //aggregateData[2].ToArray()
            };
            int[] predicted = Learn.Decide(testinput);
            return predicted;
        }
    }
}
