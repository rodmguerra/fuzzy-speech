using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FuzzySpeech.UI;
using FuzzySpeech.Model;
using FuzzySpeech.Audio;
using FuzzySpeech.Action;
using FuzzySpeech.Managers;
using FuzzySpeech.Helper;

namespace FuzzySpeech
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());


            /*
            double mel = Util.GetMelsFromFrequency(1330);
            //MessageBox.Show(""+mel);
            //MessageBox.Show(""+Util.GetFrequencyFromMels(mel));

            int arraySize = 100;
            double[] afreqs = new double[arraySize];
            for (int i = 0; i < afreqs.Length; i++)
            {
                afreqs[i] = (arraySize - i) * arraySize;
            }

            int maxFreq = 100;
            double[] amels = Util.MelScale(afreqs, maxFreq);
            double[] avgamels = Util.Max(amels, 10, 25);
   
            string samels = "";
            for (int i = 0; i < amels.Length; i++)
            {
                samels += " " + amels[i];
            }

            string avgsamels = "";
            for (int i = 0; i < avgamels.Length; i++)
            {
                avgsamels += " " + avgamels[i];
            }            

            MessageBox.Show("Mels: " + samels + "\r\n" +
                            "Mels ranged: " + avgsamels);


            Util.Average(avgamels,80);
            */

            RecognizerGenome genome = new RecognizerGenome(
                SettingsFactory.InstantiateHalavatiColorList(),
                SettingsFactory.InstantiateHalavatiLengthList());

            GeneticSettings settings = SettingsFactory.InstantiateHalavatiGeneticSettings();
            
            int numberOfPhonemes = 62;
            for (int i=0; i<numberOfPhonemes; i++)
            {
                genome.Phonemes.Add(ObjectCreator.Instance.CreateRandomPhoneme("p" + i, settings));
            }

            // create a writer and open the file
            System.IO.TextWriter tw = new System.IO.StreamWriter("original.txt");

            // write a line of text to the file
            tw.WriteLine(genome.ToString());

            // close the stream
            tw.Close();

            tw = new System.IO.StreamWriter("mutation.txt");

            // write a line of text to the file
            tw.WriteLine(genome.Mutate().ToString());

            // close the stream
            tw.Close();

            RecognizerGenome father = new RecognizerGenome(
                SettingsFactory.InstantiateHalavatiColorList(),
                SettingsFactory.InstantiateHalavatiLengthList());
            tw = new System.IO.StreamWriter("father.txt");
            for (int i = 0; i < numberOfPhonemes; i++)
            {
                father.Phonemes.Add(ObjectCreator.Instance.CreateRandomPhoneme("p" + i, settings));
            }

            // write a line of text to the file
            tw.WriteLine(father.ToString());

            // close the stream
            tw.Close();

            RecognizerGenome mother = new RecognizerGenome(
                SettingsFactory.InstantiateHalavatiColorList(),
                SettingsFactory.InstantiateHalavatiLengthList());

            for (int i = 0; i < numberOfPhonemes; i++)
            {
                mother.Phonemes.Add(ObjectCreator.Instance.CreateRandomPhoneme("p" + i, settings));
            }

            tw = new System.IO.StreamWriter("mother.txt");

            // write a line of text to the file
            tw.WriteLine(mother.ToString());

            // close the stream
            tw.Close();

            RecognizerGenome son = father.CrossOver(mother);

            tw = new System.IO.StreamWriter("son.txt");

            // write a line of text to the file
            tw.WriteLine(son.ToString());

            // close the stream
            tw.Close();

            Recognizer recognizer = new Action.Recognizer(son);
            AudioSample sample = new AudioSample(settings.NumberOfBands);
            for (int i = 0; i < 50; i++)
            {
                AudioFrame frame = new AudioFrame(settings.NumberOfBands);
                for (int j = 0; j < frame.NumberOfBands; j++)
                {
                    frame[j] = Helper.Util.Random.NextDouble();
                }
                sample.Frames.Add(frame);
            }

            Dictionary<Phoneme,double> dictionary = recognizer.PhonemeProbabilitiesDictionary(sample);
        }
    }
}
