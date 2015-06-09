using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Queues
{
    internal interface IPostQueueMsg
    {
        bool Ackrequired { get; }
        byte[] Data { get; }
    }
}
