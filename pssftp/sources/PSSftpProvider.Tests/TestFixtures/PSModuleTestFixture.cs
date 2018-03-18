using System.Management.Automation.Runspaces;
using PSSftpProvider.Tests.Factories;

namespace PSSftpProvider.Tests.TestFixtures
{
    public class PSModuleTestFixture : PowerShellTestFixture
    {
        public override void SetUp()
        {
            var modulePathItems = new[]
            {
                CommandFactory.GetPSModulePath()
            };
            SessionState.ImportPSModule(modulePathItems);
            AddSessionVariable("credential", PasswordFactory.CreateTestingCredential(), "testing credential");

            base.SetUp();
        }

        public void AddSessionVariable(string name, object value, string description)
        {
            SessionState.Variables.Add(new SessionStateVariableEntry(name, value, description));
        }

        public void AddEnvironmentVariable(string name, object value, string description)
        {
            var entry = new SessionStateVariableEntry(name, value, description);
            SessionState.EnvironmentVariables.Add(entry);
        }
    }
}