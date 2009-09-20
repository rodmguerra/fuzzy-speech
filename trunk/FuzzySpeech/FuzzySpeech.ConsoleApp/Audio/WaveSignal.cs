using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Extractor;

namespace FuzzySpeech.Audio
{
    class WaveSignal
    {
        WaveFormat format;

        public WaveFormat Format
        {
          get { return format; }
          set { format = value; }
        }
        
        int length;

        public int Length
        {
          get { return length; }
          set { length = value; }
        }
    }
}
