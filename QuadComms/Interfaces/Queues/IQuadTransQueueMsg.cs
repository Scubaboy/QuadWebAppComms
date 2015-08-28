using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Queues
{
    public interface IQuadTransQueueMsg
    {
        bool Ackrequired { get; }
        byte[] Data { get; }

        int QuadId { get; }
    }
}
