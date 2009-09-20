using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Model.Collections;
using FuzzySpeech.Common.Helper;
using FuzzySpeech.Common.Interfaces;
using System.Xml;

namespace FuzzySpeech.Model
{
    public class RecognizerGenome : ICommonObject, IXmlable
    {
        private List<FuzzyColor> colors = new List<FuzzyColor>();
        private List<FuzzyLength> lengths = new List<FuzzyLength>();
        private RecognizerGenomePhonemeCollection phonemes; 

        public RecognizerGenome(IEnumerable<FuzzyColor> colors, IEnumerable<FuzzyLength> lengths)
        {
            this.Colors.AddRange(colors);
            this.Lengths.AddRange(lengths);

            foreach (FuzzyColor color in colors)
                colorNames.Add(color.Name);

            foreach (FuzzyLength length in lengths)
                lengthNames.Add(length.Name);

            phonemes = new RecognizerGenomePhonemeCollection(this);
        }

        protected List<string> colorNames = new List<string>();
        protected List<string> lengthNames = new List<string>();

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

        protected void SortColors ()
        {
            this.colors.Sort();

            for (int i=0; i<colors.Count; i++)
            {
                colors[i].Name = colorNames[i];
            }

        }

        protected void SortLengths()
        {
            this.lengths.Sort();

            for (int i = 0; i < lengths.Count; i++)
            {
                lengths[i].Name = lengthNames[i];
            }

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
                    mutated.SortColors();
                    break;
                //mutate a length
                case 1:
                    mutatedPartItemIndex = Util.Random.Next(Lengths.Count);
                    mutated.Lengths[mutatedPartItemIndex] = (FuzzyLength)mutated.Lengths[mutatedPartItemIndex].Mutate();
                    mutated.SortLengths();
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

            //Randomly chooses which colors will come from mother
            for (int i = 0; i < son.Colors.Count; i++)
            {
                fromMother = Util.Random.Next(2);
                if (fromMother == 1)
                {
                    son.Colors[i] = (FuzzyColor) mother.Colors[i].Clone();
                }

                son.Colors[i].Name = this.colorNames[i]; //father`s color name
            }

            //Randomly chooses which lengths will come from mother
            for (int i = 0; i < son.Lengths.Count; i++)
            {
                fromMother = Util.Random.Next(2);
                if (fromMother == 1)
                {
                    son.Lengths[i] = (FuzzyLength) mother.Lengths[i].Clone();
                }
            }

            //Randomly chooses which phonemes will come from mother (other)
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
            return this.ToXml().InnerXml;
            /*
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
            */
        }





        #region IXmlable Members

