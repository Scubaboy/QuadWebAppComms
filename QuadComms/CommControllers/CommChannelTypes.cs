using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.CommControllers
{
    public enum SupportedChannels
    {
        /// <summary>
        /// 
        /// </summary>
        Comm,

        /// <summary>
        /// 
        /// </summary>
        Tcpip
    }

    internal enum ReceivedAction
    {
        Waiting,
        BuildingDataPck
    }

    internal enum TransmissionAction
    {
        WaitingAck,
        WaitingDataPckSend
    }
}
