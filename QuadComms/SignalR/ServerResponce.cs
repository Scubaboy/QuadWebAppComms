using QuadComms.Interfaces.Queues;
using QuadSignalRMsgs.HubResponces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.SignalR
{
    public class ServerResponce : ISignalRRecvQueueMsg
    {
        private Responce responce;

        public ServerResponce(Responce responce)
        {
            this.responce = responce;
        }

        public Responce ResponceForQuad
        {
            get { return this.responce; }
        }
    }
}
