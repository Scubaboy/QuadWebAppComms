using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using MbedQuad;
using QuadComms.CommsProgress;
using QuadComms.Interfaces.DataDecoder;

namespace QuadComms.Interfaces.CommsChannel
{
    internal interface ICommsChannel
    {
        void AppendData(byte[] data);
        void Setup();
        SystemModes SysMode { set; }
        Task ProcessCommsAsync(CancellationToken cancellationToken);
        Task ReadSerial(CancellationToken cancellationToken);
        IDataDecoder DataPckDecoder { get; set; }
    }
}
