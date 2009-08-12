using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Model;
using FuzzySpeech.Audio;

namespace FuzzySpeech.Managers
{
    class RecognizerManager
    {
        /// <summary>
        /// Calculates the phoneme's fuzzy length similarity to an audio input phoneme
        /// </summary>
        /// <param name="audioPhoneme">
        /// The audio phoneme to be compared to this phoneme.
        /// </param>
        /// <returns>
        /// The similarity (0 - 1) of this phoneme's length compared to the audio input
        /// </returns>
        public double LengthSimilarity(Phoneme phoneme, AudioSample audioPhoneme)
        {
            return phoneme.Length.SimilarityTo(audioPhoneme.Frames.Count);
        }

        /// <summary>
        /// Calculates the phoneme's fuzzy color similarity to an audio sample
        /// </summary>
        /// <param name="audioPhoneme">
        /// The audio sample to be compared to this phoneme.
        /// </param>
        /// <returns>
        /// The similarity (0 - 1) of this phoneme's color compared to the audio sample.
        /// </returns>
        public double ColorSimilarity(Phoneme phoneme, AudioSample audioSample)
        {
            List<double> compatibilities  = new List<double>();

            //step 1: Single frame pattern matching
            foreach(AudioFrame frame in audioSample.Frames)
            {
                compatibilities.Add(this.SingleFramePatternMatch(phoneme,frame));
            }

            //step 2: Color compatibility aggregation
            return Helper.Util.MaxNPercent(compatibilities, 0.1);
        }

        /// <summary>
        /// Calculates the phoneme's fuzzy similarity to an audio input phoneme
        /// </summary>
        /// <param name="audioPhoneme">
        /// The audio phoneme to be compared to this phoneme.
        /// </param>
        /// <returns>
        /// The similarity (0 - 1) of this phoneme compared to the audio input
        /// </returns>
        public double Similarity(Phoneme phoneme, AudioSample audioPhoneme)
        {
            return this.LengthSimilarity(phoneme, audioPhoneme);// *this.ColorSimilarity(phoneme, audioPhoneme);
        }

        public double SingleFramePatternMatch(Phoneme phoneme, AudioFrame frame)
        {
            List<double> bandMatch = new List<double>();
            List<double> frameMatch = new List<double>();

            for (int bandIndex = 0; bandIndex < frame.NumberOfBands; bandIndex++)
            {
                //Color similarities
                bandMatch.Clear();
                for( int colorIndex = 0; colorIndex < phoneme.ColorsByBand; colorIndex++)
                {
                    bandMatch.Add
                    (
                        phoneme.Bands[bandIndex].GetColor(colorIndex)
                            .SimilarityTo(  frame[bandIndex]  )

                        
                    );
                    //System.Windows.Forms.MessageBox.Show("Band: " + bandIndex + "\nColor: " + colorIndex);
                    //System.Windows.Forms.MessageBox.Show("Frame: " + frame[bandIndex] + "\nColor: " + phoneme.Bands[bandIndex].GetColor(colorIndex) + "\nCompatibility: " + bandMatch[colorIndex], phoneme.Bands[bandIndex].GetColor(colorIndex).Name);
                }

                //The best color compatibility for each band is added 
                frameMatch.Add(bandMatch.Max());
            }

            //The frame match is the minimum compatibility of the bands
            return frameMatch.Min();
        }

        //Singleton
        RecognizerManager() { }
        static RecognizerManager instance;
        public static RecognizerManager Instance
        {
            get {
                if (RecognizerManager.instance == null) instance = new RecognizerManager();
                return instance;
            }
        }

        
    }
}
