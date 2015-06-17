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
    }
}
