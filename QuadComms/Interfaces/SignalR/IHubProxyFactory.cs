using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.SignalR
{
    public interface IHubProxyFactory
    {
        Task<IHubProxy> Create(string hubUrl, string hub);
    }
}
