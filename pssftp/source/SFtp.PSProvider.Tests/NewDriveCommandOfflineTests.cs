using NUnit.Framework;
using SFtp.PSProvider.Tests.TestFixtures;

namespace SFtp.PSProvider.Tests
{
    [TestFixture]
    [Ignore("Pushed back these scenarios for later implementation phase")]
    public class NewDriveCommandOfflineTests : PSModuleTestFixture
    {
        private readonly IServerTestFixture serverTestFixture;

        public NewDriveCommandOfflineTests()
        {
            serverTestFixture = new FtpWareServerTestFixture(false);
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
                "New-PSDrive -Name sftp -PSProvider SFtp -Root sftp://127.0.0.1:10022/ -Credential $credential");

            Assert.That(executionResult.ErrorRecords, Is.Empty);
            Assert.That(executionResult.ShellHadErrors, Is.False);
        }
    }
}