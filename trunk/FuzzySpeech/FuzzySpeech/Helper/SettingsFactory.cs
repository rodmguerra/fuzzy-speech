using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Model;

namespace FuzzySpeech.Helper
{
    class SettingsFactory
    {
        private static readonly double HalavatiColorMutationStep = 0.1;
        private static readonly double HalavatiLengthMutationStep = 1;
        private static readonly int HalavatiColorsByBand = 2;
        private static readonly int HalavatiNumberOfBands = 25;

        private static readonly FuzzyColor[] HalavatiColors = new FuzzyColor[] 
        {   new FuzzyColor("Black",  0.00, 0.00, 0.05, 0.35),
            new FuzzyColor("Blue",   0.00, 0.05, 0.22, 0.50),
            new FuzzyColor("Purple", 0.00, 0.23, 0.35, 0.63),
            new FuzzyColor("Red",    0.05, 0.36, 0.50, 0.80),
            new FuzzyColor("Yellow", 0.23, 0.51, 0.63, 0.93),
            new FuzzyColor("Green",  0.36, 0.63, 0.80, 0.99),
            new FuzzyColor("Cyan",   0.51, 0.81, 0.93, 0.99),
            new FuzzyColor("White",  0.63, 0.94, 0.99, 0.99)
        };

        private static readonly FuzzyLength[] HalavatiLengths = new FuzzyLength[]
        {
            new FuzzyLength("Very Short", 2,  2,  3,  5),
            new FuzzyLength("Short",      2,  2,  6, 10),
            new FuzzyLength("Medium",     2,  6, 12, 17),
            new FuzzyLength("Long",       8, 12, 20, 30),
            new FuzzyLength("Very Long", 12, 18, 69, 99)
        };

        public static GeneticSettings InstantiateHalavatiGeneticSettings()
        {
            GeneticSettings settings = new GeneticSettings();
            settings.ColorMutationStep = HalavatiColorMutationStep;
            settings.LengthMutationStep = HalavatiLengthMutationStep;
            settings.ColorsByBand = HalavatiColorsByBand;
            settings.NumberOfBands = HalavatiNumberOfBands;
            settings.SetInitialColors(HalavatiColors);
            settings.SetInitialLength(HalavatiLengths);
            return settings;
        }

        public static List<FuzzyColor> InstantiateHalavatiColorList()
        {
            List<FuzzyColor> list = new List<FuzzyColor>();
            foreach (FuzzyColor color in HalavatiColors)
            {
                list.Add( (FuzzyColor) color.Clone());
            }
            return list;
        }

        public static List<FuzzyLength> InstantiateHalavatiLengthList()
        {
            List<FuzzyLength> list = new List<FuzzyLength>();
            foreach (FuzzyLength length in HalavatiLengths)
            {
                list.Add( (FuzzyLength) length.Clone());
            }
            return list;
        }
    }
}
