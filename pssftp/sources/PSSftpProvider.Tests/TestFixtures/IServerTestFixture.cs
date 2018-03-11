using System;

namespace PSSftpProvider.Tests.TestFixtures
{
    internal interface IServerTestFixture : IDisposable
    {
        void SetUp();
        void TearDown();
    }
}