﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FuzzySpeech.Audio.Collections;

namespace FuzzySpeech.Audio
{
    class AudioSample
    {
        private AudioFrameCollection frames;

        public AudioFrameCollection Frames
        {
            get { return frames; }
            set { frames = value; }
        }
        int bandsByFrame;

        public int BandsByFrame
        {
            get { return bandsByFrame; }
        }

        public AudioSample(int bandsByFrame)
        {
            this.bandsByFrame = bandsByFrame;
            frames = new AudioFrameCollection(this);
        }




    }
}