using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MbedQuad;
using QuadComms.CRC32Generator;
using QuadComms.CommsProgress;
using QuadComms.CommsProgress.Status;
using QuadComms.DataPckControllers.DataPckRecvControllers;
using QuadComms.DataPckControllers.DataPckRecvControllers.DataLoggerDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.DataRequestDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.FlightDataDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.MsgDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.SystemIdDataPckController;
using QuadComms.DataPckControllers.DataPckTransControllers.ReSendDataPckController;
using QuadComms.DataPckDecoderControllers.DecoderTypes;
using QuadComms.DataPckStructs;
using QuadComms.DataPcks;
using QuadComms.DataPcks.DataLoggerDataPck;
using QuadComms.DataPcks.DataRequestDataPck;
using QuadComms.DataPcks.FlightDataPck;
using QuadComms.DataPcks.MsgDataPck;
using QuadComms.DataPcks.SendConfPck;
using QuadComms.DataPcks.SystemId;
using QuadComms.Interfaces.CommsChannel;
using QuadComms.Interfaces.DataDecoder;


namespace QuadComms.CommControllers
{
    enum Mode
    {
        Synching, Syched
    };

    internal class CommPortController : ICommsChannel
    {
        private const string SynchString = "##";
        private int pckCount = 0;
        private const int RetryThreshold = 10;
        private const int SendRecvTaskSleep =   1;
        private const int ProgressUpdatePeriod = 5;
        private const int SendPeriod = 3000;
        private const int TicksBetweenProgreeUpdates = ProgressUpdatePeriod/SendRecvTaskSleep;
        private const int TicksBetweenSends = SendPeriod/SendRecvTaskSleep;
        private const string StartMarker = "<<";
        private const string EndMarker = ">>";
        private bool ignoreNextDataPck = false;
        private Mode commsSynchMode = Mode.Synching;

        private SerialPort serialPort = null;
        private string portName;
        private int baudRate;
        private int dataBits;
        private Handshake handshake;
        private Parity parity;
        private StopBits stopBits;
        private ConcurrentQueue<byte[]> dataPckSendQueue;
        private TransmissionAction transAction;
        private ReceivedAction receiveAction;
        private int bytesReceived;
        private ConcurrentQueue<List<byte>> dataPckReceivedQueue;
        private ConcurrentQueue<byte[]> rawDataPckQueue; 
        private byte[] receiveBufBuildingPck;
        private byte[] dataPckSent;
        private List<byte> rawDataPack;
        private bool gotSendConf;
        private bool gotfirstPck;
        private int pckRecvTimer;
        private DataPckTypes.DataPcks lastMsgReceived;
        private SendConf sendConf;
        private IDataDecoder dataPckDecoder;
        private int retryCount;
        private int progressTicks;
        private int sendTicks;
        private int failedSendsLastProgress;
        private int failedRecvLastProgress;
        private SystemModes mode;
        private byte[] rawData;
        private readonly List<SystemModes> ReSendModes = new List<SystemModes>()
            {
                SystemModes.ConfigCal,
                SystemModes.ArmMotors
            }; 

        public CommPortController(
            string portname,
            int baud,
            Parity parity,
            StopBits stopBits,
            Handshake handshake,
            int dataBit)
        {
            this.portName = portname;
            this.baudRate = baud;
            this.dataBits = dataBit;
            this.parity = parity;
            this.stopBits = stopBits;
            this.handshake = handshake;
            this.gotSendConf = false;
            this.lastMsgReceived = DataPckTypes.DataPcks.NoMsg;
            this.gotfirstPck = false;
            this.pckRecvTimer = 0;
        }

        public void AppendData(byte[] data)
        {
            if (this.dataPckSendQueue == null)
            {
                throw new ArgumentNullException();
            }

            this.dataPckSendQueue.Enqueue(data);
        }

