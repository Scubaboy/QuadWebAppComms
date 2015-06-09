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

namespace QuadComms
{
    public class CommsController
    {
        private ICommsChannel commChannel;
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
            await commChannel.Start(this.cancelToken);
        }

        //COuld use signalr for message trans and mode changes.

     //   public async Task CommsSerialReaderAsync()
      //  {
      //      this.commChannel.Setup();
      //      await commChannel.ReadSerial(this.cancelToken);
      //  }
      //  public void AddDataPckToSendQueue(byte[] dataPck)
      //  {
     //       this.commChannel.AppendData(dataPck);
     //   }

      //  public SystemModes Mode
     //   {
      //      set
      //      {
      //          if (this.commChannel != null)
      //          {
      //              this.commChannel.SysMode = value;
      //          }
      //      }
     //   }

   
    }
}
