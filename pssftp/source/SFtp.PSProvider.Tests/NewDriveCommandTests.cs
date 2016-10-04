using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.PowerShell.Commands;
using NUnit.Framework.Internal;
using SFtp.PSProvider.Tests.Factories;
using SFtp.PSProvider.Tests.TestFixtures;

namespace SFtp.PSProvider.Tests
{
    public class PasswordFactory
    {
        public static SecureString CreateSecureString(string value)
        {
            var result = new SecureString();
            value.ToList().ForEach(x => result.AppendChar(x));
            return result;
        }

        public static PSCredential CreatePSCredential(string userName, SecureString password)
        {
            return new PSCredential(userName, password);
        }

        public static PSCredential CreateTestingCredential()
        {
            return CreatePSCredential("username", CreateSecureString("password"));
        }
    }
    [TestFixture]
    public class NewDriveCommandTests : PSModuleTestFixture
    {
        [Test]
        public void TestNewDriveCommand()
        {
            var executionResult = Execute(
                new object[] {}, 
                new[]
                {
                    "New-PSDrive -Name sftp -PSProvider SFtp -Root sftp://127.0.0.1:2022/ -Credential $credential"
                }
            );

            Assert.That(executionResult.ErrorRecords, Is.Empty);
            Assert.That(executionResult.ShellHadErrors, Is.False);
        }
    }
}
