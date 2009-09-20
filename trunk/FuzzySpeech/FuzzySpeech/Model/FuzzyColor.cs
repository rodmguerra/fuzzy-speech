using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Common.Helper;

namespace FuzzySpeech.Model
{
    public class FuzzyColor : FuzzySet
    {
        public FuzzyColor(string name, double startPoint,
            double firstMaximum,
            double lastMaximum,
            double endPoint) : 
            base(name, startPoint,
                firstMaximum,
                lastMaximum,
                endPoint) {}

        

        public override object Clone()
        {
            FuzzyColor color = new FuzzyColor(Name, StartPoint, FirstMaximum, LastMaximum, EndPoint);
            color.MutationStep = this.MutationStep;
            return color;
        }

    }
}
