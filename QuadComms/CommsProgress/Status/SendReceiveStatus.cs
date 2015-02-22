using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuadComms.Interfaces.CommsChannel;

namespace QuadComms.CommsProgress.Status
{
    class SendReceiveStatus : ICommStatus
    {
        private readonly int failedSends;
        private readonly int failedReceives;

        public SendReceiveStatus(int failedSends, int failedReceives)
        {
            this.failedSends = failedSends;
            this.failedReceives = failedReceives;
        }

        public int FailedSends
        {
            get { return this.failedSends; }
        }

        public int FailedReceives
        {
            get { return this.failedReceives; }
        }
    }
}
