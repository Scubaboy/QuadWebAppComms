using QuadSignalRMsgs.HubResponces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.SignalR
{
    public interface ISignalRClientProxyMgr
    {
        Task<ReceiveResponce> PostToServer<T>(T msg);

        Task Start();
    }
}
