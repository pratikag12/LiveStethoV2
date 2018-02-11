using System;
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
		private int Capacity = 8;

		//Binary Reader (TODO: temp)-----------------------------------------
		private const int _chunkSize = 16000;
        public BinaryReader reader = null;
        public string inputfile = @"./Pratik_Heart_6_OP_Clean.dat";

		//Audio Player Classes--------------------------------------------
		AudioPlayer StethoPlayer;

		//Wave Writer Class------------------------------------------------
		WaveWriter StethoOutFile;


		//Serial Data In---------------------------------------------------
		SerialCom SerialDataIn;
		private ManualResetEvent SerialWait = new ManualResetEvent(false);

		//View Model
		StethoViewModel Sthetho;

        //Hi Resolution Timer
        MultimediaTimer timer; 

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

			//SerialDataIn = new SerialCom(SerialWait, 115200);
			StethoPlayer = new AudioPlayer(16000, 16, 1);  //Audio Player Class
														   //StethoOutFile = new WaveWriter(_outputwave, 16000, 16, 1); //Wave Writer Class
			//Graphing Add In Code-Behind
			SoundData = new XyDataSeries<int, short>();
			SoundData.FifoCapacity = this.Capacity * 16000;
			SoundSeries.DataSeries = SoundData;
			Yaxis.VisibleRange = new DoubleRange(-32000, 32000);

            //View Model Update
			Sthetho = new StethoViewModel(Init, Stop, Clear);
			this.DataContext = Sthetho;
			Sthetho.IsStreaming = false;

            //Open File (Remove for serial Stream)
            reader = OpenFile();

            //Audio Class
            StethoPlayer = new AudioPlayer(16000, 16, 1);

            //Open Serial Port
            SerialDataIn = new SerialCom(115200);
            //SerialDataIn.OpenSerialPort();

            //Open File For Writing
            Sthetho.OutFileName = "StethoSound";
            Sthetho.WriteToFile = false; 
           
        }

		private void Init()
		{
			Sthetho.IsStreaming = true;  //Block Streaming Button
            checkboxFile.IsEnabled = false;
            if (Sthetho.WriteToFile)
                StethoOutFile = new WaveWriter(Sthetho.OutFileName, 16000, 16, 1);
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
            var dataStream = Observable.Create<byte[]>(ob =>
            {
                SerialDataIn.SerialPortFTDI.DataReceived += (obj, e) =>
                {
                    byte[] Sound = new byte[SerialDataIn.SerialPortFTDI.BytesToRead];
                    SerialDataIn.SerialPortFTDI.Read(Sound, 0,bytes); //Could read less than bytes no of bytes. 
                    ob.OnNext(Sound);
               
                };
                return Disposable.Empty;
            }).Publish();
            */
            //Audio Playback
            dataStream
                .Buffer(1000)  //Wait every 1000 collected items to fire
				.SubscribeOn(NewThreadScheduler.Default)
				.Subscribe(values =>
				{
					var res = values.SelectMany(i => i).ToArray();
					StethoPlayer.AddData(res);
                    if(Sthetho.WriteToFile)
                        StethoOutFile.WriteData(res);  //Write Data to File                    
                    StethoPlayer.Play();
				});

            //Graphing
			dataStream
				.Buffer(10)  //Wait every 10ms to fire subscriber on a new thread. 
				.SubscribeOn(NewThreadScheduler.Default)
				.Subscribe(values =>
				{
					Console.WriteLine("draw: begin");
					// draw
					var res = values.SelectMany(i => i).ToArray();
					short[] s = new short[res.Length / 2];

					Buffer.BlockCopy(res.ToArray(), 0, s, 0, res.Length);
					using (sciChartSurface.SuspendUpdates())
					{
						foreach (short i in s)
						{
							SoundData.Append(plotCount++, i);
						}
					}
					Console.WriteLine("draw: end");
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

			dataStream.Connect();
		}

        private void Stop()
        {
            if (timer.IsRunning)
                timer.Stop();
            if (StethoOutFile != null)
                StethoOutFile.CloseFile();
            Sthetho.IsStreaming = false;  //Tell UI To Stop Streaming
            reader.BaseStream.Seek(0, SeekOrigin.Begin);  //Reset binary stream
            timer.Dispose();
            checkboxFile.IsEnabled = true;
            //Remove timer Reference
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
