namespace SFtp.PSProvider.Tests.Factories
{
    internal class CommandFactory
    {
        internal static string GetImportSftpPSProviderCommand()
        {
            return "Import-Module '" + GetPSModulePath() + "' -PassThru";
        }

        internal static string GetPSModulePath()
        {
            return typeof(SFtpDriveCmdletProvider).Assembly.Location;
        }
    }
}