using NUnit.Framework;
using SFtp.PSProvider.Tests.TestFixtures;

namespace SFtp.PSProvider.Tests
{
    [TestFixture]
    public class NewDriveCommandOfflineTests : PSModuleTestFixture
    {
        private readonly SFtpServerTestFixture serverTestFixture;

        public NewDriveCommandOfflineTests()
        {
            serverTestFixture = new SFtpServerTestFixture(false);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            serverTestFixture.SetUp();
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            serverTestFixture.TearDown();
            serverTestFixture.Dispose();
        }

        [Test]
        public void TestNewDriveCommand()
        {
            var executionResult = Execute(
                new object[] {},
                "New-PSDrive -Name sftp -PSProvider SFtp -Root sftp://127.0.0.1:2022/ -Credential $credential");

            Assert.That(executionResult.ErrorRecords, Is.Empty);
            Assert.That(executionResult.ShellHadErrors, Is.False);
        }
    }
}