        public void Setup()
        {
            this.dataPckSendQueue = new ConcurrentQueue<byte[]>();
            this.dataPckReceivedQueue = new ConcurrentQueue<List<byte>>();
            this.receiveBufBuildingPck = new byte[DataPckTypes.DataPckSendRecvSize];
            this.rawDataPckQueue = new ConcurrentQueue<byte[]>();
            this.rawData = null;
            this.receiveAction = ReceivedAction.Waiting;
            this.transAction = TransmissionAction.WaitingDataPckSend;
            this.bytesReceived = 0;
            this.retryCount = 0;
            this.progressTicks = 0;
            this.sendTicks = 0;
            this.failedRecvLastProgress = 0;
            this.failedSendsLastProgress = 0;
            this.rawDataPack  = new List<byte>();
            this.serialPort = new SerialPort(this.portName,this.baudRate,this.parity,this.dataBits,this.stopBits);
            this.serialPort.Close();
            this.serialPort.Handshake = this.handshake;
            this.serialPort.ReadBufferSize = 4096;
            this.serialPort.WriteBufferSize = 1024;
            this.serialPort.Open();
            //this.serialPort.DataReceived += serialPort_DataReceived;
            this.serialPort.DiscardInBuffer();
            this.serialPort.DiscardOutBuffer();
            
        }

