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

namespace QuadComms
{
    public class CommsController
    {
        private ICommsController commChannel;
        private IMsgProcessor msgProcessor;
        private NinjectIoC kernel;

        private CancellationToken cancelToken;

        public CommsController( SupportedChannels channel)
        {
            switch (channel)
            {
                case SupportedChannels.Comm:
                    {
                        this.kernel = new NinjectIoC();
                       

                        commChannel = this.kernel.Kernel.Get<ICommsController>();
                        msgProcessor = this.kernel.Kernel.Get<IMsgProcessor>();
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
          
            await Task.WhenAll(commChannel.Start(this.cancelToken), this.msgProcessor.Start());
        }   
    }
}
