using Microsoft.AspNet.SignalR.Client;
using QuadComms.Interfaces.Queues;
using QuadComms.Interfaces.SignalR;
using QuadSignalRMsgs.HubResponces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.SignalR.ClientHubProxies
{
    public abstract class BaseMsgHubClientProxy : ISignalRClientProxy
    {
        protected Dictionary<Type, string> QuadMsgToHubMethMap = new Dictionary<Type,string>();
        protected IHubProxy hubProxy;
        protected  ConcurrentQueue<Responce> internalResponceQueue = new ConcurrentQueue<Responce>();

        public List<Type> SupportedMsgTypes
        {
            get { throw new NotImplementedException(); }
        }

        public async Task<ReceiveResponce> Post<T>(T msg)
        {
            ReceiveResponce result;

            if (QuadMsgToHubMethMap.ContainsKey(msg.GetType()))
            {
                var hubCall = QuadMsgToHubMethMap[msg.GetType()];
                result = await this.hubProxy.Invoke<ReceiveResponce>(hubCall, msg).ConfigureAwait(false);
            }
            else
            {
                result = new ReceiveResponce(false);
            }

            return result;
        }

        public bool ServerMsgWaiting
        {
            get { return this.internalResponceQueue.Count > 0; }
        }

        public abstract  ISignalRRecvQueueMsg TakePendingMsg();

        public abstract Task StartClientProxy();

        public abstract void RegisterClientProxyMethods();
    }
}
