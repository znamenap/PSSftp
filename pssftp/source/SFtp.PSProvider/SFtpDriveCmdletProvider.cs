using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Text;
using System.Threading.Tasks;
using Renci.SshNet;

namespace SFtp.PSProvider
{
    internal class SFtpPSDriveInfo : PSDriveInfo
    {
        internal SftpClient Client { get; private set; }

        internal SFtpPSDriveInfo(string name, ProviderInfo provider, string root, string description, PSCredential credential, bool persist)
            : base(name, provider, root, description, credential, persist)
        { }

        internal SFtpPSDriveInfo(PSDriveInfo driveInfo, SftpClient client)
            : base(driveInfo)
        {
            Client = client;
        }
    }

    [CmdletProvider(Globals.ProviderName, ProviderCapabilities.Credentials)]
    public class SFtpDriveCmdletProvider : NavigationCmdletProvider
    {

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            if (drive == null)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new ArgumentNullException(nameof(drive), "we expected valid PSDriveInfo object."),
                        "NullDriveInfo",
                        ErrorCategory.InvalidArgument,
                        this
                   )
                );
                return null;
            }

            var userName = System.Threading.Thread.CurrentPrincipal.Identity.Name;
            var uri = new Uri(drive.Root);
            var authenticationMethod = new List<AuthenticationMethod>();
            var driveCredential = drive.Credential;
            if (driveCredential != null &&
                !string.IsNullOrWhiteSpace(driveCredential.UserName) &&
                driveCredential.Password != null)
            {
                var credential = driveCredential.GetNetworkCredential();
                if (credential != null)
                {
                    userName = credential.UserName;
                    authenticationMethod.Add(new PasswordAuthenticationMethod(credential.UserName, credential.Password));
                }
            }
            var connectionInfo = new ConnectionInfo(uri.Host,uri.Port, userName, authenticationMethod.ToArray());

            var sftpClient = new SftpClient(connectionInfo);
            sftpClient.Connect();

            return new SFtpPSDriveInfo(drive, sftpClient);
        }

        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            if (drive == null)
            {
                WriteError(
                    new ErrorRecord(
                        new ArgumentNullException(nameof(drive), "we expected valid PSDriveInfo object."),
                        "NullDriveInfo",
                        ErrorCategory.InvalidArgument,
                        this
                   )
                );
                return null;
            }

            var sftpDrive = drive as SFtpPSDriveInfo;
            if (sftpDrive == null)
            {
                WriteError(
                    new ErrorRecord(
                        new ArgumentNullException(nameof(drive), "we expected valid SFtpPSDriveInfo object."),
                        "WrongDriveInfoType",
                        ErrorCategory.InvalidArgument,
                        this
                   )
                );
                return null;
            }

            if (sftpDrive.Client.IsConnected)
            {
                sftpDrive.Client.Disconnect();
            }
            sftpDrive.Client.Dispose();

            return sftpDrive;
        }

        protected override bool IsValidPath(string path)
        {
            return true;
        }
    }
}
