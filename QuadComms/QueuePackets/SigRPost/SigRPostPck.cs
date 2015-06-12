using QuadComms.Interfaces.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.QueuePackets.SigRPost
{
    public class SigRPostPck<T> : ISigRPostQueueMsg<T>
    {
        public SigRPostPck(T item)
        {
            this.Msg = item;
        }

        public T Msg { get; private set; }
}
}
