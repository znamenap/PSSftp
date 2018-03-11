using NUnit.Framework;
using PSSftpProvider.Tests.TestFixtures;

namespace PSSftpProvider.Tests
{
    [TestFixture]
    //[Ignore("Pushed back these scenarios for later implementation phase")]
    public class NewDriveCommandOfflineTests : PSModuleTestFixture
    {
        private readonly IServerTestFixture serverTestFixture;

        public NewDriveCommandOfflineTests()
        {
            serverTestFixture = new FtpWareServerTestFixture(true);
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Enforce the server is not running.
            serverTestFixture.SetUp();
            serverTestFixture.TearDown();
            serverTestFixture.Dispose();
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
        }

        [Test]
        public void TestNewDriveCommand()
        {
            var executionResult = Execute(
                new object[] { },
                "New-PSDrive -Name sftp -PSProvider SFtp -Root sftp://127.0.0.1:10022/ -Credential $credential -Offline");

            Assert.That(executionResult.ErrorRecords, Is.Empty);
            Assert.That(executionResult.ShellHadErrors, Is.False);
        }
    }
}