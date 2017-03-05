using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Win32;

namespace SFtp.PSProvider.Tests.TestFixtures
{
    internal class FtpWareServerConfig
    {
        [System.ComponentModel.DisplayName("PW")]
        internal string Password { get; set; }

        [System.ComponentModel.DisplayName("User")]
        internal string Username { get; set; }

        [System.ComponentModel.DisplayName("Port")]
        internal ushort Port { get; set; }

        [System.ComponentModel.DisplayName("Path")]
        internal string DataRootPath { get; set; }

        internal string ExeRootPath { get; set; }

        [System.ComponentModel.DisplayName("Addr")]
        internal string BindAddress { get; set; }

        [System.ComponentModel.DisplayName("Zlib")]
        internal bool ZLib { get; set; }

        [System.ComponentModel.DisplayName("Nagle")]
        internal bool Nagle { get; set; }

        [System.ComponentModel.DisplayName("SCP")]
        internal bool Scp { get; set; }

        [System.ComponentModel.DisplayName("Fips")]
        internal bool Fips { get; set; }

        [System.ComponentModel.DisplayName("Hidden")]
        internal bool ShowHiddenFiles { get; set; }
    }

    class FtpWareServerConfigRegistryStore
    {
        private const string FtpWareSubKeyRegPath = @"SOFTWARE\FTPWare\msftpsrvr\msftpsrvr\";

        private static readonly byte[] FtpWareHashedPassword = {
            23, 23, 56, 52, 50, 67, 56, 51, 52, 54, 56, 65, 68, 51, 51, 70, 69, 54, 56, 49, 68, 66, 56, 54, 68, 70,
            51, 57, 49, 50, 66, 57, 67, 56, 52, 48, 54, 48, 65, 49, 50, 67, 69, 48, 68, 66, 54, 55, 65, 54, 65, 48,
            70, 53, 57, 55, 65, 69, 54, 57, 50, 67, 67, 53, 49, 48
        };

        internal FtpWareServerConfig Load()
        {
            using (var reg = Registry.CurrentUser.OpenSubKey(FtpWareSubKeyRegPath))
            {
                if (reg == null)
                {
                    throw new InvalidOperationException($@"Cannot open registry for FTPWare at HKCU:\{FtpWareSubKeyRegPath}.");
                }
                
                var password = (string)reg.GetValue("PW");
                var port = (ushort) reg.GetValue("Port");
                var user = (string) reg.GetValue("User"); // User: username
                var path = (string) reg.GetValue("Path"); // Path   : c:\
                var addr = (string) reg.GetValue("Addr"); // Addr: 127.0.0.1
                var zlib = Convert.ToBoolean((uint)reg.GetValue("ZLib")); // Zlib: 1
                var nagle = Convert.ToBoolean((uint)reg.GetValue("Nagle")); // Nagle: 1
                var scp = Convert.ToBoolean((uint)reg.GetValue("SCP")); // SCP: 1
                var fips = Convert.ToBoolean((uint)reg.GetValue("Fips")); // Fips: 1
                var hidden = Convert.ToBoolean((uint)reg.GetValue("Hidden")); // Hidden: 1

                var config = new FtpWareServerConfig
                {
                    Password = password,
                    Username = user,
                    Port = port,
                    BindAddress = addr,
                    DataRootPath = path,
                    ZLib = zlib,
                    Fips = fips,
                    Nagle = nagle,
                    Scp = scp,
                    ShowHiddenFiles = hidden
                };
                return config;
            }
        }

        internal void Save(FtpWareServerConfig config)
        {
            using (var reg = Registry.CurrentUser.OpenSubKey(FtpWareSubKeyRegPath))
            {
                if (reg == null)
                {
                    throw new InvalidOperationException($@"Cannot open registry for FTPWare at HKCU:\{FtpWareSubKeyRegPath}.");
                }

                reg.SetValue("PW", config.Password, RegistryValueKind.String);
                reg.SetValue("User", config.Username, RegistryValueKind.String);
                reg.SetValue("Port", config.Port, RegistryValueKind.DWord);
                var password = (string)reg.GetValue("PW");
                var port = (ushort)reg.GetValue("Port");
                var user = (string)reg.GetValue("User"); // User: username
                var path = (string)reg.GetValue("Path"); // Path   : c:\
                var addr = (string)reg.GetValue("Addr"); // Addr: 127.0.0.1
                var zlib = Convert.ToBoolean((uint)reg.GetValue("ZLib")); // Zlib: 1
                var nagle = Convert.ToBoolean((uint)reg.GetValue("Nagle")); // Nagle: 1
                var scp = Convert.ToBoolean((uint)reg.GetValue("SCP")); // SCP: 1
                var fips = Convert.ToBoolean((uint)reg.GetValue("Fips")); // Fips: 1
                var hidden = Convert.ToBoolean((uint)reg.GetValue("Hidden")); // Hidden: 1
                
            }
        }
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
                    throw new InvalidOperationException("Cannot determine root path.");
                }

                var config = new FtpWareServerConfig
                {
                    BindAddress = "127.0.0.1",
                    DataRootPath = Path.Combine(rootPath, "data"),
                    ExeRootPath = rootPath,
                    Fips = true,
                    Nagle = true,
                    Port = 10022,
                    ShowHiddenFiles = true,
                    Scp = true,
                    Username = "username",
                    Password = "password"
                };
                var previousServerConfig = PushServerConfig(config);

                var startInfo = new ProcessStartInfo
                {
                    FileName = Path.Combine(rootPath, "msftpsrvr.exe"),
                    Arguments = "-start",
                    UseShellExecute = false,
                    WindowStyle = ProcessWindowStyle.Hidden
                };
                process = StartServerProcess(startInfo);
            }
        }

        private FtpWareServerConfig PushServerConfig(FtpWareServerConfig config)
        {
            
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