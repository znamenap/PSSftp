using System;
using System.Management.Automation;
using System.Management.Automation.Provider;
using PSSftpProvider.Internal.Services;

namespace PSSftpProvider
{
    [CmdletProvider(Globals.ProviderName, ProviderCapabilities.Credentials | ProviderCapabilities.Filter)]
    public partial class SftpCmdletProvider : NavigationCmdletProvider
    {
        private readonly ISftpLogger log;

        public SftpDriveInfo DriveInfo => (SftpDriveInfo) PSDriveInfo;

        public SftpCmdletProvider()
            : this(SftpLoggerSource.Instance)
        {
        }

        public SftpCmdletProvider(ISftpLogger log)
        {
            this.log = log ?? throw new ArgumentNullException("log");
            log.LogMethodBegin(nameof(SftpCmdletProvider), ".ctor");
            log.LogMethodEnd(nameof(SftpCmdletProvider), ".ctor");
        }

        protected override ProviderInfo Start(ProviderInfo providerInfo)
        {
            log.LogMethodBegin(nameof(Start), "input: providerInfo={0}", providerInfo);
            log.LogInfo(providerInfo.ToString());
            var result = base.Start(providerInfo);
            log.LogMethodEnd(nameof(Start), "result: {0}", result);
            return result;
        }

        protected override void Stop()
        {
            log.LogMethodBegin(nameof(Stop), "void");
            base.Stop();
            log.LogMethodEnd(nameof(Stop), "void");
        }


        private string GetSftpLocalPath(string path)
        {
            log.LogMethodBegin(nameof(GetSftpLocalPath), "input: path={0}", path);

            var sftpPath = path.Replace("\\", "/").Replace(DriveInfo.Root, DriveInfo.Client.WorkingDirectory);
            /*if (sftpPath.EndsWith("//"))
            {
                sftpPath = sftpPath.Substring(0, sftpPath.Length - 1);
            }*/

            log.LogMethodEnd(nameof(GetSftpLocalPath), "result: {0}", sftpPath);
            return sftpPath;
        }
    }
}