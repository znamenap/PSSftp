﻿using System;
using System.Linq;
using System.Management.Automation;
using NUnit.Framework;
using PSSftpProvider.Tests.Factories;
using PSSftpProvider.Tests.TestFixtures;

namespace PSSftpProvider.Tests
{
    [TestFixture]
    public class ImportModuleTests : PowerShellTestFixture
    {
        [Test]
        public void TestImportModule()
        {
            var executionResult = Invoke(null, CommandFactory.GetImportSftpPSProviderCommand());

            Assert.That(executionResult, Is.Not.Null);
            Assert.That(executionResult.ShellHadErrors, Is.False);
            Assert.That(executionResult.OutputObjects, Is.Not.Null);
            var module = executionResult.OutputObjects.FirstOrDefault();
            Assert.That(module, Is.Not.Null);
            var matchesModuleType = module.TypeNames.Contains(typeof(PSModuleInfo).FullName,
                StringComparer.OrdinalIgnoreCase);
            Assert.That(matchesModuleType, Is.True);
            Assert.That(module.Properties["Name"].Value,
                Is.EqualTo(typeof(SftpCmdletProvider).Assembly.GetName().Name));
            dynamic moduleInfo = module;
            var name = moduleInfo.Name;
        }
    }
}