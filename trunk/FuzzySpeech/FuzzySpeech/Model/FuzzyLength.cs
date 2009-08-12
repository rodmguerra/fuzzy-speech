using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FuzzySpeech.Model
{
    class FuzzyLength : FuzzySet
    {
        public FuzzyLength(string name,
            double startPoint,
            double firstMaximum,
            double lastMaximum,
            double endPoint) : 
            base(name, startPoint,
                firstMaximum,
                lastMaximum,
                endPoint) {}


        public override object Clone()
        {
            return new FuzzyLength(Name, StartPoint, FirstMaximum, LastMaximum, EndPoint);
        }



    }


}
