using QuadComms.Interfaces.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.SignalR.MessageHub
{
    public class MessageHugClient : ISignalRClient
    {
        public MessageHugClient()
        {

        }

        public Task ConnectToHubAsync(string hubName)
        {
            throw new NotImplementedException();
        }

        public Task<T> CallServerSideMethod<T>(string methodName)
        {
            throw new NotImplementedException();
        }
    }
}
