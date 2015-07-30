using Ninject;
using QuadComms.DataPckControllers.DataPckRecvControllers;
using QuadComms.Interfaces.Queues;
using QuadComms.Interfaces.SignalR;
using QuadSignalRMsgs.HubResponces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.SignalR.Manager
{
    internal class QuadSigRMgr : ISignalRClientProxyMgr
    {
        private Dictionary<Type, ISignalRClientProxy>  msgToHubMap = new Dictionary<Type, ISignalRClientProxy>();
        private List<ISignalRClientProxy> clientHubProxies;

        public QuadSigRMgr(List<ISignalRClientProxy> clientHubProxies,
            [Named("SigRRecvQueue")]IDataTransferQueue<ISignalRRecvQueueMsg> sigRRecvQueue,
            [Named("SigRTransQueue")]IDataTransferQueue<ISigRPostQueueMsg<DataPckRecvController>> sigRPostQueue)
        {
            this.clientHubProxies = clientHubProxies;
        }

        public async Task<ReceiveResponce> PostToServer<T>(T msg)
        {
            var result = await this.msgToHubMap[typeof(T)].Post<T>(msg).ConfigureAwait(false);

            return result;
        }

        public Task Start()
        {
            return Task.Run(async () =>
            {
                 
                this.RegisterClientMsgTypes();

                //Start all client proxies.
                await Task.WhenAll(this.clientHubProxies.Select(proxy => proxy.StartClientProxy())).ConfigureAwait(false);

                while (true)
                {
                    //Process message requests.
                }
            });
            
        }

        private void RegisterClientMsgTypes()
        {
            this.clientHubProxies.ForEach(hubProxy =>
                {
                    hubProxy.SupportedMsgTypes.ForEach(msgType => {
                        this.msgToHubMap.Add(msgType, hubProxy);
                    });
                });
        }
    }
}
