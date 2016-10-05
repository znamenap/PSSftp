using System;
using System.Diagnostics;
using System.ServiceProcess;
using System.Threading;
using TimeoutException = System.ServiceProcess.TimeoutException;

namespace SFtp.PSProvider.Tests.TestFixtures
{
    internal enum DemandingServerState
    {
        Stopped,
        Running
    }

    internal class SFtpServerTestFixture : IDisposable
    {
        private readonly long maxWaitTimeMs;
        private readonly ServiceController serviceController;
        private readonly ServiceControllerStatus demandingStatus;
        private readonly ServiceControllerStatus previousStatus;

        public SFtpServerTestFixture(bool shouldBeRunning, long maxWaitTimeMs = 3000)
        {
            this.maxWaitTimeMs = maxWaitTimeMs;
            demandingStatus = shouldBeRunning ? ServiceControllerStatus.Running : ServiceControllerStatus.Stopped;
            serviceController = new ServiceController("BvSshServer-DEV");
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
                NUnit.Framework.TestContext.Out.WriteLine($"Server test fixture: {e.Message}");
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
                    serviceController.WaitForStatus(ServiceControllerStatus.Stopped, TimeSpan.FromMilliseconds(maxWaitTimeMs));
                    break;
                case ServiceControllerStatus.Running:
                    serviceController.Start();
                    serviceController.WaitForStatus(ServiceControllerStatus.Running, TimeSpan.FromMilliseconds(maxWaitTimeMs));
                    break;
                case ServiceControllerStatus.Paused:
                    serviceController.Pause();
                    serviceController.WaitForStatus(ServiceControllerStatus.Paused, TimeSpan.FromMilliseconds(maxWaitTimeMs));
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
                    throw new ArgumentOutOfRangeException(nameof(serviceControllerStatus), serviceControllerStatus, null);
            }
        }

    }
}