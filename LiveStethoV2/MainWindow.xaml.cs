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
		private int Capacity = 4;

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
			StethoPlayer = new AudioPlayer(15277, 16, 1);  //Audio Player Class
														   //StethoOutFile = new WaveWriter(_outputwave, 16000, 16, 1); //Wave Writer Class

			//Remove this with serial stream added
			//OpenFile();

			//Graphing Add In Code-Behind
			SoundData = new XyDataSeries<long, short>();
			SoundData.FifoCapacity = this.Capacity * 16000;
			SoundSeries.DataSeries = SoundData;
			Yaxis.VisibleRange = new DoubleRange(-32000, 32000);

			Init();
		}

		private void Init() {
			var reader = new BinaryReader(File.Open(inputfile, FileMode.Open));

			var clickStream = Observable.FromEventPattern<RoutedEventHandler, RoutedEventArgs>(
				x => btnStream.Click += x,
				x => btnStream.Click -= x);

			var dataStream = clickStream.SelectMany(e => Observable.Interval(TimeSpan.FromSeconds(1)))
				.Select(count => (reader.ReadBytes(_chunkSize * sizeof(short)), count));

			dataStream.Subscribe(values => {
				// play
				StethoPlayer.AddData(values.Item1);
				StethoPlayer.Play();
			});
			dataStream.Subscribe(values => {
				// draw
				short[] s = new short[values.Item1.Length / 2];
				Buffer.BlockCopy(values.Item1, 0, s, 0, values.Item1.Length);
				int shift = 0;
				using (sciChartSurface.SuspendUpdates())
				{
					foreach (short i in s)
					{
						SoundData.Append(_chunkSize * values.Item2 + shift++, i);
					}
				}
			});
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
