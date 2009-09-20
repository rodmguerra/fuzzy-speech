using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzySpeech.Audio
{
    public class AudioFrame
    {
        private double[] data;

        public double this [int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }

        public AudioFrame(int numberOfBands)
        {
            data = new double[numberOfBands];
        }

        public AudioFrame(double[] data)
        {
            this.data = new double[data.Length];
            for (int i=0; i<data.Length; i++)
            {
                this.data[i] = data[i];
            }
        }

        public int NumberOfBands
        {
            get{ return data.Length; }
        }

        public double[] GetBandRange (int firstBandIndex, int bandCount) 
        {
            return data.ToList().GetRange(firstBandIndex, bandCount).ToArray();      
        }

        public void DivideAmplitudesPer(double factor)
        {
            for (int i=0; i<data.Length; i++)
            {
                data[i] /= factor;
            }
        }

        public double GetMaximumAmplitude()
        {
            double maximumAmplitude = Double.MinValue;
            foreach (double bandAmplitude in data)
            {
                if (bandAmplitude > maximumAmplitude)
                    maximumAmplitude = bandAmplitude;
            }

            return maximumAmplitude;
        }


    }
}
