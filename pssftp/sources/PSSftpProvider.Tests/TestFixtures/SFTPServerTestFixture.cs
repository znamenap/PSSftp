using System;
using System.ServiceProcess;
using NUnit.Framework;

namespace PSSftpProvider.Tests.TestFixtures
{
    internal class SFtpServerTestFixture : IServerTestFixture
    {
        private const string BitviseServiceName = "BvSshServer-DEV";
        private readonly ServiceControllerStatus demandingStatus;
        private readonly long maxWaitTimeMs;
        private readonly ServiceControllerStatus previousStatus;
        private readonly ServiceController serviceController;

        public string Host { get; }
        public ushort Port { get; }
        public Uri Uri { get; }
        public string DataRootPath { get; }

        public SFtpServerTestFixture(bool shouldBeRunning, long maxWaitTimeMs = 3000)
        {
            this.maxWaitTimeMs = maxWaitTimeMs;
            demandingStatus = shouldBeRunning ? ServiceControllerStatus.Running : ServiceControllerStatus.Stopped;
            serviceController = new ServiceController(BitviseServiceName);
            previousStatus = GetServerStatus(serviceController.Status);
        }

        public void Dispose()
        {
            if (serviceController != null)
            {
                serviceController.Dispose();
            }
        }

        public void SetUp()
        {
            SwitchToState(demandingStatus);
        }

        public void TearDown()
        {
            try
            {
                SwitchToState(previousStatus);
            }
            catch (Exception e)
            {
                TestContext.Out.WriteLine($"Server test fixture: {e.Message}");
            }
        }

        private void SwitchToState(ServiceControllerStatus demandingServerState)
        {
            serviceController.Refresh();
            var actualState = GetServerStatus(serviceController.Status);
            if (actualState == demandingStatus)
            {
                return;
            }

            switch (demandingServerState)
            {
                case ServiceControllerStatus.Stopped:
                    serviceController.Stop();
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped,
                        TimeSpan.FromMilliseconds(maxWaitTimeMs));
                    break;
                case ServiceControllerStatus.Running:
                    serviceController.Start();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running,
                        TimeSpan.FromMilliseconds(maxWaitTimeMs));
                    break;
                case ServiceControllerStatus.Paused:
                    serviceController.Pause();
                    serviceController.WaitForStatus(ServiceControllerStatus.Paused,
                        TimeSpan.FromMilliseconds(maxWaitTimeMs));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static ServiceControllerStatus GetServerStatus(ServiceControllerStatus serviceControllerStatus)
        {
            switch (serviceControllerStatus)
            {
                case ServiceControllerStatus.StartPending:
                case ServiceControllerStatus.ContinuePending:
                case ServiceControllerStatus.Running:
                    return ServiceControllerStatus.Running;
                case ServiceControllerStatus.StopPending:
                case ServiceControllerStatus.Stopped:
                    return ServiceControllerStatus.Stopped;
                case ServiceControllerStatus.Paused:
                case ServiceControllerStatus.PausePending:
                    return ServiceControllerStatus.Paused;
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceControllerStatus), serviceControllerStatus,
                        null);
            }
        }
    }
}