        public void FromXml (XmlDocument xmlDocument)
        {
            System.Globalization.CultureInfo originalCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            XmlElement genomeElement = xmlDocument.GetElementsByTagName("RecognizerGenome")[0] as XmlElement;
            XmlElement colorsElement = genomeElement.GetElementsByTagName("Colors")[0] as XmlElement;
            XmlElement lengthsElement = genomeElement.GetElementsByTagName("Lengths")[0] as XmlElement;
            XmlElement phonemesElement = genomeElement.GetElementsByTagName("Phonemes")[0] as XmlElement;

            int numberOfBands = ((XmlElement)phonemesElement.GetElementsByTagName("Phoneme")[0])
                .GetElementsByTagName("Band").Count;

            int colorsByBand = ((XmlElement)(((XmlElement)phonemesElement.GetElementsByTagName("Phoneme")[0])
                .GetElementsByTagName("Band")[0]))
                .GetElementsByTagName("BandColor").Count;
                

            //Build Colors
            List<FuzzyColor> colors = new List<FuzzyColor>();
            XmlNodeList colorNodes = colorsElement.GetElementsByTagName("Color");
            foreach (XmlNode colorNode in colorNodes)
            {
                XmlElement pointsElement = ((XmlElement)colorNode).GetElementsByTagName("Points")[0] as XmlElement;
                string name = colorNode.Attributes["name"].Value;

                double startPoint = Convert.ToDouble(pointsElement.GetElementsByTagName("StartPoint")[0].Attributes["value"].Value);
                double firstMaximum = Convert.ToDouble(pointsElement.GetElementsByTagName("FirstMaximum")[0].Attributes["value"].Value);
                double lastMaximum = Convert.ToDouble(pointsElement.GetElementsByTagName("LastMaximum")[0].Attributes["value"].Value);
                double endPoint = Convert.ToDouble(pointsElement.GetElementsByTagName("EndPoint")[0].Attributes["value"].Value);

                FuzzyColor color = new FuzzyColor(name, startPoint, firstMaximum, lastMaximum, endPoint);
                colors.Add(color);
            }

            //Build Lengths
            List<FuzzyLength> lengths = new List<FuzzyLength>();
            XmlNodeList lengthNodes = lengthsElement.GetElementsByTagName("Length");
            foreach (XmlNode lengthNode in lengthNodes)
            {
                XmlElement pointsElement = ((XmlElement)lengthNode).GetElementsByTagName("Points")[0] as XmlElement;
                string name = lengthNode.Attributes["name"].Value;

                double startPoint = Convert.ToDouble(pointsElement.GetElementsByTagName("StartPoint")[0].Attributes["value"].Value);
                double firstMaximum = Convert.ToDouble(pointsElement.GetElementsByTagName("FirstMaximum")[0].Attributes["value"].Value);
                double lastMaximum = Convert.ToDouble(pointsElement.GetElementsByTagName("LastMaximum")[0].Attributes["value"].Value);
                double endPoint = Convert.ToDouble(pointsElement.GetElementsByTagName("EndPoint")[0].Attributes["value"].Value);

                FuzzyLength length = new FuzzyLength(name, startPoint, firstMaximum, lastMaximum, endPoint);
                lengths.Add(length);
            }

            this.colors.Clear();
            this.lengths.Clear();
            this.phonemes.Clear();
            this.colors = colors;
            this.lengths = lengths;

            RecognizerGenome genome = this;

            //Build Phonemes
            XmlNodeList phonemeNodes = phonemesElement.GetElementsByTagName("Phoneme");
            foreach (XmlNode phonemeNode in phonemeNodes)
            {
                XmlElement bandsElement = ((XmlElement)phonemeNode).GetElementsByTagName("Bands")[0] as XmlElement;
                
                XmlNodeList bandNodes = bandsElement.GetElementsByTagName("Band");
 

                Phoneme phoneme = new Phoneme(numberOfBands, colorsByBand);
                phoneme.Name = phonemeNode.Attributes["name"].Value;
                genome.phonemes.Add(phoneme);

                FuzzyLength length = genome.Lengths.Find(innerLength => innerLength.Name == phonemeNode.Attributes["length"].Value);
                phoneme.LengthID = genome.Lengths.IndexOf(length);

                foreach(XmlNode bandNode in bandNodes)
                {
                    XmlElement bandElement = bandNode as XmlElement;
                    PhonemeBand band = new PhonemeBand(colorsByBand);
                    phoneme.Bands.Add(band);
                    
                    XmlElement bandColorsElement = bandElement.GetElementsByTagName("BandColors")[0] as XmlElement;
                    XmlNodeList bandColorNodes = bandColorsElement.GetElementsByTagName("BandColor");

                    int i=0;
                    foreach( XmlNode bandColorNode in bandColorNodes)
                    {
                        XmlElement bandColorElement = (XmlElement) bandColorNode;
                        FuzzyColor color = genome.Colors.Find(innerColor => innerColor.Name == bandColorElement.Attributes["name"].Value);
                        band.ColorIDs[i] = genome.Colors.IndexOf(color);

                        i++;                        
                    }
                }
                
                
            }           

            System.Threading.Thread.CurrentThread.CurrentCulture = originalCulture;

        }

