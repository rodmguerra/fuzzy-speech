using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Model;
using FuzzySpeech.Common.Audio;
using FuzzySpeech.Managers;
using FuzzySpeech.Common.Helper;

namespace FuzzySpeech.Action
{
    class GeneticTrainer
    {
        private bool isGenePoolSorted = false;

        private Dictionary<AudioSample, string> samplePhonemeDictionary = new Dictionary<AudioSample, string>();

        public Dictionary<AudioSample, string> SamplePhonemeDictionary
        {
            get { return samplePhonemeDictionary; }
            set { samplePhonemeDictionary = value; }
        }

        private double fitnessThreshold = 0.9;

        public double FitnessThreshold
        {
            get { return fitnessThreshold; }
            set { fitnessThreshold = value; }
        }

        GeneticSettings settings;

        public GeneticSettings Settings
        {
            get { return settings; }
            set { settings = value; }
        }

        private List<KeyValuePair<RecognizerGenome, double>> genePool = new List<KeyValuePair<RecognizerGenome, double>>();

        Recognizer recognizer = new Recognizer();

        public RecognizerGenome Train()
        {    
            //Validate parameters
            if (settings.NumberOfBands <= 0)
                throw new Exception("Invalid number of bands");

            if (settings.ColorsByBand <= 0)
                throw new Exception("Invalid number of colors in each band");

            int numberOfGenomes = 100;
            for (int i = 0; i < numberOfGenomes; i++)
            {
                RecognizerGenome genome = ObjectCreator.Instance.CreateRandomGenome(settings, samplePhonemeDictionary.Values.Distinct().ToList());
                this.AddToGenePool(genome);               
            }

            while (this.BestGenePoolFitness() < this.FitnessThreshold)
            {
                //Add a mutated copy of all original genomes to the genepool
                for (int i = 0; i < numberOfGenomes; i++)
                {
                    RecognizerGenome genome = genePool[i].Key.Mutate();
                    this.AddToGenePool(genome);
                }

                //Add the crossovers of the randomly choosen mates from the original genomes to the genepool
                List<int> matesList = Common.Helper.Util.RandomInt32List(numberOfGenomes, numberOfGenomes);
                for (int i = 0; i < numberOfGenomes; i+=2)
                {
                    RecognizerGenome genome;
                    
                    //Crossover 1
                    genome = genePool[matesList[i]].Key.CrossOver(genePool[matesList[i + 1]].Key);
                    this.AddToGenePool(genome);

                    //Crossover 2
                    genome = genePool[matesList[i]].Key.CrossOver(genePool[matesList[i + 1]].Key);
                    this.AddToGenePool(genome);
                }

                //Throw out the worst 2/3 of the genomes
                SortGenePool();
                genePool.RemoveRange(genePool.Count - (genePool.Count*2 / 3), genePool.Count*2 / 3);
            }
                
            return this.BestGenePoolGenome();                
        }

        private void AddToGenePool(RecognizerGenome genome)
        {
            double fitness = this.Fitness(genome);

            //Add to the gene pool
            genePool.Add(new KeyValuePair<RecognizerGenome,double>(genome, fitness));
            isGenePoolSorted = false;
        }

        private RecognizerGenome BestGenePoolGenome()
        {
            if (!isGenePoolSorted) SortGenePool();
            return genePool[0].Key;
        }

        private void SortGenePool()
        {
            genePool = genePool.OrderByDescending(p => p.Value).ToList();
            isGenePoolSorted = true;
        }

        private double BestGenePoolFitness()
        {
            if (!isGenePoolSorted) SortGenePool();
            return genePool[0].Value;
        }

        public string CorrectPhonemeName(AudioSample sample)
        {
            return samplePhonemeDictionary[sample];
        }

        public List<AudioSample> CorrectSet(RecognizerGenome genome)
        {
            List<AudioSample> correctSet = new List<AudioSample>();
            recognizer.Genome = genome;

            foreach (KeyValuePair<AudioSample, string> samplePhonemePair in samplePhonemeDictionary)
            {
                AudioSample sample = samplePhonemePair.Key;
                if (recognizer.RecognizePhoneme(sample).Name == CorrectPhonemeName(sample))
                    correctSet.Add(samplePhonemePair.Key);
            }

            return correctSet;
        }

        public double Fitness(RecognizerGenome genome)
        {
            return (double)CorrectSet(genome).Count / (double)samplePhonemeDictionary.Count;
        }
    }
}
