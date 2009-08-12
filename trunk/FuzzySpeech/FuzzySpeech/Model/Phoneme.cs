using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Model.Collections;
using FuzzySpeech.Helper;

namespace FuzzySpeech.Model
{
    class Phoneme : CommonObject
    {
        private PhonemeBandCollection bands = null;
        private int lengthID = -1;
        private int colorsByBand;
        private int numberOfBands;
        private RecognizerGenome owner;

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public RecognizerGenome OwnerGenome
        {
            get { return owner; }
            set { owner = value; }
        }

        /// <summary>
        /// Creates a new phoneme.
        /// </summary>
        /// <param name="numberOfBands">
        /// The number of color bands of the phoneme
        /// </param>
        /// <param name="colorsByBand">
        /// The number of colors used in each band of the phoneme.
        /// </param>
        public Phoneme(int numberOfBands, int colorsByBand)
        {
            this.colorsByBand = colorsByBand;
            this.numberOfBands = numberOfBands;
            bands = new PhonemeBandCollection(this);
        }

        /// <summary>
        /// The lengthID of the phoneme in the Genome.
        /// </summary>
        public int LengthID
        {
            get { return lengthID; }
            set { lengthID = value; }
        }

        /// <summary>
        /// The length of the phoneme.
        /// </summary>
        public FuzzyLength Length
        {
            get { return OwnerGenome.Lengths[lengthID]; }
        }

        /// <summary>
        /// The color bands of the phoneme.
        /// </summary>
        public PhonemeBandCollection Bands
        {
            get { return bands; }
            set { bands = value; }
        }

        /// <summary>
        /// The number of color bands of the phoneme.
        /// </summary>
        public int NumberOfBands
        {
            get { return numberOfBands; }
        }

        /// <summary>
        /// The number of colors in each band of the phoneme
        /// </summary>
        public int ColorsByBand
        {
            get { return colorsByBand; }
        }

        /// <summary>
        /// Clones the phoneme by creating another phoneme as 
        /// an exact copy of all data in it, pointing the colors and length references to its genome's . Can change either one of
        /// colors of a band or the length
        /// </summary>
        /// <returns>
        /// The clone phoneme.
        /// </returns>
        public object Clone()
        {
            Phoneme clone = new Phoneme(this.Bands.Count, this.ColorsByBand);
            for (int i = 0; i < bands.Count; i++)
            {
                clone.Bands.Add( (PhonemeBand) this.Bands[i].Clone());
            }

            clone.LengthID = this.LengthID;
            clone.Name = this.Name;
                
            return clone;
        }

        /// <summary>
        /// Mutates the phoneme by creating another phoneme
        /// with the same data in it, but slightly changed
        /// </summary>
        /// <param name="genome">
        /// The genome with the mutation rules
        /// </param>
        /// <returns>The mutation phoneme.</returns>
        public Phoneme Mutate()
        {
            //Select a band to change one of its colors or a length to alter
            int mutatedBandIndex = Util.Random.Next(numberOfBands + 1);

            //length
            if (mutatedBandIndex == numberOfBands)
            {
                return this.MutateLength();
            }

            //band
            Phoneme mutated = (Phoneme) Clone();
            PhonemeBand mutatedBand = Bands[mutatedBandIndex].Mutate();
            mutated.Bands[mutatedBandIndex] = mutatedBand;
            return mutated;

        }

        /// <summary>
        /// Mutates the length of the phoneme to an adjacent length
        /// defined in the genome
        /// </summary>
        /// <param name="genome">
        /// The genome with the mutation rules
        /// </param>
        /// <returns></returns>
        public Phoneme MutateLength()
        {

            //Select which border the length will change in the mutation to
            int border;
            if (LengthID == 0)
            {
                border = 1;
            }
            else if (LengthID == this.OwnerGenome.Lengths.Count)
            {
                border = -1;
            }
            else
            {
                border = Util.Random.Next(2);
                border = (border == 0) ? 1 : -1;
            }

            //Selects the new length
            int mutatedLengthID = LengthID + border;

            //Mutates
            Phoneme mutated = (Phoneme) Clone();
            mutated.LengthID = mutatedLengthID;

            return mutated;
        }

               
        public override string ToString()
        {
            //Bands[0] = { xxx }
            //Bands[1] = { xxx }


            string output = String.Empty;

            int i = 0;

            foreach (PhonemeBand band in bands)
            {
                output += (" - Bands[" + i + "] = " + band + "\r\n");
                i++;
            }

            output += (" - Length = " + OwnerGenome.Lengths[LengthID].Name + "\r\n");


            return output;

        }

        

    }


}
