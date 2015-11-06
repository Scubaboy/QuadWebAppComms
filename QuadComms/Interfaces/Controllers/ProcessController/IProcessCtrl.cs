using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Interfaces.Controllers.ProcessController
{
    public interface IProcessCtrl
    {
        int CommsChannelPeriod { get; }
        int SetCommsChannelPeriod { set; }
        int HeartBeatCtrlPeriod { get; }
        int setHeartBeatCtrlPeriod { get; }
    }
}
