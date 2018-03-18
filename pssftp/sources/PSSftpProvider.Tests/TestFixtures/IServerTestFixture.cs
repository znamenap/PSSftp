using System;

namespace PSSftpProvider.Tests.TestFixtures
{
    internal interface IServerTestFixture : IDisposable
    {
        void SetUp();
        void TearDown();
        string Host { get; }
        ushort Port { get; }
        Uri Uri { get; }
        string DataRootPath { get; }
    }
}