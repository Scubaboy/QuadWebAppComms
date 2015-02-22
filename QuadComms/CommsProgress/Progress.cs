using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.DataPckControllers.DataPckRecvControllers;
using QuadComms.Interfaces.CommsChannel;

namespace QuadComms.CommsProgress
{
    public class Progress
    {
        private List<DataPckRecvController> receivedDataPcks;
        private ICommStatus status;
        private bool gotMsgConf;

        public Progress(List<DataPckRecvController> receivedDataPcks,ICommStatus status, bool gotMsgConf)
        {
            this.receivedDataPcks = receivedDataPcks;
            this.status = status;
            this.gotMsgConf = gotMsgConf;
        }

        public List<DataPckRecvController> ReceivedDataPcks
        {
            get { return this.receivedDataPcks; }
        }

        public ICommStatus Status
        {
            get { return this.status; }
        }

        public bool GotMsgConf
        {
            get { return this.gotMsgConf; }
        }
    }
}
