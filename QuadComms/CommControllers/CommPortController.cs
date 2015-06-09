using MbedQuad;
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
using QuadComms.Interfaces.DataDecoder;
using QuadComms.Interfaces.Queues;
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
    enum Mode
    {
        Synching, Syched
    };

    internal class CommPortController : ICommsChannel
    {
        private const string SynchString = "##";

        private const int RetryThreshold = 10;
        private const int SendRecvTaskSleep =   1;
        private const int ProgressUpdatePeriod = 5;
        private const int SendPeriod = 3000;
        private const int TicksBetweenProgreeUpdates = ProgressUpdatePeriod/SendRecvTaskSleep;
        private const int TicksBetweenSends = SendPeriod/SendRecvTaskSleep;
        private const string StartMarker = "<<";
        private const string EndMarker = ">>";
        private Mode commsSynchMode = Mode.Synching;
        private SerialPort serialPort = null;
        private CommPortConfig commPortConfig;
        private TransmissionAction transAction;
        private byte[] receiveBufBuildingPck;
        private IPostQueueMsg dataPckSent;
        private IDataDecoder dataPckDecoder;
        private List<byte> rawDataPack;
        ConcurrentQueue<byte[]> recvQueue;
        ConcurrentQueue<IPostQueueMsg> postQueue;
        private int pckRecvTimer;
        private int sendTicks;
        private int failedSendsLastProgress;



        public CommPortController(IDataDecoder dataPckDecoder, CommPortConfig commPortConfig, ConcurrentQueue<byte[]> recvQueue, ConcurrentQueue<IPostQueueMsg> postQueue)
        {
            this.dataPckDecoder = dataPckDecoder;
            this.commPortConfig = commPortConfig;
            this.recvQueue = recvQueue;
            this.postQueue = postQueue;
            this.pckRecvTimer = 0;
        }

        public void Setup()
        {
            this.receiveBufBuildingPck = new byte[DataPckTypes.DataPckSendRecvSize];
            this.transAction = TransmissionAction.WaitingDataPckSend;
            this.sendTicks = 0;
            this.failedSendsLastProgress = 0;
            this.rawDataPack  = new List<byte>();
            this.serialPort = new SerialPort(this.commPortConfig.PortName, this.commPortConfig.Baud, this.commPortConfig.Parity, this.commPortConfig.DataBits, this.commPortConfig.Stopbits);
            this.serialPort.Close();
            this.serialPort.Handshake = this.commPortConfig.Handshake;
            this.serialPort.ReadBufferSize = 4096;
            this.serialPort.WriteBufferSize = 1024;
            this.serialPort.Open();
            this.serialPort.DiscardInBuffer();
            this.serialPort.DiscardOutBuffer();
            
        }

        private void serialPort_DataReceived()
        {
            switch (this.commsSynchMode)
            {
                case Mode.Synching:
                    {
                        if (this.serialPort.BytesToRead == 2)
                        {
                            var synchData = new byte[2];
                            this.serialPort.Read(synchData, 0, 2);
                            this.ProcessSynch(synchData);
                            this.serialPort.DiscardInBuffer();
                        }
                        else if (this.serialPort.BytesToRead > 2)
                        {
                            try
                            {
                                this.serialPort.DiscardInBuffer();
                            }
                            catch (Exception)
                            {


                            }
                        }

                        break;
                    }
                case Mode.Syched:
                    {
                        if (this.pckRecvTimer > 300)
                        {
                            Debug.WriteLine("timeout {0}", this.pckRecvTimer);
                            this.serialPort.DiscardInBuffer();
                        }

                        if (this.serialPort.BytesToRead == 200)
                        {
                            var rawDataRcv = new byte[200];

                            this.serialPort.Read(rawDataRcv, 0, rawDataRcv.Length);

                            if (rawDataRcv[0] == 60 && rawDataRcv[1] == 60 && rawDataRcv[198] == 62 && rawDataRcv[199] == 62)
                            {
                                Task.Factory.StartNew(() =>
                                {
                                    this.recvQueue.Enqueue(rawDataRcv);
                                    Debug.WriteLine("Recv msg tyep {0}", BitConverter.ToUInt32(rawDataRcv, 6));
                                    Debug.WriteLine("timeout {0}", this.pckRecvTimer);
                                });
                            }
                            else
                            {
                                try
                                {
                                    this.serialPort.DiscardInBuffer();
                                }
                                catch (Exception)
                                {


                                }

                            }
                        }
                        else if (this.serialPort.BytesToRead == 0)
                        {
                            this.pckRecvTimer = 0;
                        }
                        else if (this.serialPort.BytesToRead > 200)
                        {
                            try
                            {
                                var rawDataRcv = new byte[this.serialPort.BytesToRead];

                                this.serialPort.Read(rawDataRcv, 0, rawDataRcv.Length);
                                this.serialPort.DiscardInBuffer();
                            }
                            catch (Exception)
                            {


                            }

                            Debug.WriteLine("Lost buffer timing");
                        }
                        else
                        {
                            this.pckRecvTimer++;
                        }
                        break;
                    }
            }
        }

        private void ProcessSynch(byte[] synchData)
        {
            var synch = Encoding.ASCII.GetString(synchData);

            if (synch == SynchString)
            {
                var sendSycnhAck = System.Text.Encoding.ASCII.GetBytes(SynchString);

                this.serialPort.DiscardOutBuffer();
                this.serialPort.Write(sendSycnhAck,0,2);

                this.commsSynchMode = Mode.Syched;
            }
        }

        public Task Start(CancellationToken cancellationToken)
        {
            return Task.Run(() =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        this.serialPort_DataReceived();

                        switch (commsSynchMode)
                        {
                            case Mode.Syched:
                                {
                                    this.TransmitAction();
                                    break;
                                }
                        }
                      
                        Thread.Sleep(SendRecvTaskSleep);
                    }

                    this.serialPort.Close();
                });
        }

        /*private void ReceiveDataPckAction()
        {
            if (this.dataPckReceivedQueue.Any())
            {
                List<byte> dataPckRecvRaw;

                if (this.dataPckReceivedQueue.TryDequeue(out dataPckRecvRaw))
                {
                    DecodedDataPck decodedDataPck;
                    this.DataPckDecoder.Decode(dataPckRecvRaw.ToArray(), out decodedDataPck);
                    this.BuildDataPck(decodedDataPck);
                }
            }
        }*/

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
                                if (this.postQueue.TryDequeue(out dataPckSent))
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

            }
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
        /*
        private void SetLastMsgReceived(DataPck dataPcks)
        {
           if (dataPcks.Type != DataPckTypes.DataPcks.SendConf & dataPcks.Type != DataPckTypes.DataPcks.FreeTxtMsg)
           {
               this.lastMsgReceived = dataPcks.Type;
           }
        }
        */
        private void SendDataPck(byte[] dataPckToSend)
        {
            var blocksSent = 0;
            var blockOffset = 0;

            Debug.WriteLine("Send msg type {0}", BitConverter.ToUInt32(dataPckToSend, 4));

            this.serialPort.DiscardOutBuffer();
            
            while (blocksSent < 200/10)
            {  
                this.serialPort.Write(dataPckToSend, blockOffset,10);
                blockOffset += 10;
                blocksSent++;
                Thread.Sleep(1);
            } 
        }

       /*
        public IDataDecoder DataPckDecoder
        {
            get
            {
                if (this.dataPckDecoder == null)
                {
                    throw new ArgumentNullException();
                }

                return this.dataPckDecoder;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException();
                }

                this.dataPckDecoder = value;
            }
        }


        public SystemModes SysMode
        {
            set
            {
               this.mode = value;
            }
        }*/
    }
}
