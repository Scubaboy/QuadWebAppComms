using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.CommsChannel
{
    public interface ICommsChannel
    {
        void ProcessCommsChannel(); 
        bool DataPcksAvailable();
        bool TakeDataPck(out byte[] rawDataPck);
        void QueueDataPck(byte[] dataPck);
        void Close();
    }
}
