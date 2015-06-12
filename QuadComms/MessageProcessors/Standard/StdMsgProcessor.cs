﻿using QuadComms.DataPckControllers.DataPckRecvControllers;
using QuadComms.DataPckControllers.DataPckRecvControllers.FlightDataDataPckController;
using QuadComms.DataPckDecoderControllers.DecoderTypes;
using QuadComms.DataPcks;
using QuadComms.DataPcks.FlightDataPck;
using QuadComms.DataPckStructs;
using QuadComms.Interfaces.MsgProcessor;
using QuadComms.Interfaces.Queues;
using QuadComms.QueuePackets.SigRPost;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuadComms.MessageProcessors.Standard
{
    public class StdMsgProcessor : IMsgProcessor
    {
        private ConcurrentQueue<IPostQueueMsg> postQueue;
        private ConcurrentQueue<IQuadRecvMsgQueue> quadRecvQueue;
        private ConcurrentQueue<ISignalRRecvQueueMsg> sigRRecvQueue;
        private ConcurrentQueue<ISigRPostQueueMsg<DataPckRecvController>> sigRPostQueue;
        private UInt32 lastMsgCRC = 0;

        private const int MsgProcessTaskSleep = 500;

        public StdMsgProcessor(ConcurrentQueue<IQuadRecvMsgQueue> quadRecvQueue, 
            ConcurrentQueue<ISignalRRecvQueueMsg> sigRRecvQueue,
            ConcurrentQueue<ISigRPostQueueMsg<DataPckRecvController>> sigRPostQueue, 
            ConcurrentQueue<IPostQueueMsg> postQueue)
        {
            this.postQueue = postQueue;
            this.quadRecvQueue = quadRecvQueue;
            this.sigRRecvQueue = sigRRecvQueue;
            this.sigRPostQueue = sigRPostQueue;
        }

        public Task Start()
        {
            return Task.Run(()=>{
                while(true)
                {
                    //Process quad received messages
                    this.processQuadRecvMsgs();

                    //Process sigR recived messages
                    this.processSigRRecvMsgs();

                    Thread.Sleep(MsgProcessTaskSleep);
                }
            });
           
        }

        private void processSigRRecvMsgs()
        {
            if (this.quadRecvQueue.Any())
            {
                IQuadRecvMsgQueue nextMsg = null;

                if (this.quadRecvQueue.TryDequeue(out nextMsg))
                {
                    if (this.lastMsgCRC != nextMsg.CRC)
                    {
                        //Have a new msg not just a repeat of the last one.
                        this.lastMsgCRC = nextMsg.CRC;

                        switch (nextMsg.Msg.Type)
                        {
                            case DataPckTypes.DataPcks.FlightData:
                                {
                                    //post to the signalR queue.
                                    sigRPostQueue.Enqueue(new SigRPostPck<FlightDataDataPckController>(
                                        new FlightDataDataPckController((FlightData)nextMsg.Msg)
                                        {
                                            CRCStatus = DecodeStatus.Complete
                                        }));

                                    break;
                                } 
                        }
                    }
                }
            }
        }

        private void processQuadRecvMsgs()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        /* private void BuildDataPck(DecodedDataPck dataPck)
        {
            if (dataPck.DataPck != null && dataPck.Status == DecodeStatus.Complete)
            {
                if (this.lastMsgReceived != DataPckTypes.DataPcks.SendConf && 
                    this.lastMsgReceived == dataPck.DataPck.Type &&
                    this.transAction != TransmissionAction.WaitingAck)
                {
                    //Send Ack
                    this.SendDataPck(new ReSendDataPckController(DataPckTypes.False)
                        {
                            CrcController = new CRC32()
                        }.GetByteArray());

                    this.lastMsgReceived = DataPckTypes.DataPcks.NoMsg;
                   // this.ignoreNextDataPck = true;
                }
                else
                {  
                    this.SetLastMsgReceived(dataPck.DataPck);
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
                        case DataPckTypes.DataPcks.FlightData:
                            {
                                recvedDataPcks.Add(
                                    new FlightDataDataPckController((FlightData)dataPck.DataPck)
                                    {
                                        CRCStatus = dataPck.Status
                                    });
                               
                                break;
                            }
                        case DataPckTypes.DataPcks.Message:
                        case DataPckTypes.DataPcks.FreeTxtMsg:
                            {
                                    recvedDataPcks.Add(
                                        new MsgDataPckController((MsgData)dataPck.DataPck)
                                        {
                                            CRCStatus = dataPck.Status
                                        });

                                if (dataPck.DataPck.Type != DataPckTypes.DataPcks.FreeTxtMsg)
                                {
                                    this.SendDataPck(new ReSendDataPckController(DataPckTypes.False)
                                        {
                                            CrcController = new CRC32()
                                        }.GetByteArray());

                                    this.lastMsgReceived = DataPckTypes.DataPcks.NoMsg;
                                }

                                break;
                            }
                            case DataPckTypes.DataPcks.RequestData:
                            {
                                recvedDataPcks.Add(new DataRequestDataPckController((DataRequest)dataPck.DataPck)
                                    {
                                        CRCStatus = dataPck.Status
                                    });

                                this.SendDataPck(new ReSendDataPckController(DataPckTypes.False)
                                {
                                    CrcController = new CRC32()
                                }.GetByteArray());

                                this.lastMsgReceived = DataPckTypes.DataPcks.NoMsg;
                                break;
                            }
                        case DataPckTypes.DataPcks.SystemId:
                            {
                                recvedDataPcks.Add(
                                    new SystemIdDataPckController((SystemId)dataPck.DataPck)
                                    {
                                        CRCStatus = dataPck.Status
                                    });
                                break;
                            }
                        case DataPckTypes.DataPcks.DataLogger:
                            {
                                recvedDataPcks.Add(
                                    new DataLoggerDataPckController((DataLogger)dataPck.DataPck)
                                    {
                                        CRCStatus = dataPck.Status
                                    });
                                break;
                            }
                        default:
                            {
                                break;
                            }
                    }
                }

                
            }
        }*/
    }
}
