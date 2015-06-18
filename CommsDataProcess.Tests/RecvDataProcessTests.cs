using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QuadComms.Interfaces.CommsDevice;
using Moq;
using QuadComms.CommsChannels;

namespace CommsDataProcess.Tests
{
    [TestClass]
    public class RecvDataProcessTests
    {
        [TestMethod]
        public void WhenInSyncModeOnlyRespondsToSyncMsg()
        {
            //Arrange
            var mockCommsDevice = new Mock<ICommsDevice>();
            mockCommsDevice.Setup(x => x.BytesToRead).Returns(3);

            var commsChannel = new BasicChannel(mockCommsDevice.Object);

            //Act

            commsChannel.ProcessCommsChannel();

            //Assert
            mockCommsDevice.Verify(mock => mock.ClearInput(), Times.Once());

        }

        [TestMethod]
        public void WhenReceiveSyncMsgRepondsWithSync()
        {
            //Arrange
            var mockCommsDevice = new Mock<ICommsDevice>();
            mockCommsDevice.Setup(x => x.BytesToRead).Returns(2);
            mockCommsDevice.Setup(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(),It.IsAny<int>()))
                .Callback((byte[] buffer, int offset, int count) =>
                {
                    var synchBytes = System.Text.Encoding.ASCII.GetBytes("##");
                    buffer[0] = synchBytes[0];
                    buffer[1] = synchBytes[1];
                });

            var commsChannel = new BasicChannel(mockCommsDevice.Object);

            //Act
            commsChannel.ProcessCommsChannel();

            //Assert
            mockCommsDevice.Verify(mock => mock.Write(It.Is<byte[]>(data => data[0] == '#' && data[1] == '#' && data.Length == 2), It.Is<int>(i => i == 0), It.Is<int>(count => count ==2)), Times.Once());
        }

        [TestMethod]
        public void WhenSynchedProcessesMessagesOfExpectedLength200Bytes()
        {
            //Arrange
            var hasBeenCalled = false;
            var mockCommsDevice = new Mock<ICommsDevice>();
            mockCommsDevice.SetupSequence(x => x.BytesToRead)
                .Returns(2)
                .Returns(200);
            mockCommsDevice.Setup(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback((byte[] buffer, int offset, int count) =>
                {
                    if (!hasBeenCalled)
                    {
                        var synchBytes = System.Text.Encoding.ASCII.GetBytes("##");
                        buffer[0] = synchBytes[0];
                        buffer[1] = synchBytes[1];
                    }
                });

            var commsChannel = new BasicChannel(mockCommsDevice.Object);

            //Act
            commsChannel.ProcessCommsChannel();
            commsChannel.ProcessCommsChannel();

            //Assert
            mockCommsDevice.VerifyGet(mock => mock.BytesToRead, Times.Exactly(2));
        }
        [TestMethod]
        public void WhenSynchedProcessesMessagesOfExpectedLength200BytesWithStartEndMarkersCopiesToRecvQueue()
        {
            //Arrange
            var hasBeenCalled = false;
            var mockCommsDevice = new Mock<ICommsDevice>();
            mockCommsDevice.SetupSequence(x => x.BytesToRead)
                .Returns(2)
                .Returns(200);
            mockCommsDevice.Setup(x => x.Read(It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
                .Callback((byte[] buffer, int offset, int count) =>
                {
                    if (!hasBeenCalled)
                    {
                        var synchBytes = System.Text.Encoding.ASCII.GetBytes("##");
                        buffer[0] = synchBytes[0];
                        buffer[1] = synchBytes[1];
                        hasBeenCalled = true;
                    }
                    else
                    {
                        buffer[0] = 60;
                        buffer[1] = 60;
                        buffer[198] = 62;
                        buffer[199] = 62;
                    }
                });

            var commsChannel = new BasicChannel(mockCommsDevice.Object);

            //Act
            commsChannel.ProcessCommsChannel();
            commsChannel.ProcessCommsChannel();

            //Assert
            Assert.AreEqual(commsChannel.DataPcksAvailable(), true);
        }
    }
}
