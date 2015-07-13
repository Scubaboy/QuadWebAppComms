using Microsoft.AspNet.SignalR.Client;
using QuadComms.Interfaces.SignalR;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QuadComms.SignalR.ClientHubProxies
{
    internal class MsgHubClientProxy : ISignalRClientProxy
    {
        private string hubUrl;
        private IHubProxy hubProxy;

        private HubConnection hub;

        public MsgHubClientProxy(string hubUrl)
        {
            this.hubUrl = hubUrl;
        }

        public List<Type> SupportedMsgTypes
        {
            get { throw new NotImplementedException(); }
        }

        public async Task<bool> Post<T>(T msg)
        {
            await this.hubProxy.Invoke("");

            return true;
        }

        public async Task StartClientProxy()
        {
            //Start connection to server hub
            this.hub = new HubConnection(this.hubUrl);
            this.hubProxy = this.hub.CreateHubProxy("MsgHub");

            await this.hub.Start().ConfigureAwait(false);
           
            //Register client proxy methods

        }
    }
}
