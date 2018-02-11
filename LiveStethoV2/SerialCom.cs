using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LiveStethoV2
{

    class SerialCom
    {
        private SerialPort SerialFTDI;


        public SerialCom(int BaudRate)
        {
            this.SerialFTDI = new SerialPort();
            this.SerialFTDI.BaudRate = BaudRate;                       //Serial Port Speed
            this.SerialFTDI.ReadBufferSize = 2048;
            this.SerialFTDI.DataBits = 8;
            this.SerialFTDI.StopBits = StopBits.One;
            this.SerialFTDI.Parity = Parity.None;
        }

        public SerialPort SerialPortFTDI
        {
            get
            {
                return SerialFTDI;
            }
        }

        public void OpenSerialPort()
        {
            try
            {
                this.SerialFTDI.Open();
            }
            catch (IOException)
            {
                throw; //Output to Debug Port
            }
            catch (InvalidOperationException err)
            {
                throw;
            }
            catch (UnauthorizedAccessException err)
            {
                throw;
            }
        }

        public void CloseSerialPort()
        {
            if (this.SerialFTDI.IsOpen)
            {
                this.SerialFTDI.Close();                                                       //Release Port Object 
            }
        }


        public bool WriteByteToSerialPort(string data)                   //Write Byte to UART Serial Port
        {
            bool result;
            if (this.SerialFTDI.IsOpen)                                     //check Serial_Port_Programmer open
            {
                try
                {
                    this.SerialFTDI.Write(data);                    //Write Data To Serial Port
                }
                catch (Exception err)
                {
                    //Display_Area.AppendText("WriteByteToSerialPort error. " + err.Message + "\r\n");   //If errors
                    result = false;
                    return result;
                }
            }

            result = true;
            return result;
        }

        public void SerialFTDIRxHandler(object sender, SerialDataReceivedEventArgs e)
        {

        }

    }

}
