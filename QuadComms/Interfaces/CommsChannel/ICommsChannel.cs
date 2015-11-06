using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.CommsChannel
{
    public interface ICommsChannel
    {
        void ClearInput();
        bool DataPcksAvailable();
        bool TakeDataPck(out byte[] rawDataPck);
        void QueueDataPck(byte[] dataPck);
        void Close();
        Task Start(CancellationToken cancellationToken);
    }
}
