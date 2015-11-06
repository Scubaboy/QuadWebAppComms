using Ninject;
using QuadComms.Breeze.EntityManagerFactory;
using QuadComms.Breeze.Repositories.ActiveQuadRepository;
using QuadComms.CommControllers;
using QuadComms.CommsChannels;
using QuadComms.CommsDevices.SerialComms;
using QuadComms.Controllers.AttachedQuadsController;
using QuadComms.Controllers.CommsContStatusController;
using QuadComms.Controllers.ProcessControllers;
using QuadComms.Controllers.QuadStatusController;
using QuadComms.CRC32Generator;
using QuadComms.DataPckControllers.DataPckRecvControllers;
using QuadComms.DataPckDecoderControllers.Binary;
using QuadComms.Interfaces.Breeze;
using QuadComms.Interfaces.CommsChannel;
using QuadComms.Interfaces.CommsController;
using QuadComms.Interfaces.CommsDevice;
using QuadComms.Interfaces.Controllers.AttachedQuadsController;
using QuadComms.Interfaces.Controllers.CommsContStatusController;
using QuadComms.Interfaces.Controllers.ProcessController;
using QuadComms.Interfaces.Controllers.QuadStatusController;
using QuadComms.Interfaces.CRCInterface;
using QuadComms.Interfaces.DataDecoder;
using QuadComms.Interfaces.Logging;
using QuadComms.Interfaces.MsgProcessor;
using QuadComms.Interfaces.Queues;
using QuadComms.Interfaces.SignalR;
using QuadComms.Logging;
using QuadComms.Logging.NLog;
using QuadComms.MessageProcessors.Standard;
using QuadComms.Queues.QuadConcurrentQueue;
using QuadComms.SignalR.ClientHubProxies;
using QuadComms.SignalR.HubFactory;
using QuadComms.SignalR.Manager;
using QuadModels;
using System.IO.Ports;

namespace QuadComms.IoC.Ninject
{
    public static class NinjectIoC
    {
        private static IKernel kernel;

        static NinjectIoC()
        {

            BuildKernel();
        }

        public static IKernel Kernel
        {
            get
            {
                return kernel;
            }
        }

        public static void BuildKernel()
        {
            kernel = new StandardKernel();

            kernel.Bind<IProcessCtrl>()
                .To<ProcessCtrl>()
                .InSingletonScope();

            kernel.Bind<IQuadStatusCtrl>()
                .To<QuadStatusCtrl>();

            kernel.Bind<IAttachedQuadsCtrl>()
                .To<AttachedQuadsctrl>()
                .InSingletonScope();
                
            kernel.Bind<ICommsContStatusCtrl>()
                .To<CommsContStatusCtrl>()
                .InSingletonScope();

            kernel
                .Bind<ILogger>()
                .To<TheLogger>()
                .InSingletonScope();

            kernel
                .Bind<ILogConfiguration>()
                .To<NLogConfiguration>()
                .InSingletonScope();

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
                .To<BasicChannel>()
                .InSingletonScope();

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
