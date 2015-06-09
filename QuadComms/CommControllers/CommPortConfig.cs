using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.CommControllers
{
    struct CommPortConfig
    {
        private string portname;
        private int baud;
        private Parity parity;
        private StopBits stopBits;
        private Handshake handshake;
        private int dataBit;

        public CommPortConfig(string portname, int baud, Parity parity, StopBits stopBits, Handshake handshake, int dataBit)
        {
            this.portname = portname;
            this.baud = baud;
            this.parity = parity;
            this.stopBits = stopBits;
            this.handshake = handshake;
            this.dataBit = dataBit;
        }

        public string PortName
        {
            get
            {
                return this.portname;
            }
        }

        public int Baud
        {
            get
            {
                return this.baud;

            }
        }

        public Parity Parity
        {
            get
            {
                return this.parity;
            }
        }

        public StopBits Stopbits
        {
            get
            {
                return this.stopBits;
            }
        }

        public Handshake Handshake
        {
            get
            {
                return this.handshake;
            }
        }

        public int DataBits
        {
            get
            {
                return this.dataBit;
            }
        }

    
    }
}
