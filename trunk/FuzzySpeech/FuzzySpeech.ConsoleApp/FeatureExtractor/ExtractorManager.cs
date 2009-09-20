using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Audio;
using System.IO;

namespace FuzzySpeech.Extractor
{
    class ExtractorManager
    {
        public void GetSignalChannelsFromWaveData16bits(byte[] waveData, out AudioSignal leftChannelSignal, out AudioSignal rightChannelSignal)
        {
            leftChannelSignal = new AudioSignal(waveData.Length / 4);
            rightChannelSignal = new AudioSignal(waveData.Length / 4);

            // Split out channels from sample
            int channelIndex = 0;
            for (int i = 0; i < waveData.Length; i += 4)
            {
                leftChannelSignal[channelIndex] = (double)BitConverter.ToInt16(waveData, i);
                rightChannelSignal[channelIndex] = (double)BitConverter.ToInt16(waveData, i + 2);
                channelIndex++;
            }
        }

        public void GetSignalChannelsFromWaveData16bits(byte[] waveData, out AudioSignal monoChannelSignal)
        {
            monoChannelSignal = new AudioSignal(waveData.Length / 2);

            // Split out channels from sample
            int channelIndex = 0;
            for (int i = 0; i < waveData.Length; i += 2)
            {
                monoChannelSignal[channelIndex] = (double)BitConverter.ToInt16(waveData, i);
                channelIndex++;
            }
        }

        public AudioSignal GetMonoSignalFromStereoSignal(AudioSignal leftChannelSignal, AudioSignal rightChannelSignal)
        {
            //Averages the two samples and get a mono signal
            AudioSignal monoChannelSignal = new AudioSignal(leftChannelSignal.FrameCount);
            for (int i = 0; i < monoChannelSignal.FrameCount; i++)
                monoChannelSignal[i] = (leftChannelSignal[i] + rightChannelSignal[i]);

            return monoChannelSignal;
        }

        public AudioSample GetSpectrogramFromChannelSignal(AudioSignal channelSignal, int fftSize, int sampleRate, AmplitudeType amplitudeType)
        {
            AudioSample spectrogram = new AudioSample(fftSize/2);
            spectrogram.SampleRate = sampleRate;
            spectrogram.AmplitudeType = amplitudeType;

            for (int i = 0; i < channelSignal.FrameCount; i += fftSize)
            {
                //int length = i + fftSize > channelSignal.FrameCount ? channelSignal.FrameCount - i: fftSize;
                //AudioSignal range = channelSignal.GetFrameRange(i, length);

                if (i + fftSize > channelSignal.FrameCount) break;
                AudioSignal range = channelSignal.GetFrameRange(i, fftSize);
                
                
                AudioFrame frameFFT = this.FFT(range, amplitudeType);

               

                spectrogram.Frames.Add(frameFFT);
            }

            return spectrogram;
        }

        public AudioSignal GetSignalFromStream(WaveStream waveStream)
        {
            bool stereo = waveStream.Format.nChannels == 2;

            AudioSignal signal;

            if (!stereo)
            {
                byte[] waveData = new byte[waveStream.Length];
                waveStream.Read(waveData, 0, waveData.Length);

                GetSignalChannelsFromWaveData16bits(waveData, out signal);
            }
            else
            {
                byte[] waveData = new byte[waveStream.Length];
                waveStream.Write(waveData, 0, waveData.Length);

                AudioSignal leftChannel;
                AudioSignal rightChannel;

                GetSignalChannelsFromWaveData16bits(waveData, out leftChannel, out rightChannel);
                signal = GetMonoSignalFromStereoSignal(leftChannel, rightChannel);

            }

            return signal;
        }

        /// <summary>
        /// Gets the correspondent frequency in mel scale from a hertz scaled frequency. 
        /// </summary>
        /// <param name="frequency">
        /// The frequency in hertz.
        /// </param>
        /// <returns>
        /// The correspondent frequency in mels.
        /// </returns>
        public double GetMelsFromFrequency(double frequency)
        {
            return 1127.01048 * Math.Log(1 + frequency / 700);
            //return (1000 / Math.Log10(2)) * Math.Log10(1 + frequency / 1000); //fant
        }

