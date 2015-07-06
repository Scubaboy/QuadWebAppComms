using QuadComms.Interfaces.SignalR;
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

        public QuadSigRMgr(List<ISignalRClientProxy> clientHubProxies)
        {
            this.clientHubProxies = clientHubProxies;
        }

        public async Task<bool> PostToServer<T>(T msg)
        {
            var result = await this.msgToHubMap[typeof(T)].Post<T>(msg).ConfigureAwait(false);

            return result;
        }

        public async Task StartClientProxies()
        {
            //Start all client proxies.
            await Task.WhenAll(this.clientHubProxies.Select(proxy => proxy.StartClientProxy())).ConfigureAwait(false);
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
