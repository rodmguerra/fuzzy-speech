using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzySpeech.Helper
{
    class Util
    {
        /// <summary>
        /// Mel Scales an array of amplitudes indexed by frequency.
        /// </summary>
        /// <param name="amplitudesByFrequency">
        /// The amplitude values for each frequency.
        /// </param>
        /// <param name="maximumFrequency">
        /// The top frequency represented in amplitude values array.
        /// </param>
        /// <returns>
        /// The mel scaled array.
        /// </returns>
        public static double[] MelScale(double[] amplitudesByFrequency, double maximumFrequency)
        {
            //Creates a vector containing amplitudes in mel scale
            double[] amplitudesByMel = new double[amplitudesByFrequency.Length];
            
            double maximumMels = GetMelsFromFrequency(maximumFrequency);
            
            //The step in frequency or mel from one amplitude to another
            double frequencyGridSize = maximumFrequency / amplitudesByFrequency.Length;
            double melGridSize = maximumMels / amplitudesByMel.Length;

            double melGridBottom = 0;
            double melGridTop = melGridSize;
            int topFrequencyIndex;
            int bottomFrequencyIndex = 0;
            for (int i = 0; i < amplitudesByFrequency.Length; i++)
            {
                //Discover the maximum frequency containing the amplitudes to be averaged
                double bottomFrequency = GetFrequencyFromMels(melGridBottom);
                double topFrequencyGrid = GetFrequencyFromMels(melGridTop);

                bottomFrequencyIndex = (int) (GetFrequencyFromMels(melGridBottom) / frequencyGridSize);
                topFrequencyIndex =  (int) Math.Round(GetFrequencyFromMels(melGridTop) / frequencyGridSize)-1;

                //if no range was selected, selects the inferior index as the range
                if (topFrequencyIndex < bottomFrequencyIndex)
                {
                    topFrequencyIndex = bottomFrequencyIndex;
                }

                //Averages the selected amplitudes
                for(int j=bottomFrequencyIndex; j<=topFrequencyIndex; j++)
                {
                    amplitudesByMel[i] += amplitudesByFrequency[j];
                }
                amplitudesByMel[i] /= ((topFrequencyIndex-bottomFrequencyIndex)+1);
                
                //Goes to the next mel grid
                melGridBottom += melGridSize;
                melGridTop += melGridSize;
            }

            return amplitudesByMel;
            
        }

        /// <summary>
        /// Gets the correspondent frequency in mel scale from a hertz scaled frequency. 
        /// </summary>
        /// <param name="frequency">
        /// The frequency in hertz.
        /// </param>
        /// <returns>
        /// The correspondent frequency in mels.
        /// </returns>
        public static double GetMelsFromFrequency(double frequency)
        {
            return 1127.01048 * Math.Log(1 + frequency / 700);
            //return (1000 / Math.Log10(2)) * Math.Log10(1 + frequency / 1000); //fant
        }
        
        /// <summary>
        /// Gets the correspondent frequency in herz scale from a mel scaled frequency.
        /// </summary>
        /// <param name="mel">
        /// The freuency in mels.
        /// </param>
        /// <returns>
        /// The correspondent frequency in herz.
        /// </returns>
        public static double GetFrequencyFromMels(double mel)
        {
            return 700 * (Math.Exp(mel / 1127.01048) - 1);  
        }

        /// <summary>
        /// Creates a new array from a portion of a given double array.
        /// </summary>
        /// <param name="values">
        /// The source array.
        /// <param name="startIndex">
        /// The start index of the desired range.
        /// </param>
        /// <param name="length">
        /// The length of the desired range.
        /// </param>
        /// <returns>
        /// The new array comprising the desired portion.
        /// </returns>
        public static double[] GetRange(double[] values, int startIndex, int length)
        {
            //sort descendant the desired positions of the array
            double[] range = new double[length];
            Array.Copy(values, startIndex, range, 0, length);

            return range;
        }

        /// <summary>
        /// Calculates the average from the n percent greatest values of a given array.
        /// </summary>
        /// <param name="values">
        /// The source array.
        /// </param>
        /// <param name="nPercent">
        /// The percentage of greatest values of the array used in the average.
        /// </param>
        /// <returns>
        /// The Max-N value, the averaged n percent greatest values of the array.
        /// </returns>
        public static double Max(double[] values, int nPercent)
        {
            //Get the nMax absolute value
            int nMax = values.Length * nPercent / 100;
            if (nMax == 0) nMax++;

            //sort descendant the desired positions of the array
            double[] copy = new double[values.Length];
            Array.Copy(values, copy, values.Length);
            Array.Sort(copy);
            Array.Reverse(copy);

            //average the nMax greater values of the range
            double average = 0;
            for (int i = 0; i < nMax; i++)
            {
                average += copy[i] ;
            }

            average /= nMax;

            return average;
        }

        /// <summary>
        /// Averages the n percent middle values of the array, i.e. averages the array excluding the borders,
        /// </summary>
        /// <param name="values">
        /// The source array,
        /// </param>
        /// <param name="nPercent">
        /// The percentage of the array used in average,
        /// </param>
        /// <returns></returns>
        public static double Average(double[] values, int nPercent)
        {
            //Get the n absolute value
            int n = values.Length * nPercent / 100;
            if (n == 0) n++;
            int startIndex = (values.Length - n)/2;

            //sort descendant the desired positions of the array
            double[] copy = new double[values.Length];
            Array.Copy(values, copy, values.Length);
            Array.Sort(copy);

            //average the nMax greater values of the range
            double average = 0;
            for (int i = startIndex; i < n; i++)
            {
                average += copy[i];
            }

            average /= n;

            return average;
        }


        /// <summary>
        /// Creates an array with a given number the ranges whose values are an average from its n percent greatest values.
        /// </summary>
        /// <param name="values">
        /// The source array.
        /// </param>
        /// <param name="nPercentMax">
        /// The percentage of values used in each range.
        /// </param>
        /// <param name="numberOfRanges">
        /// The number of ranges of the output array.
        /// </param>
        /// <returns>
        /// The Max-N array, an array comprising the Max-N operation in its ranges
        /// </returns>
        public static double[] Max(double[] values, int nPercentMax, int numberOfRanges)
        {
            
            double[] averagedRanges = new double[numberOfRanges];
            int rangeLength = values.Length / numberOfRanges;
            double[] range = null;

            //Process all ranges except the last
            for (int i = 0; i < numberOfRanges-1; i++)
            {
                range = GetRange(values, i * rangeLength, rangeLength);
                averagedRanges[i] = Max(range, nPercentMax);
            }

            //Process last range (treated separated because the array could not be a multiple of the number of ranges)
            int lastRangeStartIndex = (numberOfRanges - 1) * rangeLength;
            int lastRangeLength = values.Length - lastRangeStartIndex;
            range = GetRange(values, lastRangeStartIndex, lastRangeLength);
            averagedRanges[numberOfRanges - 1] = Max(range, nPercentMax);

            return averagedRanges;
        }


        public static double MaxN (IEnumerable<double> values, int nValues)
        {
            List<double> valuesList = new List<double>(values);
            valuesList.Sort();
            valuesList.Reverse();

            //Keep the N greater values
            int removedCount = valuesList.Count - nValues;

            bool last = true;
            for (int i = 0; i < removedCount; i++)
            {
                if (last) valuesList.RemoveAt(valuesList.Count - 1);
                else valuesList.RemoveAt(0);

                last = !last;
            }

            //Return the average of the N greater values
            return valuesList.Average();
        }

        public static double MaxNPercent(IEnumerable<double> values, double nPercent)
        {
            int nValues = (int) (values.Count() * nPercent);
            return MaxN(values, nValues);
        }

        public static List<int> RandomInt32List(int maxValue, int count)
        {
            List<int> list = new List<int>();
            for (int i = 0; i < count; i++)
            {
                int value = Random.Next(maxValue);
                while(list.Contains(value))
                {
                    value = Random.Next(maxValue);
                }

                list.Add(value);

            }

            return list;

        }

        public static readonly Random Random = new Random();

    }
}
