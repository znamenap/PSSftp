using NUnit.Framework;
using PSSftpProvider.Tests.TestFixtures;

namespace PSSftpProvider.Tests
{
    [TestFixture]
    public class NewDriveCommandTests : PSModuleTestFixture
    {
        private readonly IServerTestFixture serverTestFixture;

        public NewDriveCommandTests()
        {
            serverTestFixture = new FtpWareServerTestFixture(true);
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
                new object[] { },
                "New-PSDrive -Name sftp -PSProvider SFtp -Root sftp://127.0.0.1:10022/ -Credential $credential");

            Assert.That(executionResult.ErrorRecords, Is.Empty);
            Assert.That(executionResult.ShellHadErrors, Is.False);
        }
    }
}