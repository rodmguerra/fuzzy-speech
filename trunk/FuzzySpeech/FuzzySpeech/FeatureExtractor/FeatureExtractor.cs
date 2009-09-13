using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FuzzySpeech.Audio;

namespace FuzzySpeech.FeatureExtractor
{
    class FeatureExtractor
    {

        public AudioSample Extract(AudioSample sample)
        {
            AudioSample melScaledSpectrogram = ExtractorManager.Instance.MelScale(sample, sample.SampleRate/2);

            //TODO: Data Reduction

            return melScaledSpectrogram;
            
        }

        


            

        
    }
}
