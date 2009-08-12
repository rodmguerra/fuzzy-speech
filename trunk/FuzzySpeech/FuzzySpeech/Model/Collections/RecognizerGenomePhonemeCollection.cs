using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace FuzzySpeech.Model.Collections
{
    class RecognizerGenomePhonemeCollection : CollectionBase
    {
        private RecognizerGenome owner;

        public RecognizerGenomePhonemeCollection(RecognizerGenome owner)
        {           
            if (owner == null)
                throw new NullReferenceException("Owner can't be null.");
            
            this.owner = owner;            
        }

        public RecognizerGenome Owner
        {
            get { return owner; }
        }

        public void Add(Phoneme phoneme)
        {
            phoneme.OwnerGenome = this.Owner;

            InnerList.Add(phoneme);
        }

        public Phoneme this[int index]
        {
            get {
                if (index < 0 || index >= InnerList.Count)
                    throw new IndexOutOfRangeException();

                return (Phoneme)InnerList[index]; 
            }
            set {
                if (index < 0 || index >= InnerList.Count)
                    throw new IndexOutOfRangeException();

                if (value == null)
                    throw new NullReferenceException("The phoneme can't be null.");

                value.OwnerGenome = this.Owner;

                InnerList[index] = value; 
            }
        }

    }

}