        /// <summary>
        /// Gets the correspondent frequency in herz scale from a mel scaled frequency.
        /// </summary>
        /// <param name="mel">
        /// The freuency in mels.
        /// </param>
        /// <returns>
        /// The correspondent frequency in herz.
        /// </returns>
        public double GetFrequencyFromMels(double mel)
        {
            return 700 * (Math.Exp(mel / 1127.01048) - 1);
        }

        /// <summary>
        /// Mel Scales an array of amplitudes indexed by frequency.
        /// </summary>
        /// <param name="amplitudesByFrequency">
        /// The amplitude values for each frequency.
        /// </param>
        /// <param name="maximumFrequency">
        /// The top frequency represented in amplitude values array.
        /// </param>
        /// <returns>
        /// The mel scaled array.
        /// </returns>
        public AudioFrame MelScale(AudioFrame frame, double maximumFrequency)
        {
            //Creates a vector containing amplitudes in mel scale
            AudioFrame amplitudesByMel = new AudioFrame(frame.NumberOfBands);

            double maximumMels = GetMelsFromFrequency(maximumFrequency);

            //The step in frequency or mel from one amplitude to another
            double frequencyGridSize = maximumFrequency / frame.NumberOfBands;
            double melGridSize = maximumMels / amplitudesByMel.NumberOfBands;

            double melGridBottom = 0;
            double melGridTop = melGridSize;
            //int topFrequencyIndex;
            //int bottomFrequencyIndex = 0;
            for (int i = 0; i < frame.NumberOfBands; i++)
            {
                if (i == 127)
                    i = 127;
                //Discover the maximum frequency containing the amplitudes to be averaged
                double bottomFrequency = GetFrequencyFromMels(melGridBottom);
                double topFrequency = GetFrequencyFromMels(melGridTop);
                double frequencyRange = topFrequency - bottomFrequency;

                int bottomFrequencyIndex = (int)(bottomFrequency / frequencyGridSize);
                int frequencyIndexRange = (int)(frequencyRange / frequencyGridSize);
                int topFrequencyIndex = bottomFrequencyIndex + frequencyIndexRange;
                
                /*
                
                topFrequencyIndex = (int)Math.Round(GetFrequencyFromMels(melGridTop) / frequencyGridSize) - 1;

                //if no range was selected, selects the inferior index as the range
                if (topFrequencyIndex < bottomFrequencyIndex)
                {
                    topFrequencyIndex = bottomFrequencyIndex;
                }
                 * 
                 */

                //Averages the selected amplitudes
                for (int j = bottomFrequencyIndex; j <= topFrequencyIndex; j++)
                {
                    amplitudesByMel[i] += frame[j];
                }
                amplitudesByMel[i] /= ((topFrequencyIndex - bottomFrequencyIndex) + 1);

                //Goes to the next mel grid
                melGridBottom += melGridSize;
                melGridTop += melGridSize;
            }

            return amplitudesByMel;

        }

        //Singleton
        ExtractorManager() { }
        static ExtractorManager instance;
        public static ExtractorManager Instance
        {
            get {
                if (ExtractorManager.instance == null) instance = new ExtractorManager();
                return instance;
            }
        }



