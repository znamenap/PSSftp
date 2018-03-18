using System.Diagnostics;
using System.Management.Automation;

namespace PSSftpProvider.Parameters
{
    internal class CmdletProviderStartDynamicParameters
    {
        public CmdletProviderStartDynamicParameters()
        {
            LogLevel = SourceLevels.Warning;
        }

        [Parameter]
        public SourceLevels LogLevel { get; set; }

        [Parameter]
        public string LogPath { get; set; }
    }
}