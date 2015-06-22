﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.SignalR
{
    public interface ISignalRClientProxyMgr
    {
        Task<bool> PostToServer<T>(T msg);

        Task StartClientProxies();
    }
}
