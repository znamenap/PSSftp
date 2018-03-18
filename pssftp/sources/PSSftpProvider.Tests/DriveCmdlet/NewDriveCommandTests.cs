using System.Collections;
using NUnit.Framework;
using PSSftpProvider.Tests.TestFixtures;

namespace PSSftpProvider.Tests.DriveCmdlet
{
    [TestFixture]
    public class NewDriveCommandTests : PSModuleTestFixture
    {
        private readonly IServerTestFixture serverTestFixture;

        public NewDriveCommandTests()
        {
            serverTestFixture = new FtpWareServerTestFixture(true);
            var newDriveParams = new Hashtable
            {
                {"Name", "sftp"},
                {"PSProvider", "Sftp"},
                {"Root", serverTestFixture.Uri},
            };
            AddSessionVariable("NewDriveParams", newDriveParams, "new drive params");
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
            var executionResult = Invoke(
                new object[] { },
                "New-PSDrive @NewDriveParams -Credential $credential");

            Assert.That(executionResult.ErrorRecords, Is.Empty);
            Assert.That(executionResult.ShellHadErrors, Is.False);
        }
    }
}