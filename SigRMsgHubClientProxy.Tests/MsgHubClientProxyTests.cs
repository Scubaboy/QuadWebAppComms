using Microsoft.AspNet.SignalR.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using QuadComms.DataPckControllers.DataPckRecvControllers.FlightDataDataPckController;
using QuadComms.DataPckControllers.DataPckRecvControllers.MsgDataPckController;
using QuadComms.DataPcks.FlightDataPck;
using QuadComms.DataPcks.MsgDataPck;
using QuadComms.Interfaces.SignalR;
using QuadComms.SignalR.ClientHubProxies;
using QuadSignalRMsgs.HubResponces;
using System.Threading.Tasks;

namespace SigRMsgHubClientProxy.Tests
{
    [TestClass]
    public class MsgHubClientProxyTests
    {
        [TestMethod]
        public async Task OnlyPostSupportedMsgDataPckControllerReturnsResponceTrue()
        {
            var hubProxyFactoryMock = new Mock<IHubProxyFactory>();
            var iHubProxyMock = new Mock<IHubProxy>();
            var tcs = new TaskCompletionSource<IHubProxy>();
            tcs.SetResult(iHubProxyMock.Object);
            var tcsProxy = new TaskCompletionSource<ReceiveResponce>();
            tcsProxy.SetResult(new ReceiveResponce(true));

            iHubProxyMock.Setup(set => set.Invoke<ReceiveResponce>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(tcsProxy.Task);
     
            hubProxyFactoryMock.Setup(set => set.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(tcs.Task);

            var msgHubClient = new MsgHubClientProxy(hubProxyFactoryMock.Object,"test");

            await msgHubClient.StartClientProxy().ConfigureAwait(false);

            var msgdata = new MsgData();
            var msg = new MsgDataPckController(msgdata);
            var postResult = await msgHubClient.Post<MsgDataPckController>(msg).ConfigureAwait(false);

            Assert.IsTrue(postResult.MsgProcessed);
        }

        [TestMethod]
        public async Task UnSupportedMsgTypeReturnsResponceFalse()
        {
            var hubProxyFactoryMock = new Mock<IHubProxyFactory>();
            var iHubProxyMock = new Mock<IHubProxy>();
            var tcs = new TaskCompletionSource<IHubProxy>();
            tcs.SetResult(iHubProxyMock.Object);
            var tcsProxy = new TaskCompletionSource<ReceiveResponce>();
            tcsProxy.SetResult(new ReceiveResponce(true));

            iHubProxyMock.Setup(set => set.Invoke<ReceiveResponce>(It.IsAny<string>(), It.IsAny<object[]>())).Returns(tcsProxy.Task);

            hubProxyFactoryMock.Setup(set => set.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(tcs.Task);

            var msgHubClient = new MsgHubClientProxy(hubProxyFactoryMock.Object, "test");

            await msgHubClient.StartClientProxy().ConfigureAwait(false);

            var msgdata = new FlightData();
            var msg = new FlightDataDataPckController(msgdata);
            var postResult = await msgHubClient.Post<FlightDataDataPckController>(msg).ConfigureAwait(false);

            Assert.IsFalse(postResult.MsgProcessed);
        }
    }
}
