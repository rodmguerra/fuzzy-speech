using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzySpeech.Model
{
    class GeneticSettings
    {
        private int numberOfBands;
        private int colorsByBand;
        private double lengthMutationStep;

        public double LengthMutationStep
        {
            get { return lengthMutationStep; }
            set { lengthMutationStep = value;
                foreach (FuzzyLength length in initialLengths)
                {
                    length.MutationStep = LengthMutationStep;
                }
            }
        }
        private double colorMutationStep;
       

        public int NumberOfBands
        {
            get { return numberOfBands; }
            set { numberOfBands = value; }
        }
        
        public int ColorsByBand
        {
            get { return colorsByBand; }
            set { colorsByBand = value; }
        }
        

        public double ColorMutationStep
        {
            get { return colorMutationStep; }
            set { colorMutationStep = value;
                foreach (FuzzyColor color in initialColors)
                {
                    color.MutationStep = ColorMutationStep;
                }
            }
        }

        private int numberOfColors;

        List<FuzzyColor> initialColors = new List<FuzzyColor>();

        public void SetInitialColors (IEnumerable<FuzzyColor> colors)
        {
            initialColors.Clear();
            initialColors.AddRange(colors);

            foreach (FuzzyColor color in colors)
            {
                color.MutationStep = ColorMutationStep;
            }
        }

        public List<FuzzyColor> GetInitialColorsCopy ()
        {
            List<FuzzyColor> initialColorsCopy = new List<FuzzyColor>();
            foreach (FuzzyColor color in initialColors)
            {
                initialColorsCopy.Add((FuzzyColor) color.Clone());
            }

            return initialColorsCopy;
        }


        List<FuzzyLength> initialLengths = new List<FuzzyLength>();

        public void SetInitialLength(IEnumerable<FuzzyLength> lengths)
        {
            initialLengths.Clear();
            initialLengths.AddRange(lengths);

            foreach (FuzzyLength length in lengths)
            {
                length.MutationStep = LengthMutationStep;
            }
        }

        public List<FuzzyLength> GetInitialLengthsCopy()
        {
            List<FuzzyLength> initialLengthsCopy = new List<FuzzyLength>();
            foreach (FuzzyLength length in initialLengths)
            {
                initialLengthsCopy.Add((FuzzyLength) length.Clone());
            }

            return initialLengthsCopy;
        }

        public int NumberOfColors
        {
            get { return initialColors.Count; }
        }

        public int NumberOfLengths
        {
            get { return initialLengths.Count; }
        }



    }
}
