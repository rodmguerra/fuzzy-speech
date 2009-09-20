using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Model.Collections;
using FuzzySpeech.Common.Helper;
using FuzzySpeech.Common.Interfaces;

namespace FuzzySpeech.Model
{
    public class PhonemeBand : ICommonObject
    {
        private int [] colorIDs;
        private Phoneme owner;

        public Phoneme OwnerPhoneme
        {
            get { return owner; }
            set { owner = value; }
        }
        public int NumberOfColors
        {
            get { return colorIDs.Length; }
        }

        public PhonemeBand(int numberOfColors)
        {
            colorIDs = new int[numberOfColors];
        }

        public int [] ColorIDs
        {
            get { return colorIDs; }
            set { colorIDs = value; }
        }

        public FuzzyColor GetColor (int colorIndex)
        {
            int colorID = ColorIDs[colorIndex];
            return OwnerPhoneme.OwnerGenome.Colors[colorID];
        }

        public object Clone()
        {
            PhonemeBand clone = new PhonemeBand(this.NumberOfColors);
            for (int i = 0; i < this.NumberOfColors; i++)
            {
                clone.ColorIDs[i] = this.ColorIDs[i];
            }

            return clone;

        }

        /// <summary>
        /// Mutates randomly one of the colors of the band
        /// </summary>
        /// <param name="mutationGenome"></param>
        /// <returns></returns>
        public PhonemeBand Mutate()
        {
            //Select a color to mutate in the selected band
            int mutatedColorIndex = Util.Random.Next(NumberOfColors);

            //Select which border the color will change in the mutation to
            int border;
            if (ColorIDs[mutatedColorIndex] == 0)
            {
                border = 1;
            }
            else if (ColorIDs[mutatedColorIndex] == this.OwnerPhoneme.OwnerGenome.Colors.Count - 1)
            {
                border = -1;
            }
            else
            {
                border = Util.Random.Next(2);
                border = (border == 0) ? 1 : -1;
            }

            //Selects the new color
            int mutatedColorID = ColorIDs[mutatedColorIndex] + border;

            //Mutates
            PhonemeBand mutated = (PhonemeBand) Clone();
            mutated.ColorIDs[mutatedColorIndex] = mutatedColorID;

            return mutated;
        }

        public override string ToString()
        {
            //Colors[0] = {x; x; x; x}
            //Colors[1] = {x; x; x; x}

            string output = "{";

            int i = 0;

            foreach (int colorID in colorIDs)
            {
                output += (OwnerPhoneme.OwnerGenome.Colors[colorID].Name + "; "); 
                i++;
            }

            return (output.Substring(0, output.Length - 2) + "}");

        }



    }
}
