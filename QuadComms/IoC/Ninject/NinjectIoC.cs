using Ninject;
using QuadComms.Breeze.EntityManagerFactory;
using QuadComms.Breeze.Repositories.ActiveQuadRepository;
using QuadComms.CommControllers;
using QuadComms.CommsChannels;
using QuadComms.CommsDevices.SerialComms;
using QuadComms.DataPckControllers.DataPckRecvControllers;
using QuadComms.DataPckDecoderControllers.Binary;
using QuadComms.Interfaces.Breeze;
using QuadComms.Interfaces.CommsChannel;
using QuadComms.Interfaces.CommsController;
using QuadComms.Interfaces.CommsDevice;
using QuadComms.Interfaces.DataDecoder;
using QuadComms.Interfaces.MsgProcessor;
using QuadComms.Interfaces.Queues;
using QuadComms.MessageProcessors.Standard;
using QuadComms.Queues.QuadConcurrentQueue;
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
                .Named("QuadRecvQueue");

            kernel
                .Bind<IDataTransferQueue<ISignalRRecvQueueMsg>>()
                .To<QuadQueue<ISignalRRecvQueueMsg>>()
                .Named("SigRRecvQueue");

            kernel
                .Bind<IDataTransferQueue<ISigRPostQueueMsg<DataPckRecvController>>>()
                .To<QuadQueue<ISigRPostQueueMsg<DataPckRecvController>>>()
                .Named("SigRTransQueue");

            kernel
                .Bind<IDataTransferQueue<IQuadTransQueueMsg>>()
                .To<QuadQueue<IQuadTransQueueMsg>>()
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
                .WithConstructorArgument("serviceName", "ActiveQuads");

            kernel
                .Bind<IBreezeEntityManagerFactory>()
                .To<EntityMgrFact>();

        }
    }
}
