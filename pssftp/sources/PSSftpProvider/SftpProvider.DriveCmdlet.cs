using System;
using System.Collections.Generic;
using System.Management.Automation;
using System.Threading;
using PSSftpProvider.Parameters;
using Renci.SshNet;

namespace PSSftpProvider
{
    public partial class SftpCmdletProvider
    {
        protected internal SftpDriveInfo DriveInfo => (SftpDriveInfo)PSDriveInfo;

        protected override object NewDriveDynamicParameters()
        {
            Logger.LogMethodBegin(nameof(NewDriveDynamicParameters), "void");
            var result = new NewDriveDynamicParameters();
            Logger.LogMethodEnd(nameof(NewDriveDynamicParameters), "result : {0}", result);
            return result;
        }

        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            if (drive == null)
            {
                ThrowTerminatingError(
                    new ErrorRecord(
                        new ArgumentNullException(nameof(drive), "we expected valid DriveInfo object."),
                        "NullDriveInfo",
                        ErrorCategory.InvalidArgument,
                        this
                    )
                );
                return null;
            }

            var dynamicParams = DynamicParameters as NewDriveDynamicParameters;
            var userName = Thread.CurrentPrincipal.Identity.Name;
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
                    authenticationMethod.Add(new PasswordAuthenticationMethod(credential.UserName,
                        credential.Password));
                }
            }

            var connectionInfo = new ConnectionInfo(uri.Host, uri.Port, userName, authenticationMethod.ToArray());

            var sftpClient = new SftpClient(connectionInfo);
            if (dynamicParams != null && !dynamicParams.Offline)
            {
                sftpClient.Connect();
            }

            return new SftpDriveInfo(drive, sftpClient);
        }

        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            if (drive == null)
            {
                WriteError(
                    new ErrorRecord(
                        new ArgumentNullException(nameof(drive), "we expected valid DriveInfo object."),
                        "NullDriveInfo",
                        ErrorCategory.InvalidArgument,
                        this
                    )
                );
                return null;
            }

            var sftpDrive = drive as SftpDriveInfo;
            if (sftpDrive == null)
            {
                WriteError(
                    new ErrorRecord(
                        new ArgumentNullException(nameof(drive), "we expected valid SftpDriveInfo object."),
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
    }
}