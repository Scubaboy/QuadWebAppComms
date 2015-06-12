using QuadComms.Interfaces.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.QueuePackets.Post
{
    public class PostPck : IPostQueueMsg
    {
        public PostPck(byte[] data, bool ackRequired = false)
        {
            this.Data = data;
            this.Ackrequired = ackRequired;
        }

        public bool Ackrequired { get; private set; }

        public byte[] Data {get; private set;}
    }
}
