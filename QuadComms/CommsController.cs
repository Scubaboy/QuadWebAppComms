using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MbedQuad;
using QuadComms.CRC32Generator;
using QuadComms.CommControllers;
using QuadComms.CommsProgress;
using QuadComms.DataPckDecoderControllers.Binary;
using QuadComms.Interfaces.CommsChannel;
using QuadComms.Interfaces.MsgProcessor;
using QuadComms.IoC.Ninject;
using Ninject;
using QuadComms.Interfaces.CommsController;
using QuadComms.Interfaces.SignalR;

namespace QuadComms
{
    public class CommsController
    {
        private ICommsController commChannel;
        private IMsgProcessor msgProcessor;
        private ISignalRClientProxyMgr signalRMgr;
        private ICommsChannel commsChannel;
        private CancellationToken cancelToken;

        public CommsController( SupportedChannels channel)
        {
            switch (channel)
            {
                case SupportedChannels.Comm:
                    {


                        commsChannel = NinjectIoC.Kernel.Get<ICommsChannel>();
                        commChannel = NinjectIoC.Kernel.Get<ICommsController>();
                        msgProcessor = NinjectIoC.Kernel.Get<IMsgProcessor>();
                        this.signalRMgr = NinjectIoC.Kernel.Get<ISignalRClientProxyMgr>();
                        break;
                    }
                    case SupportedChannels.Tcpip:
                    {
                        commChannel = null;
                        break;
                    }
            }

           

        }

        public async Task CommsControllerAsync()
        {
            this.commChannel.Setup();

            await Task.WhenAll(commChannel.Start(this.cancelToken),commsChannel.Start(this.cancelToken), this.msgProcessor.Start());//, this.signalRMgr.Start()).ConfigureAwait(false);
        }   
    }
}