        public AudioFrame FFT(AudioSignal audioSignal, AmplitudeType amplitudeType)
        {

            AudioSignal x = audioSignal;

            // Assume n is a power of 2
            int n = x.FrameCount;
            int nu = (int)(Math.Log(n) / Math.Log(2));
            int n2 = n / 2;
            AudioFrame frequencyDomainFrame = new AudioFrame(n2);
            int nu1 = nu - 1;
            double[] xre = new double[n];
            double[] xim = new double[n];
            
            double tr, ti, p, arg, c, s;
            for (int i = 0; i < n; i++)
            {
                xre[i] = x[i];
                xim[i] = 0.0f;
            }
            int k = 0;
            for (int l = 1; l <= nu; l++)
            {
                while (k < n)
                {
                    int index = k + n2 < xre.Length ? k + n2 : xre.Length - 1;
                    for (int i = 1; i <= n2; i++)
                    {
                        /*
                        p = Helper.Util.BitReverse(k >> nu1, nu);
                        arg = 2 * (double)Math.PI * p / n;
                        c = (double)Math.Cos(arg);
                        s = (double)Math.Sin(arg);
                        tr = xre[k + n2] * c + xim[k + n2] * s;
                        ti = xim[k + n2] * c - xre[k + n2] * s;
                        xre[k + n2] = xre[k] - tr;
                        xim[k + n2] = xim[k] - ti;
                        xre[k] += tr;
                        xim[k] += ti;
                        k++;
                         */


                        p = Common.Helper.Util.BitReverse(k >> nu1, nu);
                        arg = 2 * (double)Math.PI * p / n;
                        c = (double)Math.Cos(arg);
                        s = (double)Math.Sin(arg);
                        tr = xre[index] * c + xim[index] * s;
                        ti = xim[index] * c - xre[index] * s;
                        xre[index] = xre[k] - tr;
                        xim[index] = xim[k] - ti;
                        xre[k] += tr;
                        xim[k] += ti;
                        k++;
                    }
                    k += n2;
                }
                k = 0;
                nu1--;
                n2 = n2 / 2;
            }
            k = 0;
            int r;
            while (k < n)
            {
                r = Common.Helper.Util.BitReverse(k, nu);
                if (r > k)
                {
                    tr = xre[k];
                    ti = xim[k];
                    xre[k] = xre[r];
                    xim[k] = xim[r];
                    xre[r] = tr;
                    xim[r] = ti;
                }
                k++;
            }

            

            for (int i = 0; i < n / 2; i++)
            {
                frequencyDomainFrame[i] = (double)(Math.Sqrt((xre[i] * xre[i]) + (xim[i] * xim[i])));

                switch (amplitudeType)
                {
                    case AmplitudeType.Decibel:   frequencyDomainFrame[i] = 10.0 * Math.Log10(frequencyDomainFrame[i]); break;
                    case AmplitudeType.Magnitude: break;
                    default: throw new Exception();
                }
            }
            
            
            //return magnitude;
            return frequencyDomainFrame;
        }

        public AudioSample MelScale(AudioSample sample, double maximumFrequency)
        {
            AudioSample melScaledSpectrogram = new AudioSample(sample.BandsByFrame);
            foreach (AudioFrame frame in sample.Frames)
            {
                melScaledSpectrogram.Frames.Add(ExtractorManager.Instance.MelScale(frame, maximumFrequency));
            }

            melScaledSpectrogram.SampleRate = sample.SampleRate;
            melScaledSpectrogram.AmplitudeType = sample.AmplitudeType;
            return melScaledSpectrogram;
        }

        public AudioSample ReadAudioSampleFromFile(string inputWaveFilePath, AmplitudeType amplitudeType)
        {
            AudioSignal signal;

            int samplesPerSecond = 0;

            using (FileStream fileStream = new FileStream(inputWaveFilePath, FileMode.Open, FileAccess.Read))
            {
                byte[] data = new byte[fileStream.Length];
                fileStream.Read(data,0,data.Length);
                fileStream.Flush();
                fileStream.Close();

                MemoryStream memoryStream = new MemoryStream(data);
                //WaveStream waveStream = new WaveStream(memoryStream);
                //sampleStream.CreateMemoryStreamFromFileStream(fileStream);

                using (WaveStream waveStream = new WaveStream(memoryStream))
                {
                    samplesPerSecond = waveStream.Format.nSamplesPerSec;
                    signal = ExtractorManager.Instance.GetSignalFromStream(waveStream);
                    waveStream.Flush();
                    waveStream.Close();
                }
            }

            
            return ExtractorManager.Instance.GetSpectrogramFromChannelSignal(signal, 256, samplesPerSecond, amplitudeType);
        }

        public AudioSample Reduct (AudioSample sample, int reductedBandCount, double nPercentMaxValues)
        {
            AudioSample reducted = new AudioSample(reductedBandCount);
            
            int bandsByRange = sample.BandsByFrame/reductedBandCount;

            foreach (AudioFrame frame in sample.Frames)
            {
                AudioFrame reductedFrame = new AudioFrame(reductedBandCount);
                for (int i = 0; i < reducted.BandsByFrame; i++)
                {
                    int rangeBandCount = ((i*bandsByRange + bandsByRange) > sample.BandsByFrame) ? sample.BandsByFrame - i*bandsByRange : bandsByRange;
                    reductedFrame[i] = Common.Helper.Util.MaxNPercent(frame.GetBandRange(i*bandsByRange, rangeBandCount), nPercentMaxValues);
                }
                reducted.Frames.Add(reductedFrame);
            }

            return reducted;
        }
    }
}
