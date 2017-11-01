
using Accord.Math;
using Accord.Statistics.Analysis;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace EEGfront
{
    public class DataProcessingPlant
    {
        private static DataProcessingPlant instance;
        public static DataProcessingPlant Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new DataProcessingPlant();
                }
                return instance;
            }
        }

        /// <summary>ApplyPCA is a method in the DataProcessingPlant class.
        /// <para>Here's how you apply pca on the 4 channels.
        /// returns: PCA applied subtracting the first and last components</para>
        /// <seealso cref="DataProcessingPlant.cs"/>
        /// </summary>
        public Queue<Double>[] ApplyPCA(Queue<Double>[] rawStream)
        {
            PrincipalComponentAnalysis pcaLib = new PrincipalComponentAnalysis(PrincipalComponentMethod.Center);

            var pca = new double[4][];
            pca[0] = rawStream[0].ToArray();
            pca[1] = rawStream[1].ToArray(); 
            pca[2] = rawStream[2].ToArray();
            pca[3] = rawStream[3].ToArray();

            // Apply PCA
            pcaLib.Learn(pca.Transpose());
            double[][] actual = pcaLib.Transform(pca.Transpose());

            // Apply Reverse PCA to Time-Series
            double[][] removedComp = new double[2][];
            removedComp[0] = pcaLib.ComponentVectors[1];
            removedComp[1] = pcaLib.ComponentVectors[2];

            actual = actual.Dot(removedComp);
            actual = actual.Transpose();

            Queue<Double>[] proxy = new Queue<double>[4];

            for (int i = 0; i < 4; i++)
            {
                proxy[i] = new Queue<double>();
                for (int j = 0; j < actual[i].Length; j++)
                {
                    proxy[i].Enqueue(actual[i][j]);
                }
            }


            return proxy;
        }

        /// <summary>GetPcaComponent is a method in the DataProcessingPlant class.
        /// <para>Here's how you apply pca on the 4 channels and retrieve the components.
        /// returns the Principle Components</para>
        /// <seealso cref="DataProcessingPlant.cs"/>
        /// </summary>
        public Queue<Double>[] GetPcaComponent(Queue<Double>[] rawStream)
        {
            PrincipalComponentAnalysis pcaLib = new PrincipalComponentAnalysis(PrincipalComponentMethod.Center);

            var pca = new double[4][];
            pca[0] = rawStream[0].ToArray();
            pca[1] = rawStream[1].ToArray();
            pca[2] = rawStream[2].ToArray();
            pca[3] = rawStream[3].ToArray();

            // Apply PCA
            pcaLib.Learn(pca.Transpose());
            double[][] actual = pcaLib.Transform(pca.Transpose()).Transpose();

            Queue<Double>[] proxy = new Queue<double>[4];

            for (int i = 0; i < 4; i++)
            {
                proxy[i] = new Queue<double>();
                for (int j = 0; j < actual[i].Length; j++)
                {
                    proxy[i].Enqueue(actual[i][j]);
                }
            }


            return proxy;

            //return actual.Transpose();
        }

        /// <summary>Conversion_fft is a method in the DataProcessingPlant class.
        /// <para>Here's how you CONVERT DOUBLE TO COMPLEX, THEN FFT.</para>
        /// <seealso cref="DataProcessingPlant.cs"/>
        /// </summary>
        public Queue<Double>[] Conversion_fft(Queue<Double>[] raw_proxy)
        {
            Queue<Double>[] proxy = new Queue<double>[4];
            for (int rawI = 0; rawI < raw_proxy.Length; rawI++)
            {
                proxy[rawI] = new Queue<double>();
                Complex[] complex_raw_proxy = new Complex[raw_proxy[rawI].Count];  // INITIALIZING FOR CONVERSION OF THE double INPUT TO a Complex variable

                ///// CONVERT TO COMPLEX VALUES /////
                double[] rr = raw_proxy[rawI].ToArray();
                int buffer_window = 0;                                                  // ignoring this many data points due to startup of data acquisition (not necessary for sim data)
                for (int j = buffer_window; j < complex_raw_proxy.Length; j++)
                {
                    //complex_raw[j] = new Complex(p[j], 0); // redundant
                    complex_raw_proxy[j] = new Complex(rr[j], 0);
                }
                Accord.Math.Transforms.FourierTransform2.FFT(complex_raw_proxy, FourierTransform.Direction.Forward);

                int firstHalfFFT = complex_raw_proxy.Length / 2;
                for (int i = 0; i < firstHalfFFT; i++)
                    proxy[rawI].Enqueue(complex_raw_proxy[i].Magnitude);
            }

            return proxy;
        }

        /// <summary>BW_hi_5 is a method in the DataProcessingPlant class.
        /// <para>Here's how you BUTTERWORTH HIGH PASS FILTER, f_c = 5 Hz.</para>
        /// <seealso cref="DataProcessingPlant.cs"/>
        /// </summary>
        public double[] BW_hi_5(double[] dd8)
        {
            double GAIN = 1.379370774e+00;
            double[] xv, yv;
            xv = new double[5];
            yv = new double[5];

            double[] dd7 = dd8;

            for (int i = 0; i < dd8.Length; i++)
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
    }
}
