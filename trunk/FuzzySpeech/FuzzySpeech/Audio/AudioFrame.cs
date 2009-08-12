using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzySpeech.Audio
{
    class AudioFrame
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

        public int NumberOfBands
        {
            get{ return data.Length; }
        }


    }
}
