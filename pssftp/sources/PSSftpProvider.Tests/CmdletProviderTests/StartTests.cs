using System;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using NUnit.Framework;
using PSSftpProvider.Tests.Factories;
using PSSftpProvider.Tests.TestFixtures;

namespace PSSftpProvider.Tests.CmdletProviderTests
{
    [TestFixture]
    public class StartTests
    {
        readonly PSModuleTestFixture moduleTestFixture;

        public StartTests()
        {
            moduleTestFixture = new PSModuleTestFixture();
        }

        [SetUp]
        public void SetUp()
        {
            moduleTestFixture.SetUp();
        }

        [TearDown]
        public void TearDown()
        {
            moduleTestFixture.TearDown();
        }

        [Test]
        public void StartPSProviderWithoutParameters()
        {
            var actual = moduleTestFixture.Invoke(null, "Get-PSProvider -PSProvider " + Globals.ProviderName);

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.ShellHadErrors, Is.False);
            Assert.That(actual.ErrorRecords, Is.Empty);
            Assert.That(actual.OutputObjects, Is.Not.Empty );
            Assert.That(actual.OutputObjects[0], Is.Not.Null);
            Assert.That(actual.OutputObjects[0].ImmediateBaseObject, Is.InstanceOf<SftpProviderInfo>());
            var instance = (SftpProviderInfo) actual.OutputObjects[0].ImmediateBaseObject;
            Assert.That(instance.Name, Is.EqualTo(Globals.ProviderName));
        }

        [Test]
        public void StartPSProviderWithLoggingParameters()
        {
            moduleTestFixture.AddEnvironmentVariable("PSSftpProviderLogLevel", System.Diagnostics.SourceLevels.All, string.Empty);
            moduleTestFixture.AddEnvironmentVariable("PSSftpProviderLogPath", "c:\\temp\\test.log", string.Empty);

            var actual = moduleTestFixture.Invoke(null, "Get-PSProvider -PSProvider " + Globals.ProviderName );

            Assert.That(actual, Is.Not.Null);
            Assert.That(actual.ShellHadErrors, Is.False);
            Assert.That(actual.ErrorRecords, Is.Empty);
            Assert.That(actual.OutputObjects, Is.Not.Empty);
            Assert.That(actual.OutputObjects[0], Is.Not.Null);
            Assert.That(actual.OutputObjects[0].ImmediateBaseObject, Is.InstanceOf<SftpProviderInfo>());
            var instance = (SftpProviderInfo) actual.OutputObjects[0].ImmediateBaseObject;
            Assert.That(instance.Name, Is.EqualTo(Globals.ProviderName));
        }
    }
}