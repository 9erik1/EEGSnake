
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
        #region pipework
        public SVMClassifier()
        {

        }

        #endregion

        #region filters

        /////////////////////////////////////////////////////
        ///// CONVERT DOUBLE TO COMPLEX, THEN FFT ///////////
        ///// Currently only uses BW_hi_5 ///////////////////
        /////////////////////////////////////////////////////
        private Complex[] Conversion_fft(double[] raw_proxy)
        {
            //var p = raw; // redundant
            //Complex[] complex = new Complex[p.Count()]; // redundant

            Complex[] complex_raw_proxy = new Complex[raw_proxy.Count()];  // INITIALIZING FOR CONVERSION OF THE double INPUT TO a Complex variable

            ///// CONVERT TO COMPLEX VALUES /////
            int buffer_window = 0;                                                  // ignoring this many data points due to startup of data acquisition (not necessary for sim data)
            for (int j = buffer_window; j < complex_raw_proxy.Count(); j++)
            {
                //complex_raw[j] = new Complex(p[j], 0); // redundant
                complex_raw_proxy[j] = new Complex(raw_proxy[j], 0);
            }
            Accord.Math.Transforms.FourierTransform2.FFT(complex_raw_proxy, FourierTransform.Direction.Forward);

            return complex_raw_proxy;
        }

        /////////////////////////////////////////////////////
        ///// BUTTERWORTH HIGH PASS FILTER, f_c = 5 Hz //////
        ///// References: [1], [2] //////////////////////////
        /////////////////////////////////////////////////////
        private double[] BW_hi_5(double[] dd8)
        {
            double GAIN = 1.379370774e+00;
            double[] xv, yv;
            xv = new double[5];
            yv = new double[5];

            double[] dd7 = dd8;

            for (int i = 0; i < dd8.Count(); i++)
            {
                xv[0] = xv[1];
                xv[1] = xv[2];
                xv[2] = xv[3];
                xv[3] = xv[4];
                xv[4] = dd8[i] / GAIN;
                yv[0] = yv[1];
                yv[1] = yv[2];
                yv[2] = yv[3];
                yv[3] = yv[4];
                yv[4] = (xv[0] + xv[4]) - 4 * (xv[1] + xv[3]) + 6 * xv[2]
                             + (-0.5255789745 * yv[0]) + (2.4389986574 * yv[1])
                             + (-4.2755090771 * yv[2]) + (3.3594051013 * yv[3]);

                dd7[i] = yv[4];
            }

            return dd7;

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
                pca[p] = BW_hi_5(pca[p]);

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
                transformed_data[f] = Conversion_fft(actual[f]);
            }

            return actual;
        }

        public void UpdateSVM(Queue<Double>[] rawStream, int output)//Know your output
        {
            double[][] aggregateData = ApplyPCA(rawStream);
            double[][] inputs =
            {
                aggregateData[1],
                aggregateData[2]
            };
            int[] outputs =
            {
                output,
                output
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
