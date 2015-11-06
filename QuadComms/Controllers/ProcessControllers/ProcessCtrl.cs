using QuadComms.Interfaces.Controllers.ProcessController;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuadComms.Controllers.ProcessControllers
{
    public class ProcessCtrl : IProcessCtrl
    {
        private int channelPeriod;
        private int heartBeatCtrlPeriod;

        public ProcessCtrl()
        {
            this.channelPeriod = 1;
            this.heartBeatCtrlPeriod = 10000;
        }

        public int CommsChannelPeriod
        {
            get { return this.channelPeriod; }
        }

        public int SetCommsChannelPeriod
        {
            set { throw new NotImplementedException(); }
        }

        public int HeartBeatCtrlPeriod
        {
            get { return heartBeatCtrlPeriod; }
        }

        public int setHeartBeatCtrlPeriod
        {
            get { throw new NotImplementedException(); }
        }
    }
}
