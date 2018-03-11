using System.Management.Automation.Runspaces;
using PSSftpProvider.Tests.Factories;

namespace PSSftpProvider.Tests.TestFixtures
{
    public class PSModuleTestFixture : PowerShellTestFixture
    {
        public PSModuleTestFixture()
        {
            SessionState.ImportPSModule(new[] {CommandFactory.GetPSModulePath()});
            SessionState.Variables.Add(new SessionStateVariableEntry("credential",
                PasswordFactory.CreateTestingCredential(), "testing credential"));
        }
    }
}