        public XmlDocument ToXml()
        {
            System.Globalization.CultureInfo originalCulture = System.Threading.Thread.CurrentThread.CurrentCulture;
            System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            XmlDocument document = new XmlDocument();
            XmlElement genomeElement = document.CreateElement("RecognizerGenome");
            document.AppendChild(genomeElement);

            
            //Color definitions
            XmlElement colorsElement = document.CreateElement("Colors");
            genomeElement.AppendChild(colorsElement);
            foreach (FuzzyColor color in colors)
            {
                XmlElement colorElement = document.CreateElement("Color");
                colorsElement.AppendChild(colorElement);
                colorElement.SetAttribute("name", color.Name);

                XmlElement pointsElement = document.CreateElement("Points");
                colorElement.AppendChild(pointsElement);

                XmlElement startPointElement = document.CreateElement("StartPoint");
                pointsElement.AppendChild(startPointElement);
                startPointElement.SetAttribute("value", color.StartPoint.ToString());

                XmlElement firstMaximumElement = document.CreateElement("FirstMaximum");
                pointsElement.AppendChild(firstMaximumElement);
                firstMaximumElement.SetAttribute("value", color.FirstMaximum.ToString());

                XmlElement lastMaximumElement = document.CreateElement("LastMaximum");
                pointsElement.AppendChild(lastMaximumElement);
                lastMaximumElement.SetAttribute("value", color.LastMaximum.ToString());

                XmlElement endPointElement = document.CreateElement("EndPoint");
                pointsElement.AppendChild(endPointElement);
                endPointElement.SetAttribute("value", color.EndPoint.ToString());
            }

            //Length definitions
            XmlElement lengthsElement = document.CreateElement("Lengths");
            genomeElement.AppendChild(lengthsElement);
            foreach (FuzzyLength length in lengths)
            {
                XmlElement lengthElement = document.CreateElement("Length");
                lengthsElement.AppendChild(lengthElement);
                lengthElement.SetAttribute("name", length.Name);

                XmlElement pointsElement = document.CreateElement("Points");
                lengthElement.AppendChild(pointsElement);

                XmlElement startPointElement = document.CreateElement("StartPoint");
                pointsElement.AppendChild(startPointElement);
                startPointElement.SetAttribute("value", length.StartPoint.ToString());

                XmlElement firstMaximumElement = document.CreateElement("FirstMaximum");
                pointsElement.AppendChild(firstMaximumElement);
                firstMaximumElement.SetAttribute("value", length.FirstMaximum.ToString());

                XmlElement lastMaximumElement = document.CreateElement("LastMaximum");
                pointsElement.AppendChild(lastMaximumElement);
                lastMaximumElement.SetAttribute("value", length.LastMaximum.ToString());

                XmlElement endPointElement = document.CreateElement("EndPoint");
                pointsElement.AppendChild(endPointElement);
                endPointElement.SetAttribute("value", length.EndPoint.ToString());
            }

            //Phonemes
            XmlElement phonemesElement = document.CreateElement("Phonemes");
            genomeElement.AppendChild(phonemesElement);
            foreach (Phoneme phoneme in phonemes)
            {
                XmlElement phonemeElement = document.CreateElement("Phoneme");
                phonemesElement.AppendChild(phonemeElement);
                phonemeElement.SetAttribute("name", phoneme.Name);
                phonemeElement.SetAttribute("length", phoneme.Length.Name);

                XmlElement bandsElement = document.CreateElement("Bands");
                phonemeElement.AppendChild(bandsElement);

                int bandIndex = 0;
                foreach (PhonemeBand band in phoneme.Bands)
                {
                    XmlElement bandElement = document.CreateElement("Band");
                    bandsElement.AppendChild(bandElement);
                    //bandElement.SetAttribute("index", bandIndex.ToString());

                    XmlElement bandColorsElement = document.CreateElement("BandColors");
                    bandElement.AppendChild(bandColorsElement);

                    for (int bandColorIndex = 0; bandColorIndex < band.NumberOfColors; bandColorIndex++)
                    {
                        XmlElement bandColorElement = document.CreateElement("BandColor");
                        bandColorsElement.AppendChild(bandColorElement);

                        //bandColorElement.SetAttribute("index", bandColorIndex.ToString());
                        bandColorElement.SetAttribute("name", band.GetColor(bandColorIndex).Name);
                    }

                    bandIndex++;
                }
            }

            System.Threading.Thread.CurrentThread.CurrentCulture = originalCulture;

            return document;
        }

        #endregion
    }
}
