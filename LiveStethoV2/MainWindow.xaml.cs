﻿using System;
using System.ComponentModel;
using System.IO.Ports;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using SciChart.Charting.Model.DataSeries;
using SciChart.Data.Model;
using System.Reactive.Linq;
using System.Reactive;
using NAudio.Wave;
using System.Reactive.Concurrency;
using System.Linq;
using System.Reactive.Disposables;
using NAudio;
using MathNet.Filtering;


namespace LiveStethoV2
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		//Graphing---------------------------------------
		private IXyDataSeries<int, short> SoundData;
        private int plotCount = 0;
		private int Capacity = 10;
        List<byte> SoundBuffer = new List<byte>();
   
		//Binary Reader (TODO: temp)-----------------------------------------
		private const int _chunkSize = 16000;
        public BinaryReader reader = null;
        string inputfile = "Somethingelse";
        //Audio Player Classes--------------------------------------------
        AudioPlayer StethoPlayer;

        //Wave Writer Class------------------------------------------------
        //private WaveWriter StethoOutFile;
        //byte[] Sound = new byte[512];
        //int counter = 0;
        Stream tmpFile;

        //Serial Data In---------------------------------------------------
        SerialCom SerialDataIn;
		private ManualResetEvent SerialWait = new ManualResetEvent(false);

		//View Model
		StethoViewModel Sthetho;

        //Hi Resolution Timer, For File Reading
        MultimediaTimer timer;

        //Filter Object
        double fc1 = 50; //low cutoff frequency
        double fc2 = 250; //high cutoff frequency
        double samplerate = 15277;
        OnlineFilter BandPassfilter;
        OnlineFilter Denoiser;

        public MainWindow()
		{
			try
			{
				InitializeComponent();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}

			//StethoOutFile = new WaveWriter(_outputwave, 16000, 16, 1); //Wave Writer Class
			//Graphing Add In Code-Behind
			SoundData = new XyDataSeries<int, short>();
			SoundData.FifoCapacity = this.Capacity * 16000;
			SoundSeries.DataSeries = SoundData;
			YAxis.VisibleRange = new DoubleRange(-32000, 32000);

            //View Model Update
            Sthetho = new StethoViewModel(Init, Stop, Clear);
            this.DataContext = Sthetho;
			Sthetho.IsStreaming = false;

            //Open File (Remove for serial Stream)
            reader = OpenFile();

            //Audio Class
            StethoPlayer = new AudioPlayer(15277, 16, 1);

            //Open Serial Port
            SerialDataIn = new SerialCom(921600, "COM8");
            //SerialDataIn.OpenSerialPort();

            //Open File For Writing
            Sthetho.OutFileName = "StethoSound";
            Sthetho.WriteToFile = true;

            VertAnnotate.CoordinateMode = SciChart.Charting.Visuals.Annotations.AnnotationCoordinateMode.Absolute;
            VertAnnotate.Y1 = -32000;
            VertAnnotate.VerticalAlignment = VerticalAlignment.Top;
            Sthetho.AnnotationX = 0;

            //Create filter objects
            BandPassfilter = OnlineFilter.CreateHighpass(ImpulseResponse.Finite, samplerate, fc1);
            Denoiser = OnlineFilter.CreateDenoise(25);
        }

		private void Init()  
		{
			Sthetho.IsStreaming = true;  //Block Streaming Button
            checkboxFile.IsEnabled = false; //Disable CheckBox
            SerialDataIn.OpenSerialPort();  //Open Serial Data Port
            BinaryReader br = new BinaryReader(SerialDataIn.SerialPortFTDI.BaseStream);
     
            if (Sthetho.WriteToFile)
            {
               //StethoOutFile = new WaveWriter(Sthetho.OutFileName, 15277, 16, 1);
                 tmpFile = File.OpenWrite(@"D:\Test Data S\CDLData\StethoStream.bin"); }
           
            timer = new MultimediaTimer() { Interval = 1 };
            
            Console.WriteLine("Running graphing system");
            //Observable to output to observer
            var dataStream = Observable.Create<byte[]>(ob =>
            {
                timer.Elapsed += (source, e) =>
            	{
            		if (reader.BaseStream.Position < reader.BaseStream.Length)
           		{
           			//Console.WriteLine("timer: " + reader.BaseStream.Position.ToString() + " - " + reader.BaseStream.Length.ToString());
           			ob.OnNext(reader.ReadBytes(32));  //Call connected, subscribed observables. 
           			//Console.WriteLine("timer: readed");
           		}
           		else
                    {
                        //Console.WriteLine("timer: ready to stop");
                        timer.Stop();
                        if (StethoOutFile != null)
                            StethoOutFile.CloseFile();
                        Sthetho.IsStreaming = false;
                        checkboxFile.IsEnabled = true;
                        //Console.WriteLine("timer: stopped");
                    }
                };
                timer.Start();
                return Disposable.Empty;
            }).Publish();

            /*
            //Observable to output to observer - SerialPort
            var dataStream = Observable.FromEventPattern<SerialDataReceivedEventHandler, SerialDataReceivedEventArgs>(
                h => SerialDataIn.SerialPortFTDI.DataReceived += h,
                h => SerialDataIn.SerialPortFTDI.DataReceived -= h)
                .Select(arg =>
                {
                    byte[] buf = new byte[SerialDataIn.SerialPortFTDI.BytesToRead];
                    SerialDataIn.SerialPortFTDI.Read(buf, 0, buf.Length);
                    return buf;
                })
            .SelectMany(data => data.ToObservable())
            .Publish();
            */

            // Save to file
            if (Sthetho.WriteToFile)
            { dataStream.Subscribe(data => {
                tmpFile.WriteByte(data);
                tmpFile.Flush();
                }); }
  
            ////Audio Playback----------------
            /*dataStream
                .Buffer(512)
               .Subscribe(values =>
                    { StethoPlayer.AddData(values.ToArray());
                    StethoPlayer.Play();
                });*/
            //------------------------------------
            
            //Graphing
            dataStream
                //.SubscribeOn(NewThreadScheduler.Default)
                .Buffer(2)
                .Buffer(153)
                .Subscribe(values =>
                {
                    // draw
                    using (sciChartSurface.SuspendUpdates())
                    {
                        foreach (var v in values)
                        {
                            short s = BitConverter.ToInt16(new byte[2] { (byte)v[1], (byte)v[0] }, 0);
                            //if (StethoPlayer.PlayBackState == PlaybackState.Playing)
                            //Sthetho.AnnotationX = plotCount - (16000);
                            if (Sthetho.FilterHeart)
                            {
                                SoundData.Append(plotCount++, (short)Denoiser.ProcessSample(BandPassfilter.ProcessSample((double)s)));
                                //SoundData.Append(plotCount++, (short)(double)s));
                            }
                            else
                            {
                                SoundData.Append(plotCount++,  s);
                            }
                        }
                    }
                });
            

            //File Writing      
            /*
            dataStream.Buffer(2000).SubscribeOn(NewThreadScheduler.Default)
                .Subscribe(values =>
                {
                    var res = values.SelectMany(i => i).ToArray();
                    StethoOutFile.WriteData(res);
                }
                );
              */  

            dataStream.Connect();  //For the .Publish on the observable
		}

        private void Stop()
        {
            //if (timer.IsRunning)
            //timer.Stop();
            //if (StethoOutFile != null)
            //StethoOutFile.CloseFile();
            //Sthetho.IsStreaming = false;  //Tell UI To Stop Streaming
            //reader.BaseStream.Seek(0, SeekOrigin.Begin);  //Reset binary stream
            //timer.Dispose();
            //checkboxFile.IsEnabled = true;
            //Remove timer Reference
            //tmpFile.Close();
        }

        private void Clear()
        {
            SoundData.Clear();
        }

		//Temporay Open File--------------------------------------------------->
		public BinaryReader OpenFile()
		{
			if (!File.Exists(inputfile))
			{
				throw new Exception();
			}
			return new BinaryReader(File.Open(inputfile, FileMode.Open));
		}
		//--------------------------------------------------------------------->

		//Graphing Library
		#region INotifyPropertyChanged implementation

		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName = null)
		{
			if (PropertyChanged != null)
				PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		#endregion
	}

}
