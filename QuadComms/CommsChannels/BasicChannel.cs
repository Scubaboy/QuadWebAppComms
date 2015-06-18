using QuadComms.Interfaces.CommsChannel;
using QuadComms.Interfaces.CommsDevice;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QuadComms.CommsChannels
{
    enum Mode
    {
        Synching, Syched
    };

    public class BasicChannel : ICommsChannel
    {
        private ICommsDevice commsDevice;
        private Mode commsSynchMode = Mode.Synching;
        private const string SynchString = "##";
        private int pckRecvTimer;
        private ConcurrentQueue<byte[]> dataPckReceivedQueue = new ConcurrentQueue<byte[]>();
        private ConcurrentQueue<byte[]> dataPckSendQueue = new ConcurrentQueue<byte[]>();

        public BasicChannel(ICommsDevice commsDevice)
        {
            this.commsDevice = commsDevice;
        }

        public void ProcessCommsChannel()
        {
            //Process Receives
            switch (this.commsSynchMode)
            {
                case Mode.Synching:
                    {
                        if (this.commsDevice.BytesToRead == 2)
                        {
                            var synchData = new byte[2];
                            this.commsDevice.Read(synchData, 0, 2);
                            this.ProcessSynch(synchData);
                            this.commsDevice.ClearInput();
                        }
                        else if (this.commsDevice.BytesToRead > 2)
                        {
                            try
                            {
                                this.commsDevice.ClearInput();
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
                            this.commsDevice.ClearInput();
                        }

                        if (this.commsDevice.BytesToRead == 200)
                        {
                            var rawDataRcv = new byte[200];

                            this.commsDevice.Read(rawDataRcv, 0, rawDataRcv.Length);

                            if (rawDataRcv[0] == 60 && rawDataRcv[1] == 60 && rawDataRcv[198] == 62 && rawDataRcv[199] == 62)
                            {
                              //  Task.Factory.StartNew(() =>
                               // {
                                    this.dataPckReceivedQueue.Enqueue(rawDataRcv);
                                    Debug.WriteLine("Recv msg tyep {0}", BitConverter.ToUInt32(rawDataRcv, 6));
                                    Debug.WriteLine("timeout {0}", this.pckRecvTimer);
                               // });
                            }
                            else
                            {
                                try
                                {
                                    this.commsDevice.ClearInput();
                                }
                                catch (Exception)
                                {


                                }

                            }
                        }
                        else if (this.commsDevice.BytesToRead == 0)
                        {
                            this.pckRecvTimer = 0;
                        }
                        else if (this.commsDevice.BytesToRead > 200)
                        {
                            try
                            {
                                var rawDataRcv = new byte[this.commsDevice.BytesToRead];

                                this.commsDevice.Read(rawDataRcv, 0, rawDataRcv.Length);
                                this.commsDevice.ClearInput();
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

            //Process Sends
            if (this.dataPckSendQueue.Any())
            {
                byte[] sendData;
                var blocksSent = 0;
                var blockOffset = 0;

                if (this.dataPckReceivedQueue.TryDequeue(out sendData))
                {
                     this.commsDevice.ClearOutput();

                     while (blocksSent < 200 / 10)
                     {
                         this.commsDevice.Write(sendData, blockOffset, 10);
                         blockOffset += 10;
                         blocksSent++;
                         Thread.Sleep(1);
                     }
                }
            }
            
        }

        private void ProcessSynch(byte[] synchData)
        {
            var synch = Encoding.ASCII.GetString(synchData);

            if (synch == SynchString)
            {
                var sendSycnhAck = System.Text.Encoding.ASCII.GetBytes(SynchString);

                this.commsDevice.ClearOutput();
                this.commsDevice.Write(sendSycnhAck, 0, 2);

                this.commsSynchMode = Mode.Syched;
            }
        }


        public bool DataPcksAvailable()
        {
            return this.dataPckReceivedQueue.Any();
        }

        public bool TakeDataPck(out byte[] rawDataPck)
        {
            return this.dataPckReceivedQueue.TryDequeue(out rawDataPck);
        }


        public void QueueDataPck(byte[] dataPck)
        {
            this.dataPckSendQueue.Enqueue(dataPck);
        }


        public void Close()
        {
            this.commsDevice.Close();
        }

        public void ClearInput()
        {
            this.commsDevice.ClearInput();
            this.commsDevice.ClearOutput();
        }
    }
}
