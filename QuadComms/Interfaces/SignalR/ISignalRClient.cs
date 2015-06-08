using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.SignalR
{
    public interface ISignalRClient
    {
        Task ConnectToHubAsync(string hubName);

        Task<T> CallServerSideMethod<T>(String methodName);

    }
}
