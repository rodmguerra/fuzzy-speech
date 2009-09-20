using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzySpeech.Audio
{
    class AudioSignal
    {
        double[] data;

        public double this[int index]
        {
            get { return data[index]; }
            set { data[index] = value; }
        }

        public AudioSignal(int frameCount)
        {
            data = new double[frameCount];
        }

        public int FrameCount
        {
            get{ return data.Length; }
        }

        public AudioSignal GetFrameRange (int firstFrameIndex, int frameCount) 
        {
            AudioSignal range = new AudioSignal();
            double [] rangeData = data.ToList().GetRange(firstFrameIndex, frameCount).ToArray();
            range.data = rangeData;
            return range;           
        }

        private AudioSignal() {}


    }
}
