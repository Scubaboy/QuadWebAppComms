using QuadComms.CommControllers;
using QuadComms.Interfaces.CommsDevice;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.CommsDevices.SerialComms
{
    internal class Serial : ICommsDevice
    {
        private SerialPort serialPort = null;

        public Serial(CommPortConfig commPortConfig)
        {
            this.serialPort = new SerialPort(commPortConfig.PortName, commPortConfig.Baud, commPortConfig.Parity, commPortConfig.DataBits, commPortConfig.Stopbits);
            this.serialPort.Close();
            this.serialPort.Handshake = commPortConfig.Handshake;
            this.serialPort.ReadBufferSize = 200;
            this.serialPort.WriteBufferSize = 1024;
            this.serialPort.Open();
            this.serialPort.DiscardInBuffer();
            this.serialPort.DiscardOutBuffer();
        }

        public void Write(byte[] buffer, int offset, int count)
        {
            this.serialPort.Write(buffer, offset, count);
        }

        public void ClearOutput()
        {
            this.serialPort.DiscardOutBuffer();
        }

        public void ClearInput()
        {
            this.serialPort.DiscardInBuffer();
        }

        public void Read(byte[] buffer, int offset, int count)
        {
            this.serialPort.Read(buffer, offset, count);
        }

        public int BytesToRead
        {
            get 
            {
                return this.serialPort.BytesToRead; 
            }
        }


        public void Close()
        {
            this.serialPort.Close();
        }


        public int ReadByte()
        {
            return this.serialPort.ReadByte();
        }
    }
}
