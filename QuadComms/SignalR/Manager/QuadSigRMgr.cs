using Ninject;
using QuadComms.DataPckControllers.DataPckRecvControllers;
using QuadComms.Interfaces.Queues;
using QuadComms.Interfaces.SignalR;
using QuadComms.SignalR.ClientHubProxies;
using QuadSignalRMsgs.HubResponces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuadComms.SignalR.Manager
{
    internal class QuadSigRMgr : ISignalRClientProxyMgr
    {
        private Dictionary<Type, ISignalRClientProxy>  msgToHubMap = new Dictionary<Type, ISignalRClientProxy>();
        private List<BaseMsgHubClientProxy> clientHubProxies;
        private IDataTransferQueue<ISigRPostQueueMsg<DataPckRecvController>> sigRPostQueue;
        private IDataTransferQueue<ISignalRRecvQueueMsg> sigRRecvQueue;

        public QuadSigRMgr(List<BaseMsgHubClientProxy> clientHubProxies,
            [Named("SigRRecvQueue")]IDataTransferQueue<ISignalRRecvQueueMsg> sigRRecvQueue,
            [Named("SigRTransQueue")]IDataTransferQueue<ISigRPostQueueMsg<DataPckRecvController>> sigRPostQueue)
        {
            this.clientHubProxies = clientHubProxies;
            this.sigRPostQueue = sigRPostQueue;
            this.sigRRecvQueue = sigRRecvQueue;

        }

        private async Task<ReceiveResponce> PostToServer<T>(T msg)
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

                //Register client proxy methods
                this.clientHubProxies.ForEach(client => client.RegisterClientProxyMethods());

                while (true)
                {
                    //Process message requests.
                    if (this.sigRPostQueue.Any())
                    {
                        ISigRPostQueueMsg<DataPckRecvController> post = null;
                        if (this.sigRPostQueue.Remove(out post))
                        {
                            await this.PostToServer(post.Msg).ConfigureAwait(false);
                        }
                    }

                    //Check client proxies for responce messages.
                    this.clientHubProxies.ForEach((client) =>
                        {
                            if (client.ServerMsgWaiting)
                            {
                                var responce = client.TakePendingMsg();

                                this.sigRRecvQueue.Add(responce);
                            }
                        });

                    Thread.Sleep(1000);
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
