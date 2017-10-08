
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
        #region pipework
        public SVMClassifier()
        {
            mathServ = DataProcessingPlant.Instance;
        }

        #endregion

        private double[][] ApplyPCA(Queue<Double>[] rawStream)
        {
            PrincipalComponentAnalysis pcaLib = new PrincipalComponentAnalysis(PrincipalComponentMethod.Center);

            var pca = new double[4][];
            pca[0] = rawStream[0].ToArray();
            pca[1] = rawStream[1].ToArray();
            pca[2] = rawStream[2].ToArray();
            pca[3] = rawStream[3].ToArray();

            for (int p = 0; p < pca.Count(); p++)
                pca[p] = mathServ.BW_hi_5(pca[p]);

            // Apply PCA
            var x = pcaLib.Learn(pca.Transpose());
            double[][] actual = pcaLib.Transform(pca.Transpose());


            // Apply Reverse PCA to Time-Series
            double[][] removedComp = new double[2][];
            removedComp[0] = pcaLib.ComponentVectors[0];
            removedComp[1] = pcaLib.ComponentVectors[1];

            actual = actual.Dot(pcaLib.ComponentVectors);
            actual = actual.Transpose();


            // Apply FFT
            Complex[][] transformed_data = new Complex[actual.Count()][];
            for (int f = 0; f < actual.Count(); f++)
            {
                transformed_data[f] = new Complex[actual[f].Count()];
                //transformed_data[f] = mathServ.Conversion_fft(actual[f]);//this is need
            }

            return actual;
        }

        public void UpdateSVM(Queue<Double>[] rawStream, int output)//Know your output
        {
            double[][] aggregateData = ApplyPCA(rawStream);
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

        public int[] AnswerSVM(Queue<Double>[] rawStream)
        {
            double[][] aggregateData = ApplyPCA(rawStream);
            double[][] testinput =
            {
                aggregateData[1],
                aggregateData[2]
            };
            int[] predicted = Learn.Decide(testinput);
            return predicted;
        }


        public int[] MachineLearn(Queue<Double>[] rawStream)
        {

            double[][] aggregateData = ApplyPCA(rawStream);

            double[][] inputs =
             {
                aggregateData[0], //  0 
                aggregateData[0], //  0
                aggregateData[3], //  2
    aggregateData[3], //  2
    aggregateData[3], //  2
    aggregateData[3],//2
    aggregateData[3],
    aggregateData[3],//  2
    aggregateData[3], //  2
    aggregateData[3], //  2
    aggregateData[3], //  2
    aggregateData[3],//2
    aggregateData[3],
    aggregateData[3],//  2
    aggregateData[3], //  2
    aggregateData[3], //  2
    aggregateData[3], //  2
    aggregateData[3],//2
    aggregateData[3],
    aggregateData[3],//  2
    aggregateData[3], //  2
    aggregateData[3], //  2
    aggregateData[3], //  2
    aggregateData[3],//2
    aggregateData[3],
    aggregateData[3]//  2
            };

            int[] outputs = // those are the class labels
{
    0, 0,

    1, 1, 1, 1, 1,1,1, 1, 1, 1, 1,1,1, 1, 1, 1, 1,1,1, 1, 1, 1, 1,1
};
            double[][] testinput =
{
                //               input         output
    aggregateData[3], //  2
    aggregateData[0], //  0 
    aggregateData[0], //  0
    aggregateData[0], //  0
    aggregateData[0], //  0

    aggregateData[3], //  2

    aggregateData[3], //  2
    aggregateData[3] //  0

 };


            // Create the multi-class learning algorithm for the machine
            var teacher = new MulticlassSupportVectorLearning<Gaussian>()
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


            // Create the multi-class learning algorithm for the machine
            var calibration = new MulticlassSupportVectorLearning<Gaussian>()
            {
                Model = Learn, // We will start with an existing machine

                // Configure the learning algorithm to use SMO to train the
                //  underlying SVMs in each of the binary class subproblems.
                Learner = (param) => new ProbabilisticOutputCalibration<Gaussian>()
                {
                    Model = param.Model // Start with an existing machine
                }
            };


            // Configure parallel execution options
            calibration.ParallelOptions.MaxDegreeOfParallelism = 1;

            // Learn a machine
            calibration.Learn(inputs, outputs);

            // Obtain class predictions for each sample
            int[] predicted = Learn.Decide(testinput);

            Console.WriteLine(predicted);
            return predicted;
        }
    }
}
