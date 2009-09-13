using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Audio;
using System.IO;

namespace FuzzySpeech.FeatureExtractor
{
    class ExtractorManager
    {
        public void GetSignalChannelsFromWaveData16bits(byte[] waveData, out double[] leftChannelSignal, out double[] rightChannelSignal)
        {
            leftChannelSignal = new double[waveData.Length / 4];
            rightChannelSignal = new double[waveData.Length / 4];

            // Split out channels from sample
            int channelIndex = 0;
            for (int i = 0; i < waveData.Length; i += 4)
            {
                leftChannelSignal[channelIndex] = (double)BitConverter.ToInt16(waveData, i);
                rightChannelSignal[channelIndex] = (double)BitConverter.ToInt16(waveData, i + 2);
                channelIndex++;
            }
        }

        public void GetSignalChannelsFromWaveData16bits(byte[] waveData, out double[] monoChannelSignal)
        {
            monoChannelSignal = new double[waveData.Length / 4];

            // Split out channels from sample
            int channelIndex = 0;
            for (int i = 0; i < waveData.Length; i += 2)
            {
                monoChannelSignal[channelIndex] = (double)BitConverter.ToInt16(waveData, i);
                channelIndex++;
            }
        }

        public double[] GetMonoSignalFromStereoSignal(double[] leftChannelSignal, double[] rightChannelSignal)
        {
            //Averages the two samples and get a mono signal
            double[] monoChannelSignal = new double[leftChannelSignal.Length];
            for (int i = 0; i < monoChannelSignal.Length; i++)
                monoChannelSignal[i] = (leftChannelSignal[i] + rightChannelSignal[i]);

            return monoChannelSignal;
        }

        public AudioSample GetSpectrogramFromChannelSignal(double[] channelSignal, int fftSize, int sampleRate)
        {
            AudioSample spectrogram = new AudioSample(fftSize/2);
            spectrogram.SampleRate = sampleRate;

            for (int i = 0; i < channelSignal.Length; i += fftSize)
            {
                double[] range = channelSignal.ToList().GetRange(i * fftSize, fftSize).ToArray();
                AudioFrame frameFFT = this.FFT(ref channelSignal, AmplitudeType.Decibel);
                spectrogram.Frames.Add(frameFFT);
            }

            return spectrogram;
        }

        public double[] GetSignalFromStream(WaveStream waveStream, bool stereo)
        {
            double[] signal;

            if (!stereo)
            {
                byte[] waveData = new byte[waveStream.Length];
                waveStream.Write(waveData, 0, waveData.Length);

                GetSignalChannelsFromWaveData16bits(waveData, out signal);
            }
            else
            {
                byte[] waveData = new byte[waveStream.Length];
                waveStream.Write(waveData, 0, waveData.Length);

                double[] leftChannel;
                double[] rightChannel;

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
            int topFrequencyIndex;
            int bottomFrequencyIndex = 0;
            for (int i = 0; i < frame.NumberOfBands; i++)
            {
                //Discover the maximum frequency containing the amplitudes to be averaged
                double bottomFrequency = GetFrequencyFromMels(melGridBottom);
                double topFrequencyGrid = GetFrequencyFromMels(melGridTop);

                bottomFrequencyIndex = (int)(GetFrequencyFromMels(melGridBottom) / frequencyGridSize);
                topFrequencyIndex = (int)Math.Round(GetFrequencyFromMels(melGridTop) / frequencyGridSize) - 1;

                //if no range was selected, selects the inferior index as the range
                if (topFrequencyIndex < bottomFrequencyIndex)
                {
                    topFrequencyIndex = bottomFrequencyIndex;
                }

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



        public AudioFrame FFT(ref double[] x, AmplitudeType amplitudeType)
        {
            // Assume n is a power of 2
            int n = x.Length;
            int nu = (int)(Math.Log(n) / Math.Log(2));
            int n2 = n / 2;
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
                    for (int i = 1; i <= n2; i++)
                    {
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
                r = Helper.Util.BitReverse(k, nu);
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

            AudioFrame frequencyDomainFrame = new AudioFrame(n2);

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
            return melScaledSpectrogram;
        }

        public AudioSample ReadAudioSampleFromFile(string inputWaveFilePath)
        {
            WaveStream sampleStream;

            using (FileStream fileStream = new FileStream(inputWaveFilePath, FileMode.Open, FileAccess.Read))
            {
                Stream stream = WaveStream.CreateMemoryStreamFromFileStream(fileStream);
                sampleStream = new WaveStream(stream);
            }


            double[] signal = ExtractorManager.Instance.GetSignalFromStream(sampleStream, sampleStream.Format.nChannels == 2);
            return ExtractorManager.Instance.GetSpectrogramFromChannelSignal(signal, 256, sampleStream.Format.nSamplesPerSec);
        }
    }
}
