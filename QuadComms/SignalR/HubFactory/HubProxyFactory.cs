using Microsoft.AspNet.SignalR.Client;
using QuadComms.Interfaces.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.SignalR.HubFactory
{
    internal class HubProxyFactory : IHubProxyFactory
    {
        private List<HubConnection> activeConnections = new List<HubConnection>();

        public async Task<IHubProxy> Create(string hubUrl, string hub)
        {
            var connection  = new HubConnection(hubUrl);

            this.activeConnections.Add(connection);

            await connection.Start().ConfigureAwait(false);

            return connection.CreateHubProxy(hub);
        }
    }
}
