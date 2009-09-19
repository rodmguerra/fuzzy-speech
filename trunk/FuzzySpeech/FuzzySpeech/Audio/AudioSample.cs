using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Audio.Collections;

namespace FuzzySpeech.Audio
{
    class AudioSample
    {
        private AudioFrameCollection frames;

        public AudioFrameCollection Frames
        {
            get { return frames; }
            set { frames = value; }
        }
        int bandsByFrame;

        public int BandsByFrame
        {
            get { return bandsByFrame; }
        }

        public AudioSample(int bandsByFrame)
        {
            this.bandsByFrame = bandsByFrame;
            frames = new AudioFrameCollection(this);
        }

        private AmplitudeType amplitudeType = AmplitudeType.Decibel;

        public AmplitudeType AmplitudeType
        {
            get { return amplitudeType; }
            set { amplitudeType = value; }
        }

        private int sampleRate;

        public int SampleRate
        {
            get { return sampleRate; }
            set { sampleRate = value; }
        }


        public void DivideAmplitudesPer (double factor)
        {
            foreach (AudioFrame frame in frames)
            {
                frame.DivideAmplitudesPer(factor);
            }
        }

        public double GetMaximumAmplitude()
        {
            double maximumAmplitude = Double.MinValue;
            foreach (AudioFrame frame in frames)
            {
                double frameMaximumAmplitude = frame.GetMaximumAmplitude();
                if (frameMaximumAmplitude > maximumAmplitude)
                    maximumAmplitude = frameMaximumAmplitude;
            }

            return maximumAmplitude;
        }

    }

    public enum AmplitudeType
    {
        Decibel,
        Magnitude
    }
}
