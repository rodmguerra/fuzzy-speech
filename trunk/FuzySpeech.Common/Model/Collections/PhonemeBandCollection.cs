using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace FuzzySpeech.Model.Collections
{
    public class PhonemeBandCollection : CollectionBase
    {
        private Phoneme owner;

        
        public PhonemeBandCollection(Phoneme owner)
        {           
            if (owner == null)
                throw new Exception("Owner can't be null.");
            
            this.owner = owner;
            
            if (this.Owner.NumberOfBands < 1)
                throw new Exception("The PhonemeBandCollection must have at least one band.");

            InnerList.Capacity = Owner.NumberOfBands;
            
        }

        public Phoneme Owner
        {
            get { return owner; }
        }

        public void Add(PhonemeBand phonemeBand)
        {
            if (InnerList.Count >= Owner.NumberOfBands)
                throw new Exception("The list of PhonemeBands is full.");

            if (phonemeBand.NumberOfColors != Owner.ColorsByBand)
                throw new Exception("The number of colors of the phoneme band must be compatible to the phoneme's color by band property.");

            phonemeBand.OwnerPhoneme = this.Owner;
            InnerList.Add(phonemeBand);
        }

        public PhonemeBand this[int index]
        {
            get {
                if (index < 0 || index >= InnerList.Count)
                    throw new IndexOutOfRangeException();

                return (PhonemeBand)InnerList[index]; 
            }
            set {
                if (index < 0 || index >= InnerList.Count)
                    throw new IndexOutOfRangeException();

                if (value == null)
                    throw new NullReferenceException("The phoneme band can't be null.");

                if (value.NumberOfColors != owner.ColorsByBand)
                    throw new Exception("The number of colors of the phoneme band must be compatible to the phoneme's color by band property.");

                value.OwnerPhoneme = this.Owner;
                InnerList[index] = value; 
            }
        }

    }
}
