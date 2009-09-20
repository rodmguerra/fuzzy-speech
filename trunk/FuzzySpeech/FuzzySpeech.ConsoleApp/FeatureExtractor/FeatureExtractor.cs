using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using FuzzySpeech.Audio;

namespace FuzzySpeech.Extractor
{
    class FeatureExtractor
    {
        int bandCount = 25;
        double reductionMaxPercent = 0.1;
        AmplitudeType amplitudeType = AmplitudeType.Magnitude;

        public AudioSample Extract(AudioSample sample)
        {
            AudioSample melScaledSpectrogram = ExtractorManager.Instance.MelScale(sample, sample.SampleRate/2);
            return ExtractorManager.Instance.Reduct(melScaledSpectrogram, bandCount, reductionMaxPercent);
            
        }

        


            

        
    }
}
