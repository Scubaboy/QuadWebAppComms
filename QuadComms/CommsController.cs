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

namespace QuadComms
{
    public class CommsController
    {
        private ICommsChannel commChannel;
        private IMsgProcessor msgProcessor;

        private CancellationToken cancelToken;

        public CommsController(CancellationToken cancellationToken, SupportedChannels channel)
        {
            switch (channel)
            {
                case SupportedChannels.Comm:
                    {
                       // commChannel = new CommPortController("com5", 9600, Parity.None, StopBits.One, Handshake.None,8);
                      //  var decoder = new BinaryDecoder();
                      //  decoder.CrcController = new CRC32();
                       // commChannel.DataPckDecoder = decoder;
                        break;
                    }
                    case SupportedChannels.Tcpip:
                    {
                        commChannel = null;
                        break;
                    }
            }

            this.cancelToken = cancellationToken;

        }

        public async Task CommsControllerAsync()
        {
            this.commChannel.Setup();
          
            await Task.WhenAll(commChannel.Start(this.cancelToken), this.msgProcessor.Start());
        }   
    }
}
