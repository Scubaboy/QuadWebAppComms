using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MbedQuad;
using QuadComms.CommsProgress;
using QuadComms.Interfaces.DataDecoder;

namespace QuadComms.Interfaces.CommsController
{
    internal interface ICommsController
    {
        void Setup();
        Task Start(CancellationToken cancellationToken);
    }
}
