using System;
using System.Collections;
using System.Linq;
using System.Text;
using FuzzySpeech.Common.Audio;

namespace FuzzySpeech.Common.Audio.Collections
{
    public class AudioFrameCollection : CollectionBase
    {
        private AudioSample ownerSample;

        public AudioFrameCollection(AudioSample owner)
        {           
            if (owner == null)
                throw new Exception("Owner can't be null.");
            
            this.ownerSample = owner;            
        }

        public AudioSample OwnerSample
        {
            get { return ownerSample; }
        }

        public void Add(AudioFrame audioFrame)
        {
            if (audioFrame.NumberOfBands != OwnerSample.BandsByFrame)
                throw new Exception("The NumberOfBands must be compatible to the sample BandsByFrame property.");

            InnerList.Add(audioFrame);
        }

        public AudioFrame this[int index]
        {
            get {
                if (index < 0 || index >= InnerList.Count)
                    throw new IndexOutOfRangeException();

                return (AudioFrame)InnerList[index]; 
            }
            set {
                if (index < 0 || index >= InnerList.Count)
                    throw new IndexOutOfRangeException();

                if (value == null)
                    throw new NullReferenceException("The AudioFrame can't be null.");

                if (value.NumberOfBands != ownerSample.BandsByFrame)
                    throw new Exception("The NumberOfBands must be compatible to the sample BandsByFrame property.");

                InnerList[index] = value; 
            }
        }
    }
}
