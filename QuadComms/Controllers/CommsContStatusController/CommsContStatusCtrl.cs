namespace QuadComms.Controllers.CommsContStatusController
{
    using QuadComms.Interfaces.Controllers.CommsContStatusController;
    using System;

    class CommsContStatusCtrl : ICommsContStatusCtrl
    {
        Mode connectionStatus = Mode.Synching;

        public Mode ChannelConStatus
        {
            get { return this.connectionStatus; }
        }

        public Mode SetChannelConStatus
        {
            set { this.connectionStatus = value; }
        }
    }
}
