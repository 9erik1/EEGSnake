
using Accord.Math;
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

        /////////////////////////////////////////////////////
        ///// CONVERT DOUBLE TO COMPLEX, THEN FFT ///////////
        ///// Currently only uses BW_hi_5 ///////////////////
        /////////////////////////////////////////////////////
        public double[] Conversion_fft(double[] raw_proxy)
        {
            //var p = raw; // redundant
            //Complex[] complex = new Complex[p.Count()]; // redundant

            Complex[] complex_raw_proxy = new Complex[raw_proxy.Length];  // INITIALIZING FOR CONVERSION OF THE double INPUT TO a Complex variable

            ///// CONVERT TO COMPLEX VALUES /////
            int buffer_window = 0;                                                  // ignoring this many data points due to startup of data acquisition (not necessary for sim data)
            for (int j = buffer_window; j < complex_raw_proxy.Length; j++)
            {
                //complex_raw[j] = new Complex(p[j], 0); // redundant
                complex_raw_proxy[j] = new Complex(raw_proxy[j], 0);
            }
            Accord.Math.Transforms.FourierTransform2.FFT(complex_raw_proxy, FourierTransform.Direction.Forward);

            int firstHalfFFT = complex_raw_proxy.Length / 2;
            double[] magnitudes = new double[firstHalfFFT];
            for(int i = 0; i < firstHalfFFT; i++)           
                magnitudes[i] = complex_raw_proxy[i].Magnitude;            

            return magnitudes;
        }

        /////////////////////////////////////////////////////
        ///// BUTTERWORTH HIGH PASS FILTER, f_c = 5 Hz //////
        ///// References: [1], [2] //////////////////////////
        /////////////////////////////////////////////////////
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
