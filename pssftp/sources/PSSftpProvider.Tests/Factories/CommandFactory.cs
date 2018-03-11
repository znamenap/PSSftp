namespace PSSftpProvider.Tests.Factories
{
    internal class CommandFactory
    {
        internal static string GetImportSftpPSProviderCommand()
        {
            return "Import-Module '" + GetPSModulePath() + "' -PassThru";
        }

        internal static string GetPSModulePath()
        {
            return typeof(SftpCmdletProvider).Assembly.Location;
        }
    }
}