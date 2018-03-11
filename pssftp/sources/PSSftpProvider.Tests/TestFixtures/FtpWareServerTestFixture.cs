using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.Win32;

namespace PSSftpProvider.Tests.TestFixtures
{
    internal class FtpWareServerConfig
    {
        [DisplayName("PW")]
        internal string Password { get; set; }

        [DisplayName("User")]
        internal string Username { get; set; }

        [DisplayName("Port")]
        internal ushort Port { get; set; }

        [DisplayName("Path")]
        internal string DataRootPath { get; set; }

        internal string ExeRootPath { get; set; }

        [DisplayName("Addr")]
        internal string BindAddress { get; set; }

        [DisplayName("Zlib")]
        internal bool ZLib { get; set; }

        [DisplayName("Nagle")]
        internal bool Nagle { get; set; }

        [DisplayName("SCP")]
        internal bool Scp { get; set; }

        [DisplayName("Fips")]
        internal bool Fips { get; set; }

        [DisplayName("Hidden")]
        internal bool ShowHiddenFiles { get; set; }
    }

    internal class FtpWareServerConfigRegistryStore
    {
        private const string FtpWareSubKeyRegPath = @"SOFTWARE\FTPWare\msftpsrvr\msftpsrvr\";

        private static readonly byte[] FtpWareHashedPassword =
        {
            23, 23, 56, 52, 50, 67, 56, 51, 52, 54, 56, 65, 68, 51, 51, 70, 69, 54, 56, 49, 68, 66, 56, 54, 68, 70,
            51, 57, 49, 50, 66, 57, 67, 56, 52, 48, 54, 48, 65, 49, 50, 67, 69, 48, 68, 66, 54, 55, 65, 54, 65, 48,
            70, 53, 57, 55, 65, 69, 54, 57, 50, 67, 67, 53, 49, 48
        };

        internal static string HashedPassword => Encoding.ASCII.GetString(FtpWareHashedPassword);

        internal FtpWareServerConfig Load()
        {
            using (var reg = Registry.CurrentUser.OpenSubKey(FtpWareSubKeyRegPath))
            {
                if (reg == null)
                {
                    throw new InvalidOperationException(
                        $@"Cannot open registry for FTPWare at HKCU:\{FtpWareSubKeyRegPath}");
                }

                var password = (string) reg.GetValue("PW");
                var port = Convert.ToUInt16(reg.GetValue("Port"));
                var user = (string) reg.GetValue("User"); // User: username
                var path = (string) reg.GetValue("Path"); // Path   : c:\
                var addr = (string) reg.GetValue("Addr"); // Addr: 127.0.0.1
                var zlib = Convert.ToInt32(reg.GetValue("ZLib")) == 1 ? true : false; // Zlib: 1
                var nagle = Convert.ToInt32(reg.GetValue("Nagle")) == 1 ? true : false; // Nagle: 1
                var scp = Convert.ToInt32(reg.GetValue("SCP")) == 1 ? true : false; // SCP: 1
                var fips = Convert.ToInt32(reg.GetValue("Fips")) == 1 ? true : false; // Fips: 1
                var hidden = Convert.ToInt32(reg.GetValue("Hidden")) == 1 ? true : false; // Hidden: 1

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
            using (var reg = OpenOrCreateRegistryKey())
            {
                if (reg == null)
                {
                    throw new InvalidOperationException(
                        $@"Cannot open registry for FTPWare at HKCU:\{FtpWareSubKeyRegPath}");
                }

                reg.SetValue("PW", config.Password, RegistryValueKind.String);
                reg.SetValue("Port", config.Port.ToString(), RegistryValueKind.String);
                reg.SetValue("User", config.Username, RegistryValueKind.String);
                reg.SetValue("Path", config.DataRootPath, RegistryValueKind.String);
                reg.SetValue("Addr", config.BindAddress, RegistryValueKind.String);
                reg.SetValue("ZLib", config.ZLib ? 1 : 0, RegistryValueKind.DWord);
                reg.SetValue("Nagle", config.Nagle ? 1 : 0, RegistryValueKind.DWord);
                reg.SetValue("SCP", config.Scp ? 1 : 0, RegistryValueKind.DWord);
                reg.SetValue("Fips", config.Fips ? 1 : 0, RegistryValueKind.DWord);
                reg.SetValue("Hidden", config.ShowHiddenFiles ? 1 : 0, RegistryValueKind.DWord);
            }
        }

        private static RegistryKey OpenOrCreateRegistryKey()
        {
            var regKey = Registry.CurrentUser.OpenSubKey(FtpWareSubKeyRegPath, true);
            if (regKey == null)
            {
                regKey = Registry.CurrentUser.CreateSubKey(FtpWareSubKeyRegPath,
                    RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryOptions.None);
            }

            return regKey;
        }
    }

    internal class FtpWareServerTestFixture : IServerTestFixture
    {
        private readonly bool shouldBeRunning;
        private Process process;

        public string DataRootPath { get; private set; }

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

                DataRootPath = Path.Combine(rootPath, "data");
                if (!Directory.Exists(DataRootPath))
                {
                    Directory.CreateDirectory(DataRootPath);
                }

                var config = new FtpWareServerConfig
                {
                    BindAddress = "127.0.0.1",
                    DataRootPath = DataRootPath,
                    ExeRootPath = rootPath,
                    ZLib = true,
                    Fips = true,
                    Nagle = true,
                    Port = 10022,
                    ShowHiddenFiles = true,
                    Scp = true,
                    Username = "username",
                    Password = FtpWareServerConfigRegistryStore.HashedPassword
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

        public void TearDown()
        {
            if (process != null)
            {
                var exited = false;
                var messageDelivered = process.CloseMainWindow();
                if (messageDelivered)
                {
                    exited = process.WaitForExit(4000);
                }

                if (!exited)
                {
                    process.Kill();
                }
            }
        }

        private FtpWareServerConfig PushServerConfig(FtpWareServerConfig config)
        {
            var store = new FtpWareServerConfigRegistryStore();
            FtpWareServerConfig previousConfig = null;
            try
            {
                previousConfig = store.Load();
            }
            catch (InvalidOperationException exception)
            {
                Console.WriteLine("WARNING: " + exception.Message);
            }
            finally
            {
                store.Save(config);
            }

            return previousConfig;
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