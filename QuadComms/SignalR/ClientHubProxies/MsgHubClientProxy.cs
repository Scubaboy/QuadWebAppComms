using Microsoft.AspNet.SignalR.Client;
using QuadComms.DataPckControllers.DataPckRecvControllers.MsgDataPckController;
using QuadComms.Interfaces.SignalR;
using QuadSignalRMsgs.HubResponces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuadComms.SignalR.ClientHubProxies
{
    internal class MsgHubClientProxy : ISignalRClientProxy
    {
        private string hubUrl;
        
        private IHubProxy hubProxy;
        
        private List<Type> supportedMsgs = new List<Type>
        {
            typeof(MsgDataPckController)
        };

        private Dictionary<Type, string> QuadMsgToHubMethMap = new Dictionary<Type, string>
        {
            {typeof(MsgDataPckController), "MsgFromQuad"}
        };

        private HubConnection hub;

        public MsgHubClientProxy(string hubUrl)
        {
            this.hubUrl = hubUrl;
        }

        public List<Type> SupportedMsgTypes
        {
            get { return supportedMsgs; }
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

        public async Task StartClientProxy()
        {
            //Start connection to server hub
            this.hub = new HubConnection(this.hubUrl);
            this.hubProxy = this.hub.CreateHubProxy("MsgHub");

            await this.hub.Start().ConfigureAwait(false);
           
            //Register client proxy methods
            this.hubProxy.On<MsgResponce>("SendMsgResponceToQuad", async (i) => await this.ProcessMsgresponce().ConfigureAwait(false));
        }

        private Task ProcessMsgresponce()
        {
            return Task.Run(() => 
            {
                //Add message to singalR transmit queue.
            });
        }
    }
}
