using System.Management.Automation;
using Renci.SshNet;

namespace PSSftpProvider
{
    public class SftpDriveInfo : PSDriveInfo
    {
        internal SftpClient Client { get; }

        internal SftpDriveInfo(string name, ProviderInfo provider, string root, string description,
            PSCredential credential, bool persist)
            : base(name, provider, root, description, credential, persist)
        {
        }

        internal SftpDriveInfo(PSDriveInfo driveInfo, SftpClient client)
            : base(driveInfo)
        {
            Client = client;
        }
    }
}