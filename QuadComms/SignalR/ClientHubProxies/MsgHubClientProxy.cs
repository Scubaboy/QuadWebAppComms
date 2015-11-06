using Microsoft.AspNet.SignalR.Client;
using QuadComms.DataPckControllers.DataPckRecvControllers.MsgDataPckController;
using QuadComms.Interfaces.Queues;
using QuadComms.Interfaces.SignalR;
using QuadSignalRMsgs.HubResponces;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuadComms.SignalR.ClientHubProxies
{
    public class MsgHubClientProxy : BaseMsgHubClientProxy
    {
        private List<Type> supportedMsgs = new List<Type>
        {
            typeof(MsgDataPckController)
        };

        private IHubProxyFactory hubProxyfactory;
        private string hubUrl;

        public MsgHubClientProxy(IHubProxyFactory hubProxyFactory, string hubUrl)
        {
            this.hubProxyfactory = hubProxyFactory;
            this.hubUrl = hubUrl;
        }

        public override List<Type> SupportedMsgTypes
        {
            get { return supportedMsgs; }
        }


        public override async Task StartClientProxy()
        {
            //Start connection to server hub
            this.hubProxy = await this.hubProxyfactory.Create(this.hubUrl, "MsgHub").ConfigureAwait(false);

            this.QuadMsgToHubMethMap = new Dictionary<Type, string>
            {
                {typeof(MsgDataPckController), "MsgFromQuad"}
            };
        }

        public override void RegisterClientProxyMethods()
        {
            //Register client proxy methods
            this.hubProxy.On<Responce>("SendMsgResponceToQuad", async (i) => await this.ProcessMsgresponce(i).ConfigureAwait(false));
        }

        private Task ProcessMsgresponce(Responce responceFromUI)
        {
            var responceToAdd = responceFromUI;
            return Task.Run(() => 
            {
                internalResponceQueue.Enqueue(responceToAdd);
            });
        }

        public override ISignalRRecvQueueMsg TakePendingMsg()
        {
            
                Responce newResponce = null;

                this.internalResponceQueue.TryDequeue(out newResponce);

                return new ServerResponce(newResponce);
            
        }
    }
}
