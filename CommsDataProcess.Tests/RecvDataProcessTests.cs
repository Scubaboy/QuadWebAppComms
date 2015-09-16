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
            mockCommsDevice.VerifyGet(mock => mock.BytesToRead, Times.Exactly(1));
        }
        [TestMethod]
        public void WhenSynchedProcessesMessagesOfExpectedLength200BytesWithStartEndMarkersCopiesToRecvQueue()
        {
            //Arrange
            var hasBeenCalled = false;
            var commsIter = 0;
            var mockedMessage = new byte[200];
            var mockCommsDevice = new Mock<ICommsDevice>();

            mockedMessage[0] = (byte)'<';
            mockedMessage[1] = (byte)'<';
            mockedMessage[198] = (byte)'>';
            mockedMessage[199] = (byte)'>';

            mockCommsDevice.SetupSequence(x => x.BytesToRead)
                .Returns(2);

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
                });

            mockCommsDevice.Setup(x => x.ReadByte()).Returns(() =>
            {
                return mockedMessage[commsIter];
            });

            var commsChannel = new BasicChannel(mockCommsDevice.Object);

            //Act
            commsChannel.ProcessCommsChannel();
            while (commsIter < 200)
            {
                commsChannel.ProcessCommsChannel();
                commsIter++;
            }

            //Assert
            Assert.AreEqual(commsChannel.DataPcksAvailable(), true);
        }
    }
}
