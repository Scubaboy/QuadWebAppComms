using QuadSignalRMsgs.HubResponces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Queues
{
    public interface ISignalRRecvQueueMsg
    {
        Responce ResponceForQuad { get; }
    }
}
