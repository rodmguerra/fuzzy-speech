using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Model.Collections;
using FuzzySpeech.Helper;

namespace FuzzySpeech.Model
{
    class RecognizerGenome : CommonObject
    {
        private List<FuzzyColor> colors = new List<FuzzyColor>();
        private List<FuzzyLength> lengths = new List<FuzzyLength>();
        private RecognizerGenomePhonemeCollection phonemes; 

        public RecognizerGenome(IEnumerable<FuzzyColor> colors, IEnumerable<FuzzyLength> lengths)
        {
            this.Colors.AddRange(colors);
            this.Lengths.AddRange(lengths);

            phonemes = new RecognizerGenomePhonemeCollection(this);
        }

        /// <summary>
        /// Genome's color definitions.
        /// </summary>
        public List<FuzzyColor> Colors
        {
            get { return colors; }
        }
        
        /// <summary>
        /// Genome's length definitions
        /// </summary>
        public List<FuzzyLength> Lengths
        {
            get { return lengths; }
        }
        
        /// <summary>
        /// Genome's set of phonemes
        /// </summary>
        public RecognizerGenomePhonemeCollection Phonemes
        {
            get { return phonemes; }
            set { phonemes = value; }
        }     
   
        public object Clone()
        {
            
            //clone colors
            List<FuzzyColor> cloneColors = new List<FuzzyColor>();
            foreach (FuzzyColor color in colors)
            {
                cloneColors.Add( (FuzzyColor) color.Clone());
            }

            //clone lengths
            List<FuzzyLength> cloneLengths = new List<FuzzyLength>();
            foreach (FuzzyLength length in lengths)
            {
                cloneLengths.Add( (FuzzyLength) length.Clone());
            }

            //clone phonemes (the references must be pointed to the new colors and lengths)
            RecognizerGenome clone = new RecognizerGenome(cloneColors,cloneLengths);
            foreach (Phoneme phoneme in phonemes)
            {
                clone.Phonemes.Add( (Phoneme) phoneme.Clone());
            }

            

            return clone;
        }

        /// <summary>
        /// Mutation operator, creates a new genome
        /// by changing slightly the first genome
        /// </summary>
        /// <returns>
        /// The mutated genome.
        /// </returns>
        public RecognizerGenome Mutate()
        {
            RecognizerGenome mutated = (RecognizerGenome) Clone();
            int mutatedPartIndex = Util.Random.Next(3);
            int mutatedPartItemIndex;

            switch (mutatedPartIndex)
            {
                //mutate a color
                case 0:
                    mutatedPartItemIndex = Util.Random.Next(Colors.Count);
                    mutated.Colors[mutatedPartItemIndex] = (FuzzyColor)mutated.Colors[mutatedPartItemIndex].Mutate();
                    mutated.Colors.Sort();
                    break;
                //mutate a length
                case 1:
                    mutatedPartItemIndex = Util.Random.Next(Lengths.Count);
                    mutated.Lengths[mutatedPartItemIndex] = (FuzzyLength)mutated.Lengths[mutatedPartItemIndex].Mutate();
                    mutated.Lengths.Sort();
                    break;
                //mutate a phoneme
                case 2:
                    mutatedPartItemIndex = Util.Random.Next(Phonemes.Count);
                    mutated.Phonemes[mutatedPartItemIndex] = mutated.Phonemes[mutatedPartItemIndex].Mutate();
                    break;
            }

            return mutated;
        }

        /// <summary>
        /// Cross Over operator, creates a son genome mixing randomly features of this
        /// genome (the father) and other genome (the mather)
        /// </summary>
        /// <param name="other">
        /// The other genome.
        /// </param>
        /// <returns>
        /// The son genome resulting of the crossover of the two genomes
        /// </returns>
        public RecognizerGenome CrossOver(RecognizerGenome mother)
        {
            //Son starts the alghorithm exactly like father
            RecognizerGenome son = (RecognizerGenome) this.Clone();
            int fromMother;

            //Randomly chooses which colors will com from mother
            for (int i = 0; i < son.Colors.Count; i++)
            {
                fromMother = Util.Random.Next(2);
                if (fromMother == 1)
                {
                    son.Colors[i] = (FuzzyColor) mother.Colors[i].Clone();
                }
            }

            //Randomly chooses which lengths will com from mother
            for (int i = 0; i < son.Lengths.Count; i++)
            {
                fromMother = Util.Random.Next(2);
                if (fromMother == 1)
                {
                    son.Lengths[i] = (FuzzyLength) mother.Lengths[i].Clone();
                }
            }

            //Randomly chooses which phonemes will com from mother (other)
            for (int i = 0; i < son.Phonemes.Count; i++)
            {
                fromMother = Util.Random.Next(2);
                if (fromMother == 1)
                {
                    son.Phonemes[i] = (Phoneme) mother.Phonemes[i].Clone();
                }
            }

            return son;
        }


        public override string ToString()
        {
            //Bands[0] = { xxx }
            //Bands[1] = { xxx }


            string output = "Colors:\r\n";

            int i = 0;
            foreach (FuzzyColor color in Colors)
            {
                output += (" - " + color.Name + " = " + color + "\r\n");
                i++;
            }

            output += "\r\nLengths:\r\n";

            i = 0;
            foreach (FuzzyLength length in Lengths)
            {
                output += (" - " + length.Name + " = " + length + "\r\n");
                i++;
            }

            output += "\r\n";

            i = 0;
            foreach (Phoneme phoneme in Phonemes)
            {
                output += ("Phonemes[" + i + "]: \r\n" + phoneme + "\r\n");
                i++;
            }

            return output;

        }




    }
}
