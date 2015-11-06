namespace QuadComms.Controllers.QuadStatusController
{
    using Ninject;
    using QuadComms.CRC32Generator;
    using QuadComms.DataPckControllers.DataPckTransControllers.HeartBeatDataPckController;
    using QuadComms.Interfaces.Controllers.CommsContStatusController;
    using QuadComms.Interfaces.Controllers.ProcessController;
    using QuadComms.Interfaces.Controllers.QuadStatusController;
    using QuadComms.Interfaces.Logging;
    using QuadComms.Interfaces.Queues;
    using QuadComms.QueuePackets.Post;
    using QuadModels;
    using System;

    public class QuadStatusCtrl : IQuadStatusCtrl
    {
        private const int HeartBeatThreshold = 60;

        private ICommsContStatusCtrl connectionStatus;
        private IDataTransferQueue<IQuadTransQueueMsg> postQueue;
        private ILogger localLogger;
        private IProcessCtrl ProcessCtrl;
        private int statusCycleTotal;
        private int cycleTotal;
        private uint quadId;
        private DateTime timeHeartBeatLastReceived;
        private DateTime heartBeatPreviousTime;
        private DateTime statusUpdatePreviousTime;

        public QuadStatusCtrl(
            ICommsContStatusCtrl connectionStatus,
            IProcessCtrl ProcessCtrl,
            uint quadId,
            [Named("QuadTransQueue")]IDataTransferQueue<IQuadTransQueueMsg> postQueue,
            ILogger localLogger)
        {
            this.connectionStatus = connectionStatus;
            this.postQueue = postQueue;
            this.localLogger = localLogger;
            this.ProcessCtrl = ProcessCtrl;
            this.cycleTotal = 0;
            this.statusCycleTotal = 0;
            this.quadId = quadId;
            this.timeHeartBeatLastReceived = DateTime.Now;
            this.heartBeatPreviousTime = DateTime.Now;
            this.statusUpdatePreviousTime = DateTime.Now;
        }

        public void UpdateHeartBeat()
        {
            var updateThreshhold = this.ProcessCtrl.HeartBeatCtrlPeriod / this.ProcessCtrl.CommsChannelPeriod;

            var difference = DateTime.Now - this.heartBeatPreviousTime;

            this.heartBeatPreviousTime = DateTime.Now;

            this.cycleTotal += (int)difference.TotalMilliseconds;

            if (this.cycleTotal >= updateThreshhold)
            {
                //Post a heartbeat datapck on the post queue.
                this.postQueue.Add(
                    new PostPck(
                        new HeatBeatDataPckCtrl()
                        {
                            CrcController = new CRC32()
                        }.GetByteArray(),
                    (int)this.quadId,
                    false));
                
                this.cycleTotal = 0;
            }
            else
            {
                this.cycleTotal++;
            }
        }


        public uint AttachedQuadId
        {
            get { return this.quadId; }
        }


        public bool HeartBeatReceived
        {
            get
            {
                return (DateTime.Now - this.timeHeartBeatLastReceived).TotalSeconds < HeartBeatThreshold;
            }
        }


        public void ReportStatus(out Status status)
        {
            var updateThreshhold = this.ProcessCtrl.HeartBeatCtrlPeriod / this.ProcessCtrl.CommsChannelPeriod;

            var difference = DateTime.Now - this.statusUpdatePreviousTime;

            this.statusUpdatePreviousTime = DateTime.Now;

            this.statusCycleTotal += (int)difference.TotalMilliseconds;

            if (this.statusCycleTotal >= updateThreshhold)
            {
                status = new Status((int)this.quadId, this.timeHeartBeatLastReceived, this.HeartBeatReceived);

                statusCycleTotal = 0;
            }
            else
            {
                status = null;
                statusCycleTotal++;
            }
        }


        public void UpdateQuadsReceivedHeartBeat()
        {
            this.timeHeartBeatLastReceived = DateTime.Now;
        }
    }
}
