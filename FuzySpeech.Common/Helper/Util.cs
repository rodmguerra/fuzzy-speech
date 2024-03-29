﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzySpeech.Common.Helper
{
    public class Util
    {
        

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


        /*
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

        */
        public static int BitReverse(int j, int nu)
        {
            int j2;
            int j1 = j;
            int k = 0;
            for (int i = 1; i <= nu; i++)
            {
                j2 = j1 / 2;
                k = 2 * k + j1 - 2 * j2;
                j1 = j2;
            }
            return k;
        }

        
        /*

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

         */

        public static double MaxN (IEnumerable<double> values, int nValues)
        {
            List<double> valuesList = new List<double>(values);
            valuesList.Sort();
            valuesList.Reverse();

            //Return the average of the N greater values
            return valuesList.GetRange(0,nValues).Average();
        }

        public static double MaxNPercent(IEnumerable<double> values, double nPercent)
        {
            int nValues = (int) (values.Count() * nPercent);
            if (nValues == 0) nValues++; //if percentage is tends to zero, uses max operator instead
            return MaxN(values, nValues);
        }


        public static double AverageN(IEnumerable<double> values, int nValues)
        {
            List<double> valuesList = new List<double>(values);
            valuesList.Sort();
            valuesList.Reverse();

            //Return the average of the N middle values
            return valuesList.GetRange( (valuesList.Count - nValues)/2, nValues).Average();
        }

        public static double AverageNPercent(IEnumerable<double> values, double nPercent)
        {
            int nValues = (int)(values.Count() * nPercent);
            if (nValues == 0) nValues++;
            return AverageN(values, nValues);
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
