using MbedQuad;
using Ninject;
using QuadComms.CRC32Generator;
using QuadComms.DataPckControllers.DataPckRecvControllers;
using QuadComms.DataPckControllers.DataPckRecvControllers.DataLoggerDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.DataRequestDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.FlightDataDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.MsgDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.SystemIdDataPckController;
using QuadComms.DataPckControllers.DataPckTransControllers.ReSendDataPckController;
using QuadComms.DataPckDecoderControllers.DecoderTypes;
using QuadComms.DataPcks;
using QuadComms.DataPcks.DataLoggerDataPck;
using QuadComms.DataPcks.DataRequestDataPck;
using QuadComms.DataPcks.FlightDataPck;
using QuadComms.DataPcks.MsgDataPck;
using QuadComms.DataPcks.SendConfPck;
using QuadComms.DataPcks.SystemId;
using QuadComms.DataPckStructs;
using QuadComms.Interfaces.CommsChannel;
using QuadComms.Interfaces.CommsController;
using QuadComms.Interfaces.CommsDevice;
using QuadComms.Interfaces.DataDecoder;
using QuadComms.Interfaces.Queues;
using QuadComms.QueuePackets.QuadRecv;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace QuadComms.CommControllers
{
   

    internal class CommController : ICommsController
    {
        

        private const int RetryThreshold = 10;
        private const int SendRecvTaskSleep =   1;
        private const int ProgressUpdatePeriod = 5;
        private const int SendPeriod = 3000;
        private const int TicksBetweenProgreeUpdates = ProgressUpdatePeriod/SendRecvTaskSleep;
        private const int TicksBetweenSends = SendPeriod/SendRecvTaskSleep;
        private const string StartMarker = "<<";
        private const string EndMarker = ">>";
        private TransmissionAction transAction;
        private byte[] receiveBufBuildingPck;
        private IQuadTransQueueMsg dataPckSent;
        private IDataDecoder dataPckDecoder;
        private List<byte> rawDataPack;
        private ICommsChannel commsChannel;
        IDataTransferQueue<IQuadRecvMsgQueue> recvQueue;
        IDataTransferQueue<IQuadTransQueueMsg> postQueue;
        
        private int sendTicks;
        private int failedSendsLastProgress;



        public CommController(
            ICommsChannel commsChannel,
            IDataDecoder dataPckDecoder,
            [Named("QuadRecvQueue")]IDataTransferQueue<IQuadRecvMsgQueue> recvQueue,
            [Named("QuadTransQueue")]IDataTransferQueue<IQuadTransQueueMsg> postQueue)
        {
            this.dataPckDecoder = dataPckDecoder;
            this.commsChannel = commsChannel;
            this.recvQueue = recvQueue;
            this.postQueue = postQueue;
        }

        public void Setup()
        {
            this.receiveBufBuildingPck = new byte[DataPckTypes.DataPckSendRecvSize];
            this.transAction = TransmissionAction.WaitingDataPckSend;
            this.sendTicks = 0;
            this.failedSendsLastProgress = 0;
            this.rawDataPack  = new List<byte>();           
        }

        public Task Start(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
                {
                    this.commsChannel.ClearInput();
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        this.commsChannel.ProcessCommsChannel();
                        this.ReceiveDataPckAction();
                        this.TransmitAction();
                      
                        Thread.Sleep(SendRecvTaskSleep);
                    }

                    this.commsChannel.Close();
                });
        }

        private void ReceiveDataPckAction()
        {
            if (this.commsChannel.DataPcksAvailable())
            {
                byte[] dataPckRecvRaw;

                if (this.commsChannel.TakeDataPck(out dataPckRecvRaw))
                {
                    DecodedDataPck decodedDataPck;
                    this.dataPckDecoder.Decode(dataPckRecvRaw, out decodedDataPck);
                    this.ProcessNewDataPck(decodedDataPck);
                }
            }
        }

        private void TransmitAction()
        {
            if (this.sendTicks >= TicksBetweenSends)
            {
                switch (this.transAction)
                {
                    case TransmissionAction.WaitingAck:
                        {
                            this.failedSendsLastProgress++;
                            this.SendDataPck(dataPckSent.Data);
                            break;
                        }
                    case TransmissionAction.WaitingDataPckSend:
                        {
                            if (this.postQueue.Any())
                            {
                                if (this.postQueue.Remove(out dataPckSent))
                                {
                                    if (dataPckSent.Ackrequired)
                                    {
                                       this.transAction = TransmissionAction.WaitingAck;
                                    }
                                    else
                                    {
                                        this.transAction = TransmissionAction.WaitingDataPckSend;
                                    }

                                    this.SendDataPck(dataPckSent.Data);  
                                }
                            }
                            break;
                        }
                }

                this.sendTicks = 0;
            }
            else
            {
                this.sendTicks++;
            }
        }

        private void ProcessNewDataPck(DecodedDataPck dataPck)
        {
            if (dataPck.DataPck != null && dataPck.Status == DecodeStatus.Complete)
            {
                switch (dataPck.DataPck.Type)
                {
                    case DataPckTypes.DataPcks.SendConf:
                        {
                            if (this.transAction == TransmissionAction.WaitingAck)
                            {
                                this.transAction = TransmissionAction.WaitingDataPckSend;
                            }
                            break;
                        }
                    default:
                        {
                            if (dataPck.DataPck.AckRequired == DataPckTypes.True)
                            {
                                //Send Ack
                            //    this.SendDataPck(new ReSendDataPckController(DataPckTypes.False)
                            //    {
                             //       CrcController = new CRC32()
                            //    }.GetByteArray());
                            }

                            //Push the new message onto the recv queue for the message processing obj
                            this.recvQueue.Add(new QuadRecvPck(dataPck.DataPck, dataPck.DataPckCrc));

                            break;
                        }
                }
            }
        }

        private void SendDataPck(byte[] dataPckToSend)
        {
            var blocksSent = 0;
            var blockOffset = 0;

            Debug.WriteLine("Send msg type {0}", BitConverter.ToUInt32(dataPckToSend, 4));
            this.commsChannel.QueueDataPck(dataPckToSend); 
        }
    }
}
