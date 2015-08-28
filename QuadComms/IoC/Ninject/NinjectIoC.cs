using Ninject;
using QuadComms.Breeze.EntityManagerFactory;
using QuadComms.Breeze.Repositories.ActiveQuadRepository;
using QuadComms.CommControllers;
using QuadComms.CommsChannels;
using QuadComms.CommsDevices.SerialComms;
using QuadComms.CRC32Generator;
using QuadComms.DataPckControllers.DataPckRecvControllers;
using QuadComms.DataPckDecoderControllers.Binary;
using QuadComms.Interfaces.Breeze;
using QuadComms.Interfaces.CommsChannel;
using QuadComms.Interfaces.CommsController;
using QuadComms.Interfaces.CommsDevice;
using QuadComms.Interfaces.CRCInterface;
using QuadComms.Interfaces.DataDecoder;
using QuadComms.Interfaces.MsgProcessor;
using QuadComms.Interfaces.Queues;
using QuadComms.Interfaces.SignalR;
using QuadComms.MessageProcessors.Standard;
using QuadComms.Queues.QuadConcurrentQueue;
using QuadComms.SignalR.ClientHubProxies;
using QuadComms.SignalR.HubFactory;
using QuadComms.SignalR.Manager;
using QuadModels;
using System.IO.Ports;

namespace QuadComms.IoC.Ninject
{
    public class NinjectIoC
    {
        private  IKernel kernel;

        public NinjectIoC()
        {

            BuildKernel();
        }

        public IKernel Kernel
        {
            get
            {
                return kernel;
            }
        }

        public void BuildKernel()
        {
            kernel = new StandardKernel();

            kernel
                .Bind<IDataTransferQueue<IQuadRecvMsgQueue>>()
                .To<QuadQueue<IQuadRecvMsgQueue>>()
                .InSingletonScope()
                .Named("QuadRecvQueue");

            kernel
                .Bind<IDataTransferQueue<ISignalRRecvQueueMsg>>()
                .To<QuadQueue<ISignalRRecvQueueMsg>>()
                .InSingletonScope()
                .Named("SigRRecvQueue");

            kernel
                .Bind<IDataTransferQueue<ISigRPostQueueMsg<DataPckRecvController>>>()
                .To<QuadQueue<ISigRPostQueueMsg<DataPckRecvController>>>()
                .InSingletonScope()
                .Named("SigRTransQueue");

            kernel
                .Bind<IDataTransferQueue<IQuadTransQueueMsg>>()
                .To<QuadQueue<IQuadTransQueueMsg>>()
                .InSingletonScope()
                .Named("QuadTransQueue");

            kernel
                .Bind<IDataDecoder>()
                .To<BinaryDecoder>();

            kernel
                .Bind<ICommsController>()
                .To<CommController>();

            kernel
                .Bind<ICommsChannel>()
                .To<BasicChannel>();

            kernel
                .Bind<ICRC>()
                .To<CRC32>();

            kernel
                .Bind<ICommsDevice>()
                .To<Serial>()
                .WithConstructorArgument<CommPortConfig>(
                new CommPortConfig(
                    "com5",
                    9600,
                    Parity.None,
                    StopBits.One,
                    Handshake.None,
                    8));

            kernel
                .Bind<IMsgProcessor>()
                .To<StdMsgProcessor>();

            kernel
                .Bind<IBreezeRepository<ActiveQuad>>()
                .To<ActiveQuadRepos>()
                .WithConstructorArgument("serviceName", "http://localhost:64297/breeze/");

            kernel
                .Bind<IBreezeEntityManagerFactory>()
                .To<EntityMgrFact>();

            kernel
                .Bind<ISignalRClientProxyMgr>()
                .To<QuadSigRMgr>();
            kernel
                .Bind<IHubProxyFactory>()
                .To<HubProxyFactory>()
                .InSingletonScope();
            kernel
                .Bind<BaseMsgHubClientProxy>()
                .To<MsgHubClientProxy>()
                .WithConstructorArgument("hubUrl", "http://localhost:8989");
        }
    }
}
