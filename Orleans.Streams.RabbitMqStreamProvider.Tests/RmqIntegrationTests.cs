﻿using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Orleans.TestingHost;
using static RabbitMqStreamTests.ToxiProxyHelpers;
using static RabbitMqStreamTests.TestClusterUtils;

namespace RabbitMqStreamTests
{
    //[Ignore]
    [TestClass]
    public class RmqIntegrationTests
    {
        [TestMethod]
        public async Task TestConcurrentProcessingWithPrefilledQueue()
        {
            await _cluster.TestRmqStreamProviderWithPrefilledQueue(
                conn => { },
                conn => { },
                1000, 10);
        }

        [TestMethod]
        public async Task TestConcurrentProcessingOnFly()
        {
            await _cluster.TestRmqStreamProviderOnFly(
                conn => { },
                1000, 10);
        }
        
        #region Test class setup

        private static TestCluster _cluster;
        private static Process _proxyProcess;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // TODO: ensure empty RMQ

            // ToxiProxy
            _proxyProcess = StartProxy();

            // Orleans cluster
            _cluster = CreateClusterOptions().CreateTestCluster();
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            // close first to avoid a case where Silo hangs, I stop the test and the proxy process keeps running
            _proxyProcess.CloseMainWindow();
            _proxyProcess.WaitForExit();

            _cluster.Shutdown();
        }

        #endregion
    }
}
