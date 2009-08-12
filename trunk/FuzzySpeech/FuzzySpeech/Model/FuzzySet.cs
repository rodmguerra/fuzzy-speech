using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Helper;

namespace FuzzySpeech.Model
{
    abstract class FuzzySet : CommonObject, IComparable
    {
        private double[] points = new double[4];

        public double[] Points
        {
            get { return points; }
            set { points = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// The start point of the trapezoid modeling the fuzzy set.
        /// </summary>
        public double StartPoint
        {
            get { return points[0]; }
            set { points[0] = value; SortPoints(); }
        }

        /// <summary>
        /// The first maximum of the trapezoid modeling the fuzzy set.
        /// </summary>
        public double FirstMaximum
        {
            get { return points[1]; }
            set { points[1] = value; SortPoints(); }
        }

        /// <summary>
        /// The last maximum of the trapezoid modeling the fuzzy set.
        /// </summary>
        public double LastMaximum
        {
            get { return points[2]; }
            set { points[2] = value; SortPoints(); }
        }

        /// <summary>
        /// The end point of the trapezoid modeling the fuzzy set.
        /// </summary>
        public double EndPoint
        {
            get { return points[3]; }
            set { points[3] = value; SortPoints(); }
        }

        /// <summary>
        /// Creates a new fuzzy set using into a trapezoid limited by the points in parameter.
        /// The points represent the values in which the similarity to the set changes.
        /// </summary>
        /// <param name="startPoint">
        /// The start point of the set, values lesser than this point have similarity = 0.
        /// </param>
        /// <param name="firstMaximum">
        /// The first maximum of the set, values from the start point to this point have similarity between 0 (start point)
        /// and 1 (first maximum)
        /// </param>
        /// <param name="lastMaximum">
        /// The last maximum of the set, values from the first to the last maximum have similarity = 1
        /// </param>
        /// <param name="endPoint">
        /// The end point of the set, values from the last maximum to this point have similarity between 1 (last maximum)
        /// and 0 (end point.
        /// </param>
        public FuzzySet(string name, double startPoint,
            double firstMaximum,
            double lastMaximum,
            double endPoint)
        {
            this.Name = name;
            points[0] = startPoint;
            points[1] = firstMaximum;
            points[2] = lastMaximum;
            points[3] = endPoint;

            SortPoints();
        }

        public abstract object Clone();

        /// <summary>
        /// Calculates the similarity of a value to this set.
        /// </summary>
        /// <param name="value">
        /// The value to be compared to the set.
        /// </param>
        /// <returns>
        /// The similarity of the value to this set.
        /// </returns>
        public double SimilarityTo(double value)
        {
            //lesser then set
            if (value < StartPoint)
            {
                return 0;
            }

            //at ascending curve
            if (value < FirstMaximum)
            {
                return ((double)value - StartPoint) / (FirstMaximum - StartPoint);
            }

            //at constant curve
            if (value < LastMaximum)
            {
                return 1;
            }

            //at descendant curve
            if (value < EndPoint)
            {
                return (EndPoint - (double)value) / (EndPoint - LastMaximum);
            }

            //greater then set
            return 0;
        }

        protected void SortPoints()
        {
            Array.Sort(points);
        }

        public override string ToString()
        {
            string output = "{";

            foreach (double point in points)
            {
                output += (point + "; ");
            }

            return output.Substring(0, output.Length - 2) + "}";

        }

        public int CompareTo(object otherObject)
        {
            
            if (!(otherObject is FuzzySet))
                throw new ArgumentException("Object is not a FuzzySet");
            
            int comparison;
            FuzzySet other = otherObject as FuzzySet;
            
            //The lesser set is the first to start
            comparison = StartPoint.CompareTo(other.StartPoint);
            if (comparison != 0) return comparison;

            //Starting in same point, the lesser set is the one that ends first
            comparison = EndPoint.CompareTo(other.EndPoint);
            if (comparison != 0) return comparison;

            //Tie break 1: first maximum
            comparison = FirstMaximum.CompareTo(other.FirstMaximum);
            if (comparison != 0) return comparison;

            //Final try: LastMaximum
            comparison = LastMaximum.CompareTo(other.LastMaximum);
            return comparison;

        }

        private double mutationStep = 0.01;

        public double MutationStep
        {
            get { return mutationStep; }
            set { mutationStep = value; }
        }

        public FuzzySet Mutate()
        {
            int pointIndex;
            int signal;

            //generate a signal to the mutation (-1 or +1)
            signal = Util.Random.Next(2);
            signal = (signal == 0) ? 1 : -1;

            //select the point that will be mutated (0 .. 3)
            pointIndex = Util.Random.Next(4);

            //create mutated set
            FuzzySet mutated = (FuzzySet) this.Clone();
            mutated.Points[pointIndex] += (signal * mutationStep);

            return mutated;
        }


   



    }
}
