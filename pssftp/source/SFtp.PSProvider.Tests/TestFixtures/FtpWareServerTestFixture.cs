using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace SFtp.PSProvider.Tests.TestFixtures
{
    internal class FtpWareServerConfig
    {
        internal string Password { get; set; }
        internal string Username { get; set; }
    }

    internal class FtpWareServerTestFixture : IServerTestFixture
    {
        private readonly bool shouldBeRunning;
        private Process process;

        internal FtpWareServerTestFixture(bool shouldBeRunning)
        {
            this.shouldBeRunning = shouldBeRunning;
        }

        public void Dispose()
        {
            if (process != null)
            {
                TearDown();
                process = null;
                GC.SuppressFinalize(this);
            }
        }

        public void SetUp()
        {
            if (shouldBeRunning)
            {
                var rootPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
                if (string.IsNullOrWhiteSpace(rootPath))
                {
                    throw new InvalidOperationException("Cannot determin root path.");
                }
                var startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(rootPath, "msftpsrvr.exe"),
                    Arguments = "-start",
                    UseShellExecute = false
                };
                process = StartServerProcess(startInfo);
            }
        }

        public void TearDown()
        {
            if (process != null)
            {
                bool exited = false;
                var messageDelivered = process.CloseMainWindow();
                if (messageDelivered)
                {
                    exited = process.WaitForExit(2000);
                }
                if (!exited)
                {
                    process.Kill();
                }
            }
        }
        private static Process StartServerProcess(ProcessStartInfo startInfo)
        {
            var process = Process.Start(startInfo);
            if (process != null)
            {
                var exited = process.WaitForExit(1000);
                if (exited)
                {
                    throw new InvalidOperationException("Cannot start FtpWare SSh Server.");
                }
            }
            return process;
        }
    }
}