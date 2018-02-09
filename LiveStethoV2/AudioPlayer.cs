﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using NAudio.Wave;
using System.Threading;

namespace LiveStethoV2
{
    class AudioPlayer
    {
        private readonly BackgroundWorker AudioPlayerBG = new BackgroundWorker(); //New Bg Worker
        private IWavePlayer player;        //Wave Out Player
        private BufferedWaveProvider audioprovider;  //Ram Stream
        private ManualResetEvent AudioSync = new ManualResetEvent(false);  //Sync Construct
        private int _rate; 

        public AudioPlayer(int rate, int bits, int channels)
        {
            this.AudioPlayerBG.WorkerReportsProgress = true;
            this.AudioPlayerBG.WorkerSupportsCancellation = true;
            this.AudioPlayerBG.DoWork += new DoWorkEventHandler(this.AudioPlayerWork);
            this.AudioPlayerBG.ProgressChanged += new ProgressChangedEventHandler(this.AudioPlayerProgressChanged);
            this.AudioPlayerBG.RunWorkerCompleted += new RunWorkerCompletedEventHandler(this.AudioPlayerCompleted);

            this._rate = rate; 
            this.InitializePlayer(rate, bits, channels);
        }

        private IWavePlayer AudioOut
        {
            get {
                return this.player;
            }
            set { player = value; }
        }
        private BufferedWaveProvider AudioProvider
        {
            get
            {
                return this.audioprovider;
            }
            set { audioprovider = value;
				//audioprovider.BufferDuration = TimeSpan.FromMinutes(1);
			}
        }

        private void AudioSyncSet()
        {
           this.AudioSync.Set();
        }

        public void AddData(byte[] data)
        {
            this.audioprovider.AddSamples(data, 0, data.Length);
        }
        private void InitializePlayer(int rate, int bits, int channels)
        {
            this.AudioOut = new WaveOut();
            this.AudioProvider = new BufferedWaveProvider(new WaveFormat(rate, bits, channels));
            this.AudioOut.Init(this.AudioProvider);  //Initialize directsoundout player
            this.AudioOut.PlaybackStopped += new EventHandler<NAudio.Wave.StoppedEventArgs>(this.SoundPlayed); //Register Playback Stopped Handler
        }

        public void Play()
        {
            if (!this.AudioPlayerBG.IsBusy)
            {
                this.AudioPlayerBG.RunWorkerAsync();
            }
            else
            {
                this.AudioSyncSet();  //Indicate to Continue Play
            }
        }
        private void SoundPlayed(object sender, StoppedEventArgs data)
        {
            Console.WriteLine("NAudio Finished Sound Playing");
        }

        private void AudioPlayerWork(object sender, DoWorkEventArgs e)
        {
            while (true) //TODO, change to more legitimate playing
            {
                //Waite For Manual Reset Event
                player.Play();
                this.AudioSync.Reset();
                //this.AudioSync.WaitOne(); //Wait For Set Event
            }
        }

        private void AudioPlayerProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException(); 
        }

        private void AudioPlayerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Console.WriteLine("BG Thread Completed");
        }
    }
}