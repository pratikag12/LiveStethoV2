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
		//Graphing(TODO)---------------------------------------
		private IXyDataSeries<long, short> SoundData;
		private int GlobalBlock = 0;
		private int Capacity = 8;

		//Binary Reader (TODO: temp)-----------------------------------------
		private const int _chunkSize = 16000;
		public byte[] Sound = new byte[16000 * 2];  //1 Sec of Buffered Sound Data
		public BinaryReader sr;
		public string inputfile = @"./TestAudio.dat";

		//Audio Player Classes--------------------------------------------
		AudioPlayer StethoPlayer;

		//Wave Writer Class------------------------------------------------
		WaveWriter StethoOutFile;
		const string _outputwave = @"D:\Stethoscope Test Data\OutputAudio";

		//Serial Data In---------------------------------------------------
		SerialCom SerialDataIn;
		private ManualResetEvent SerialWait = new ManualResetEvent(false);

		//View Model
		StethoViewModel Sthetho;

		private int plotCount = 0;
		FileStream fs = null;
		BinaryReader reader = null;
		MultimediaTimer timer = new MultimediaTimer() { Interval = 1 };

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

			//Remove this with serial stream added
			//OpenFile();

			//Graphing Add In Code-Behind
			SoundData = new XyDataSeries<long, short>();
			SoundData.FifoCapacity = this.Capacity * 16000;
			SoundSeries.DataSeries = SoundData;
			Yaxis.VisibleRange = new DoubleRange(-32000, 32000);

			Sthetho = new StethoViewModel(() => Init());
			this.DataContext = Sthetho;
			Sthetho.IsStreaming = false;

			fs = File.Open(inputfile, FileMode.Open);
			reader = new BinaryReader(fs);
		}

		private void Init()
		{
			//Sthetho.IsStreaming = true;

			var waveOut = new WaveOut();
			var provider = new BufferedWaveProvider(new WaveFormat(16000, 16, 1));
			waveOut.Init(provider);
			Console.WriteLine("Running graphing system");

			//var dataStream = Observable.Interval(TimeSpan.FromMilliseconds(100))

			var dataStream = Observable.Create<byte[]>(ob =>
			{
				timer.Elapsed += (source, e) =>
				{
					if (reader.BaseStream.Position < reader.BaseStream.Length)
					{
						Console.WriteLine("timer: " + reader.BaseStream.Position.ToString() + " - " + reader.BaseStream.Length.ToString());
						ob.OnNext(reader.ReadBytes(32));
						Console.WriteLine("timer: readed");
					}
					else
					{
						Console.WriteLine("timer: ready to stop");
						timer.Stop();
						Console.WriteLine("timer: stopped");
					}
				};
				timer.Start();
				return Disposable.Empty;
			}).Publish();

			dataStream
				.Buffer(1000)
				.SubscribeOn(NewThreadScheduler.Default)
				.Subscribe(values =>
				{
					// play
					//Console.WriteLine("Playing Started");
					//Sthetho.IsStreaming = true;
					//List<byte[]> temp1 = values.Select(t => t.Item1).ToList();
					//byte[] temp2 = temp1.SelectMany(a => a).ToArray();
					//Console.WriteLine(temp2.Count());
					//try
					//{
					var res = values.SelectMany(i => i).ToArray();
					//StethoPlayer.AddData(res);
					//StethoPlayer.Play();

					provider.AddSamples(res, 0, res.Length);
					waveOut.Play();

					//}
					//catch (Exception ex)
					//{
					//	Console.WriteLine("Audio Playing Exception Occured");
					//	Console.WriteLine(ex);
					//}

					//Console.WriteLine("Playing observer on next exit");
				});

			dataStream
				.Buffer(10)
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

			dataStream.Connect();
		}

		//Temporay Open File--------------------------------------------------->
		public void OpenFile()
		{
			if (!File.Exists(inputfile))
			{
				throw new Exception();
			}
			sr = new BinaryReader(File.Open(inputfile, FileMode.Open));
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
