using QuadComms.DataPcks;
using QuadComms.Interfaces.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.QueuePackets.QuadRecv
{
    public class QuadRecvPck : IQuadRecvMsgQueue
    {
        public QuadRecvPck(DataPck data, UInt32 crc)
        {
            this.Msg = data;
            this.CRC = crc;
        }

        public DataPck Msg { get; private set; }


        public uint CRC { get; private set; }
    }
}
