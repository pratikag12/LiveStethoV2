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
using NAudio;
using MathNet.Filtering;
using RestSharp;
using System.Net;

namespace LiveStethoV2
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Graphing---------------------------------------
        private IXyDataSeries<double, short> SoundData;
        private int plotCount = 0;
        private int Capacity = 10;
        List<byte> SoundBuffer = new List<byte>();

        //Binary Reader (TODO: temp)-----------------------------------------
        private const int _chunkSize = 16000;
        public BinaryReader reader = null;
        string inputfile = Path.Combine(Directory.GetCurrentDirectory(), @"TestAudio.dat");
        string outputfile = Path.Combine(Directory.GetCurrentDirectory(), @"StethoSound.dat");
        //Audio Player Classes--------------------------------------------
        AudioPlayer StethoPlayer;

        //Wave Writer Class------------------------------------------------
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

            //Graphing Add In Code-Behind
            SoundData = new XyDataSeries<double, short>();
            SoundData.FifoCapacity = this.Capacity * 16000;

            //View Model Update
            Sthetho = new StethoViewModel(Init, Stop, Clear, Retrieve);
            this.DataContext = Sthetho;
            Sthetho.IsStreaming = false;
            Sthetho.FilterHeart = false;

            //Open File (Remove for serial Stream)
            reader = OpenFile(inputfile);

            //Audio Class
            StethoPlayer = new AudioPlayer(15277, 16, 1);

            //Open Serial Port
            SerialDataIn = new SerialCom(921600, "COM8");

            //Open File For Writing
            Sthetho.OutFileName = "StethoSound"; //Deprecated
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
            //SerialDataIn.OpenSerialPort();  //Open Serial Data Por

            //Setup Sound plot
            SoundSeries.DataSeries = SoundData;
            YAxis.VisibleRange = new DoubleRange(-32000, 32000);
            this.sciChartSurface.XAxis.AutoRange = SciChart.Charting.Visuals.Axes.AutoRange.Always;
            SoundData.Clear(); 

            //Check if File will be written
            if (Sthetho.WriteToFile)
            {
               
                if(File.Exists(outputfile))
                {
                    File.Delete(outputfile);
                }
                tmpFile = File.OpenWrite(outputfile);
            }

            timer = new MultimediaTimer() { Interval = 1 };

            Console.WriteLine("Running graphing system");
            //Observable to output to observer
            var dataStream = Observable.Create<byte[]>(ob =>
            {
                timer.Elapsed += (source, e) =>
                {
                    if (reader.BaseStream.Position < reader.BaseStream.Length)
                    {
                        ob.OnNext(reader.ReadBytes(32));  //Call connected, subscribed observables. 
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
            })
            .SelectMany(data => data.ToObservable())
            .Publish();

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
            {
                dataStream.Buffer(2).Subscribe(data =>
                {
                    tmpFile.WriteByte(data[0]);
                    tmpFile.WriteByte(data[1]);
                    tmpFile.Flush();
                });
            }

            //Graphing
            dataStream
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
                                SoundData.Append(plotCount++, s);
                            }
                        }
                    }
                });

            dataStream.Connect();  //For the .Publish on the observable
        }

        private void Stop()
        {
            if (timer.IsRunning)
                timer.Stop();
            reader.BaseStream.Seek(0, SeekOrigin.Begin);  //Reset binary stream
            timer.Dispose();

            this.plotCount = 0; //Reset Plot count
            //Remove timer Reference
            tmpFile.Close();  //Save File
            SaveResultToServer(); //Save Result to Server
            Sthetho.IsStreaming = false;  //Tell UI To Stop Streaming
        }

        //Server Comm
        public async void SaveResultToServer()
        {
            long fileLength = new FileInfo(outputfile).Length; //Get file length
            FlaskCommunication flaskcom = new FlaskCommunication(); 
            //Send File to server
            Console.WriteLine("Adding MetaData to Server");
            Tuple<HttpStatusCode, SoundDataModel.SoundData> data = await flaskcom.PostMetaData("HeartData", fileLength);
            if(data.Item1 == HttpStatusCode.OK)
            {
                HttpStatusCode fileupload = await flaskcom.PostFile(outputfile, fileLength, 
                    data.Item2.Id);
                if(fileupload != HttpStatusCode.OK)
                {
                    MessageBox.Show("Failed to save file to server");
                }
            }     
        }

        private void Clear()
        {
            SoundData.Clear();
        }

        private void Retrieve()
        {
            GetDataFromServer();
        }

        //Server Comm
        public async void GetDataFromServer()
        {            
            //Get All MetaData From server
            FlaskCommunication flaskcom = new FlaskCommunication();
            Tuple<HttpStatusCode, SoundDataModel> data = await flaskcom.GetAllMetaData();

            List<SoundDataModel.SoundData> SoundList = null;
            if (data.Item1 == HttpStatusCode.OK)
            {
                SoundList = data.Item2.SoundDatas; //Get List of SoundData
            }

            //Test Populate List View
            PopupListView view = new PopupListView(SoundList); //Generate Form with list data
            view.ShowDialog();
            //Get Selected MetaData Object
            Tuple<HttpStatusCode, byte []> seldata = await flaskcom.GetSoundFile(view.SelectedRecord);
            
            if(seldata.Item1 == HttpStatusCode.OK)
            {
                SoundData.Clear();
                this.PlotServerData(seldata.Item2);
                //Analyze if Needed

            }

        }

        public void PlotServerData(byte [] sounddata)
        {

            var shorts = new List<short>(); //To Store data list
            var revdataSeries = new XyDataSeries<int, short>(); //Data Series
            SoundSeries.DataSeries = revdataSeries;     //Attach series to Datasiers

            //Convert Byte Array to short
            for (int n = 0; n < sounddata.Length; n += 2)
            {
                short sample = BitConverter.ToInt16(sounddata, n);
                shorts.Add(sample);
            }

            List<int> xval = Enumerable.Range(plotCount, shorts.Count).ToList<int>();
               
            //Change View
            YAxis.VisibleRange = new DoubleRange(shorts.Min() - 7500, shorts.Max()+ 7500);
            XAxis.VisibleRange = new DoubleRange(xval.Min(), xval.Max());
            this.sciChartSurface.XAxis.AutoRange = SciChart.Charting.Visuals.Axes.AutoRange.Never;

            //Plot Data
            revdataSeries.Append(xval, shorts);
        }

        //Temporay Open File--------------------------------------------------->
        public BinaryReader OpenFile(string inputfile)
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
