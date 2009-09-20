using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzySpeech.Model
{
    class ObjectCreator
    {
        public RecognizerGenome CreateRandomGenome(GeneticSettings settings, List<string> phonemeNames)
        {
            RecognizerGenome genome = new RecognizerGenome(settings.GetInitialColorsCopy(), settings.GetInitialLengthsCopy());
            foreach (string phonemeName in phonemeNames)
            {
                genome.Phonemes.Add(CreateRandomPhoneme(phonemeName, settings));
            }

            return genome;
        }


        //Singleton
        static ObjectCreator instance;
        public static ObjectCreator Instance
        {
            get
            {
                if (ObjectCreator.instance == null) instance = new ObjectCreator();
                return instance;
            }
        }


        /// <summary>
        /// Creates a random phoneme with random colors in its bands
        /// </summary>
        /// <param name="numberOfBands">
        /// The number of color bands of the phoneme
        /// </param>
        /// <param name="colorsByBand">
        /// The number of colors used in each band of the phoneme.
        /// </param>
        /// <param name="genome">
        /// The genome from which the color definitions are taken from
        /// </param>
        /// <returns>
        /// The instance of the random phoneme.
        /// </returns>
        public Phoneme CreateRandomPhoneme(string name, GeneticSettings settings)
        {
            Phoneme newPhoneme = new Phoneme(settings.NumberOfBands, settings.ColorsByBand);
            //Assign a random color an its colorByBand - 1 upper neighbors
            int colorIndex;
            PhonemeBand band;
            for (int i = 0; i < newPhoneme.NumberOfBands; i++)
            {
                band = new PhonemeBand(settings.ColorsByBand);
                colorIndex = Common.Helper.Util.Random.Next(settings.NumberOfColors - settings.ColorsByBand + 1);
                for (int j = 0; j < newPhoneme.ColorsByBand; j++)
                {
                    band.ColorIDs[j] = colorIndex;
                    colorIndex++;
                }

                newPhoneme.Bands.Add(band);
            }


            //Assign a random length
            int lengthIndex = Common.Helper.Util.Random.Next(settings.NumberOfLengths);
            newPhoneme.LengthID = lengthIndex;

            //Assign the name
            newPhoneme.Name = name;

            return newPhoneme;


        }
    }
}
