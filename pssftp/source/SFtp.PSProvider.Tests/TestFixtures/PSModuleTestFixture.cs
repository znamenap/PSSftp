using System.Management.Automation.Runspaces;
using SFtp.PSProvider.Tests.Factories;

namespace SFtp.PSProvider.Tests.TestFixtures
{
    public class PSModuleTestFixture : PowerShellTestFixture
    {
        public PSModuleTestFixture()
        {
            SessionState.ImportPSModule(new[] { CommandFactory.GetPSModulePath() });
            SessionState.Variables.Add(new SessionStateVariableEntry("credential", 
                PasswordFactory.CreateTestingCredential(), "testing credential"));

        }
    }
}
