using System;
using System.Linq;
using System.Management.Automation;
using NUnit.Framework;
using SFtp.PSProvider.Tests.Factories;
using SFtp.PSProvider.Tests.TestFixtures;

namespace SFtp.PSProvider.Tests
{
    [TestFixture]
    public class ImportModuleTests : PowerShellTestFixture
    {
        [Test]
        public void TestImportModule()
        {
            var executionResult = Execute(null, CommandFactory.GetImportSftpPSProviderCommand());

            Assert.That(executionResult, Is.Not.Null);
            Assert.That(executionResult.ShellHadErrors, Is.False);
            Assert.That(executionResult.OutputObjects, Is.Not.Null);
            var module = executionResult.OutputObjects.FirstOrDefault();
            Assert.That(module, Is.Not.Null);
            var matchesModuleType = module.TypeNames.Contains(typeof(PSModuleInfo).FullName,
                StringComparer.OrdinalIgnoreCase);
            Assert.That(matchesModuleType, Is.True);
            Assert.That(module.Properties["Name"].Value,
                Is.EqualTo(typeof(SFtpDriveCmdletProvider).Assembly.GetName().Name));
            dynamic moduleInfo = module;
            var name = moduleInfo.Name;
        }
    }
}