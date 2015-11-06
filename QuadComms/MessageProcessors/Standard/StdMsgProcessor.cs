using Ninject;
using QuadComms.CRC32Generator;
using QuadComms.DataPckControllers.DataPckRecvControllers;
using QuadComms.DataPckControllers.DataPckRecvControllers.FlightDataDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.MsgDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.SystemIdDataPckController;
using QuadComms.DataPckControllers.DataPckTransControllers.TimeSyncDataPckController;
using QuadComms.DataPckDecoderControllers.DecoderTypes;
using QuadComms.DataPcks;
using QuadComms.DataPcks.FlightDataPck;
using QuadComms.DataPcks.HeartBeatDataPck;
using QuadComms.DataPcks.MsgDataPck;
using QuadComms.DataPcks.SystemId;
using QuadComms.DataPckStructs;
using QuadComms.Interfaces.Breeze;
using QuadComms.Interfaces.Controllers.AttachedQuadsController;
using QuadComms.Interfaces.MsgProcessor;
using QuadComms.Interfaces.Queues;
using QuadComms.QueuePackets.Post;
using QuadComms.QueuePackets.SigRPost;
using QuadModels;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuadComms.MessageProcessors.Standard
{
    public class StdMsgProcessor : IMsgProcessor
    {
        private IDataTransferQueue<IQuadTransQueueMsg> postQueue;
        private IDataTransferQueue<IQuadRecvMsgQueue> quadRecvQueue;
        private IDataTransferQueue<ISignalRRecvQueueMsg> sigRRecvQueue;
        private IDataTransferQueue<ISigRPostQueueMsg<DataPckRecvController>> sigRPostQueue;
        private UInt32 lastMsgCRC = 0;
        private IBreezeRepository<ActiveQuad> activeQuadRepos;
        private IAttachedQuadsCtrl attachedQuads;
        private const int MsgProcessTaskSleep = 500;

        public StdMsgProcessor(
            [Named("QuadRecvQueue")]IDataTransferQueue<IQuadRecvMsgQueue> quadRecvQueue,
            [Named("SigRRecvQueue")]IDataTransferQueue<ISignalRRecvQueueMsg> sigRRecvQueue,
            [Named("SigRTransQueue")]IDataTransferQueue<ISigRPostQueueMsg<DataPckRecvController>> sigRPostQueue,
            [Named("QuadTransQueue")]IDataTransferQueue<IQuadTransQueueMsg> postQueue,
            IBreezeRepository<ActiveQuad> activeQuadRepos,
            IAttachedQuadsCtrl attachedQuads)
        {
            this.postQueue = postQueue;
            this.quadRecvQueue = quadRecvQueue;
            this.sigRRecvQueue = sigRRecvQueue;
            this.sigRPostQueue = sigRPostQueue;
            this.activeQuadRepos = activeQuadRepos;
            this.attachedQuads = attachedQuads;
        }

        public Task Start()
        {
            return Task.Run( ()=>
            {
                while(true)
                {
                    //Process quad received messages
                    this.processQuadRecvMsgs();//.ConfigureAwait(false);

                    //Process sigR recived messages
                    this.processSigRRecvMsgs();

                    Thread.Sleep(MsgProcessTaskSleep);
                }
            });
           
        }

        /// <summary>
        /// 
        /// </summary>
        private void processSigRRecvMsgs()
        {
            if (this.sigRRecvQueue.Any())
            {
                ISignalRRecvQueueMsg sigRMsg;

                if (this.sigRRecvQueue.Remove(out sigRMsg))
                {
                    var sendToQuadMsg = new PostPck(sigRMsg.ResponceForQuad.ResponceForQuad,sigRMsg.ResponceForQuad.QuadId, sigRMsg.ResponceForQuad.AckRequired);

                    this.postQueue.Add(sendToQuadMsg);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private bool processQuadRecvMsgs()
        {
            var result = true;

            if (this.quadRecvQueue.Any())
            {
                IQuadRecvMsgQueue nextMsg = null;

                if (this.quadRecvQueue.Remove(out nextMsg))
                {
                    if (this.lastMsgCRC != nextMsg.CRC || nextMsg.Msg.Type == DataPckTypes.DataPcks.HeartBeat)
                    {
                        //Have a new msg not just a repeat of the last one.
                        this.lastMsgCRC = nextMsg.CRC;

                        switch (nextMsg.Msg.Type)
                        {
                            case DataPckTypes.DataPcks.FlightData:
                                {
                                    //post to the signalR queue.
                                    sigRPostQueue.Add(new SigRPostPck<FlightDataDataPckController>(
                                        new FlightDataDataPckController((FlightData)nextMsg.Msg)
                                        {
                                            CRCStatus = DecodeStatus.Complete
                                        }));

                                    break;
                                }
                            
                            case DataPckTypes.DataPcks.FreeTxtMsg:
                                {
                                    //post to the signalR queue.
                                    sigRPostQueue.Add(new SigRPostPck<MsgDataPckController>(
                                        new MsgDataPckController((MsgData)nextMsg.Msg)
                                        {
                                            CRCStatus = DecodeStatus.Complete
                                        }));
                                    break;
                                }
                            case DataPckTypes.DataPcks.Message:
                                {
                                    //post to the signalR queue.
                                    sigRPostQueue.Add(new SigRPostPck<MsgDataPckController>(
                                        new MsgDataPckController((MsgData)nextMsg.Msg)
                                        {
                                            CRCStatus = DecodeStatus.Complete
                                        }));
                                    break;
                                }
                            case DataPckTypes.DataPcks.SystemId:
                                {
                                    //post to database
                                    var newQuad = (SystemId)nextMsg.Msg;

                                    this.attachedQuads.AddQuad(newQuad.quadID);
                                    /*this.activeQuadRepos.Add(new ActiveQuad(-1) 
                                    {
                                        InUse = false,
                                        QuadId = newQuad.quadID.ToString(),
                                        SupportedAlt = newQuad.altimeterOptions,
                                        SupportedComms = newQuad.telemtryfeeds,
                                        SupportedIMU = newQuad.imu,
                                        SupportGPS = newQuad.gpsMsgFormat
                                    });

                                   var saveResult = await this.activeQuadRepos.SaveChanges();
                                   
                                    //Post to signalr to update clients of new quad.
                                   sigRPostQueue.Add(new SigRPostPck<SystemIdDataPckController>(
                                       new SystemIdDataPckController(newQuad)
                                       {
                                           CRCStatus = DecodeStatus.Complete
                                       }));
                                    */

                                    this.postQueue.Add(
                                        new PostPck(
                                            new TimeSyncDataPckCtrl((UInt32)DateTime.Now.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds)
                                            {
                                                CrcController = new CRC32()
                                            }.GetByteArray(),
                                            (int)newQuad.quadID,
                                            true));
                                    break;
                                }
                            case DataPckTypes.DataPcks.DataLogger:
                                {
                                    //post to database
                               
                                    break;
                                }
                            case DataPckTypes.DataPcks.HeartBeat:
                                {
                                    var heartBeat = (HeartBeatData)nextMsg.Msg;
                                    this.attachedQuads.UpdateReceivedQuadHeartBeat(heartBeat.quadID);
                                    break;
                                }
                            default:
                                {
                                    break;
                                }
                        }
                    }
                }
            }

            return result;
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
