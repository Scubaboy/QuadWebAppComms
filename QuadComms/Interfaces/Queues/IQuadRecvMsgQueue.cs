using QuadComms.DataPcks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Queues
{
    public interface IQuadRecvMsgQueue
    {
        DataPck Msg { get;  }
        UInt32 CRC { get; }
    }
}
