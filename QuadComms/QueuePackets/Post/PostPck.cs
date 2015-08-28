using QuadComms.Interfaces.Queues;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.QueuePackets.Post
{
    public class PostPck : IQuadTransQueueMsg
    {
        public PostPck(byte[] data, int quadId, bool ackRequired = false)
        {
            this.Data = data;
            this.Ackrequired = ackRequired;
            this.QuadId = quadId;
        }

        public bool Ackrequired { get; private set; }

        public byte[] Data {get; private set;}


        public int QuadId
        {
            get;
            private set;
        }
    }
}
