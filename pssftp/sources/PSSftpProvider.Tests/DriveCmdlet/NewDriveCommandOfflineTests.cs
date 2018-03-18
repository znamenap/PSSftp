using NUnit.Framework;
using PSSftpProvider.Tests.TestFixtures;

namespace PSSftpProvider.Tests.DriveCmdlet
{
    [TestFixture]
    public class NewDriveCommandOfflineTests : PSModuleTestFixture
    {
        private readonly IServerTestFixture serverTestFixture;

        public NewDriveCommandOfflineTests()
        {
            serverTestFixture = new FtpWareServerTestFixture(false);
            AddSessionVariable("uri", serverTestFixture.Uri.ToString(), "uri to sftp server.");
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // Enforce the server is not running.
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
            var executionResult = Invoke(
                new object[] { },
                "New-PSDrive -Name sftp -PSProvider Sftp -Root $uri -Credential $credential -Offline");

            Assert.That(executionResult.ErrorRecords, Is.Empty);
            Assert.That(executionResult.ShellHadErrors, Is.False);
        }
    }
}