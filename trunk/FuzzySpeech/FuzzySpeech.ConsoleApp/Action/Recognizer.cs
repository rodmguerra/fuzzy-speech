using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Model;
using FuzzySpeech.Audio;
using FuzzySpeech.Managers;

namespace FuzzySpeech.Action
{
    class Recognizer
    {
        private RecognizerGenome genome;

        public RecognizerGenome Genome
        {
            get { return genome; }
            set { genome = value; }
        }

        public Recognizer(RecognizerGenome genome)
        {
            this.Genome = genome;
        }

        public Recognizer()
        {
        }
        public Dictionary<Phoneme,double> PhonemeProbabilitiesDictionary(AudioSample audioPhoneme)
        {
            Dictionary<Phoneme, double> recognitionDictionary = new Dictionary<Phoneme, double>();

            //Process similarity for each phoneme in the genome and add to the list
            RecognizerManager manager = RecognizerManager.Instance;
            foreach (Phoneme phoneme in genome.Phonemes)
            {

                //phoneme, similarity
                recognitionDictionary.Add(
                    phoneme, manager.Similarity(phoneme, audioPhoneme)
                );

            }

            //Order the dictionary by greater similarity
            recognitionDictionary.OrderByDescending(pair => pair.Value);

            return recognitionDictionary;
        }

        public List<Phoneme> ProbablePhonemes(AudioSample audioPhoneme)
        {
            //Sort the dictionary by probabilities
            IOrderedEnumerable<KeyValuePair<Phoneme, double>> orderedEnumerable =
                this.PhonemeProbabilitiesDictionary(audioPhoneme).OrderByDescending(pair => pair.Value);

            //Put the dictionary ordered keys in a list
            List<Phoneme> list = new List<Phoneme>();
            foreach (KeyValuePair<Phoneme, double> pair in orderedEnumerable)
            {
                list.Add(pair.Key);
            }

            return list;
        }

        public List<Phoneme> ProbablePhonemes(AudioSample audioPhoneme, int count)
        {
            return ProbablePhonemes(audioPhoneme).GetRange(0, count);
        }

        public Phoneme RecognizePhoneme(AudioSample audioPhoneme)
        {
            return ProbablePhonemes(audioPhoneme).First();
        }

    }
}
