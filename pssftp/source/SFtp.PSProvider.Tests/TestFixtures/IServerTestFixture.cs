using System;

namespace SFtp.PSProvider.Tests.TestFixtures
{
    internal interface IServerTestFixture : IDisposable
    {
        void SetUp();
        void TearDown();
    }
}