        private void serialPort_DataReceived()//object sender, SerialDataReceivedEventArgs e)
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
                                    Debug.WriteLine("timeout {0}",this.pckRecvTimer);
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
                                            this.dataPckReceivedQueue.Enqueue(rawDataRcv.ToList());
                                            Debug.WriteLine("Recv msg tyep {0}", BitConverter.ToUInt32(rawDataRcv, 6));
                                            Debug.WriteLine("timeout {0}", this.pckRecvTimer);
                                        });

                                        //this.serialPort.DiscardInBuffer();
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
                           // }
                      //  }
                         
                     
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

        public Task ReadSerial(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(() =>
                {
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        this.serialPort_DataReceived();
                    }
                });
        }
        public Task ProcessCommsAsync(CancellationToken cancellationToken, ConcurrentQueue<Progress> progress)
        {
            return Task.Run(() =>
                {
                    var recvedDataPcks = new List<DataPckRecvController>();

                    while (!cancellationToken.IsCancellationRequested)
                    {
                        this.serialPort_DataReceived();

                        switch (commsSynchMode)
                        {
                            case Mode.Synching:
                                {

                                    break;
                                }
                                case Mode.Syched:
                                {
                                   // this.ReceiveRawDataAction();
                                    this.ReceiveDataPckAction(ref recvedDataPcks);
                                    this.TransmitAction();
                                    this.ProgressReportAction(progress, ref recvedDataPcks, this.gotSendConf);
                                    this.gotSendConf = false;
                                    break;
                                }
                        }
                      
                        Thread.Sleep(SendRecvTaskSleep);
                    }

                    this.serialPort.Close();
                });
        }

        private void ProgressReportAction(ConcurrentQueue<Progress> progress, ref  List<DataPckRecvController> recvedDataPcks,bool msgSendConf)
        {
            progress.Enqueue(
                new Progress(
                    recvedDataPcks,
                    new SendReceiveStatus(
                        this.failedSendsLastProgress,
                        this.failedRecvLastProgress), msgSendConf));

                recvedDataPcks = new List<DataPckRecvController>();
                this.progressTicks = 0;
        }

        /*private void ReceiveRawDataAction()
        {
            //Process any remaing data from last run
            if (this.rawData != null)
            {
                this.CopyRawdata(ref rawData, ref this.rawDataPack);
            }

            //If new data in the queue extract a packet and start decoding.
            if (this.rawDataPckQueue.Any())
            {
                if (this.rawDataPckQueue.TryDequeue(out rawData))
                {
                    this.CopyRawdata(ref rawData, ref this.rawDataPack);
                }
            }
        }

        private void CopyRawdata(ref byte[] rawInputData,ref List<byte> rawDataPck)
        {
            if (!rawDataPck.Any())
            {
                var startMarkerIndex = this.FindIndexOfMarker(rawInputData, StartMarker, 0);
                var endMarkerIndex = this.FindIndexOfMarker(rawInputData, EndMarker, startMarkerIndex > -1 ? startMarkerIndex + 2 : 0);

                if (startMarkerIndex != -1 && (endMarkerIndex != -1 && endMarkerIndex > startMarkerIndex))
                {
                    for (var iter = startMarkerIndex; iter <= endMarkerIndex+1; iter++)
                    {
                        rawDataPck.Add(rawInputData[iter]);
                    }

                    if (rawDataPck.Count == 200)
                    {
                        this.dataPckReceivedQueue.Enqueue(rawDataPck);
                    }

                    rawDataPck = new List<byte>();

                    var remainingData = rawInputData.Length - 200;

                    if (remainingData > 0)
                    {
                        var rawInputDataTemp = new byte[remainingData];
                        for (var iter = 200; iter < rawInputData.Length; iter++)
                        {
                            rawInputDataTemp[iter - 200] = rawInputData[iter];
                        }

                        rawInputData = rawInputDataTemp;
                    }
                    else
                    {
                        rawInputData = null;
                    }
                }
                else if (startMarkerIndex != -1 && endMarkerIndex == -1)
                {
                    for (var iter = startMarkerIndex; iter < rawInputData.Length; iter++)
                    {
                        rawDataPck.Add(rawInputData[iter]);
                    }

                    rawInputData = null;
                }
                else
                {
                    rawInputData = null;
                }
            }
            else
            {
                var rawDataPckToString = System.Text.Encoding.ASCII.GetString(rawDataPck.ToArray());

                if (rawDataPckToString.Contains(StartMarker))
                {
                    var endMarkerIndex = this.FindIndexOfMarker(rawInputData,  EndMarker,0);

                    if (endMarkerIndex != -1)
                    {
                        for (var iter = 0; iter <= endMarkerIndex + 1; iter++)
                        {
                            rawDataPck.Add(rawInputData[iter]);
                        }

                        if (rawDataPck.Count == 200)
                        {
                            this.dataPckReceivedQueue.Enqueue(rawDataPck);
                        }

                        rawDataPck = new List<byte>();

                        var remainingData = rawInputData.Length - (endMarkerIndex + 1);// -2 - endMarkerIndex;

                        if (remainingData > 0)
                        {
                            var rawInputDataTemp = new byte[remainingData];

                            for (var iter = endMarkerIndex + 1; iter <= remainingData; iter++)
                            {
                                rawInputDataTemp[iter - (endMarkerIndex + 1)] = rawInputData[iter];
                            }

                            rawInputData = rawInputDataTemp;
                        }
                        else
                        {
                            rawInputData = null;
                        }
                    }
                    else
                    {
                        rawDataPck.AddRange(rawInputData);
                        rawInputData = null;
                    }
                }
                else
                {
                    rawDataPck.Clear();
                    rawInputData = null;
                }
            }
        }
        
        private int FindIndexOfMarker(byte[] rawData, string marker, int offset)
        {
            var rawDataAsString = Encoding.ASCII.GetString(rawData);

            return rawDataAsString.IndexOf(marker,offset);
        }
        */

        private void ReceiveDataPckAction(ref List<DataPckRecvController> recvedDataPcks)
        {
            if (this.dataPckReceivedQueue.Any())
            {
                List<byte> dataPckRecvRaw;

                if (this.dataPckReceivedQueue.TryDequeue(out dataPckRecvRaw))
                {
                    DecodedDataPck decodedDataPck;
                    this.DataPckDecoder.Decode(dataPckRecvRaw.ToArray(), out decodedDataPck);
                    this.BuildDataPck(ref recvedDataPcks, decodedDataPck);
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
                            this.SendDataPck(dataPckSent);
                            break;
                        }
                    case TransmissionAction.WaitingDataPckSend:
                        {
                            if (this.dataPckSendQueue.Any())
                            {
                                if (this.dataPckSendQueue.TryDequeue(out dataPckSent))
                                {
                                    this.transAction = TransmissionAction.WaitingAck;
                                    this.SendDataPck(dataPckSent);  
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

        private void BuildDataPck(ref List<DataPckRecvController> recvedDataPcks, DecodedDataPck dataPck)
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
                    this.ignoreNextDataPck = true;
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
                                this.gotSendConf = true;
                               // this.lastMsgReceived = DataPckTypes.DataPcks.NoMsg;
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
        }

        private void SetLastMsgReceived(DataPck dataPcks)
        {
           if (dataPcks.Type != DataPckTypes.DataPcks.SendConf & dataPcks.Type != DataPckTypes.DataPcks.FreeTxtMsg)
           {
               this.lastMsgReceived = dataPcks.Type;
           }
        }

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
        }
    }
}
