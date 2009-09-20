using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using FuzzySpeech.UI;
using FuzzySpeech.Model;
using FuzzySpeech.Audio;
using FuzzySpeech.Action;
using FuzzySpeech.Managers;
using FuzzySpeech.Common.Helper;
using System.Xml;
using System.IO;

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

            /*
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
            List<Phoneme> list = recognizer.ProbablePhonemes(sample);
            */

            using (XmlTextWriter xmlWriter = new XmlTextWriter("son.xml", System.Text.Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                son.ToXml().WriteContentTo(xmlWriter);
            }

            RecognizerGenome son2 = new RecognizerGenome(
                SettingsFactory.InstantiateHalavatiColorList(),
                SettingsFactory.InstantiateHalavatiLengthList());

            using (StreamReader reader = new StreamReader("son.xml"))
            {
                XmlDocument document = new XmlDocument();
                document.InnerXml = reader.ReadToEnd();
                son2.FromXml(document);
            }

            using (XmlTextWriter xmlWriter = new XmlTextWriter("son2.xml", System.Text.Encoding.UTF8))
            {
                xmlWriter.Formatting = Formatting.Indented;
                xmlWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='UTF-8'");
                son2.ToXml().WriteContentTo(xmlWriter);
            }
/*
            //Testando o extrator
            Extractor.FeatureExtractor extractor = new Extractor.FeatureExtractor();
            AudioSample amostra = extractor.Extract(Extractor.ExtractorManager.Instance.ReadAudioSampleFromFile(@"D:\Documents and Settings\Rodrigo M Guerra\Desktop\waves\aaa.wav",AmplitudeType.Magnitude));
            amostra.DivideAmplitudesPer(amostra.GetMaximumAmplitude());






            //Testando o trainer
            GeneticTrainer trainer = new GeneticTrainer();
            trainer.FitnessThreshold = 0.99;
            trainer.Settings = SettingsFactory.InstantiateHalavatiGeneticSettings();
            Dictionary<AudioSample,string> spDic = new Dictionary<AudioSample,string>();
            //Cria um sample dictionary (64 * 3 samples)
            for (int count = 0; count < 64; count++)
            {
                AudioSample sample = new AudioSample(trainer.Settings.NumberOfBands);
                AudioSample sample2 = new AudioSample(trainer.Settings.NumberOfBands);
                AudioSample sample3 = new AudioSample(trainer.Settings.NumberOfBands);

                int maxFrames = 100;
                int currentMaxFrames = (int)((double)maxFrames * Helper.Util.Random.NextDouble());
                for (int i = 0; i < currentMaxFrames; i++)
                {
                    AudioFrame frame = new AudioFrame(trainer.Settings.NumberOfBands);
                    AudioFrame frame2 = new AudioFrame(trainer.Settings.NumberOfBands);
                    AudioFrame frame3 = new AudioFrame(trainer.Settings.NumberOfBands);
                    for (int j = 0; j < frame.NumberOfBands; j++)
                    {
                        frame[j] = Helper.Util.Random.NextDouble();
                        //altera em até 5% pra cima
                        frame2[j] = frame[j] += Helper.Util.Random.NextDouble()/20;

                        //altera em até 5% pra baixo
                        frame3[j] = frame[j] -= Helper.Util.Random.NextDouble()/20;
                    }
                    sample.Frames.Add(frame);
                    sample2.Frames.Add(frame2);
                    sample3.Frames.Add(frame3);

                    
                    
                }

                spDic.Add(sample , "p" + count);
                spDic.Add(sample2, "p" + count);
                spDic.Add(sample3, "p" + count);
                
            }

            trainer.SamplePhonemeDictionary = spDic;
            RecognizerGenome trainedGenome = trainer.Train();
            int kakka = 0;

 */
 
            //Testando o extrator
            Extractor.FeatureExtractor extractor = new Extractor.FeatureExtractor();

            DirectoryInfo directoryInfo = new DirectoryInfo(@"D:\Documents and Settings\Rodrigo M Guerra\Desktop\inputDirectory");
            FileInfo[] filesInfo = directoryInfo.GetFiles();
            double maximumMagnitute = Double.MinValue;
            Dictionary<AudioSample, string> spDic = new Dictionary<AudioSample, string>();
            foreach (FileInfo fileInfo in filesInfo)
            {
                AudioSample amostra = extractor.Extract(Extractor.ExtractorManager.Instance.ReadAudioSampleFromFile(fileInfo.FullName,AmplitudeType.Decibel));
                spDic.Add(amostra, fileInfo.Name[0].ToString());

                double sampleMagnitude = amostra.GetMaximumAmplitude();
                if (sampleMagnitude > maximumMagnitute) maximumMagnitute = sampleMagnitude;                             
            
            }

            foreach (KeyValuePair<AudioSample, string> spPair in spDic)
            {
                spPair.Key.DivideAmplitudesPer(maximumMagnitute);
            }


            //Testando o trainer
            GeneticTrainer trainer = new GeneticTrainer();
            trainer.FitnessThreshold = 0.99;
            trainer.Settings = SettingsFactory.InstantiateHalavatiGeneticSettings();
            trainer.SamplePhonemeDictionary = spDic;
            RecognizerGenome trainedGenome = trainer.Train();
            int kakka = 0;
        }
    }
}
