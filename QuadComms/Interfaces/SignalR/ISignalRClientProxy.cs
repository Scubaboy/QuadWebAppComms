using QuadSignalRMsgs.HubResponces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.SignalR
{
    public interface ISignalRClientProxy
    {
        List<Type> SupportedMsgTypes { get; }

        Task<ReceiveResponce> Post<T>(T msg);

        Task StartClientProxy();

    }
}
