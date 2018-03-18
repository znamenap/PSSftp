using System.Diagnostics;

namespace PSSftpProvider.Internal.Services
{
    internal class NullLogger : ILogger
    {
        internal static ILogger Null = new NullLogger();

        #region Implementation of ILogger

        [DebuggerStepThrough]
        void ILogger.LogCritical(string format, params object[] args)
        { }

        [DebuggerStepThrough]
        void ILogger.LogDebug(string format, params object[] args)
        {
        }

        [DebuggerStepThrough]
        void ILogger.LogError(string format, params object[] args)
        {
        }

        [DebuggerStepThrough]
        void ILogger.LogInfo(string format, params object[] args)
        {
        }

        [DebuggerStepThrough]
        void ILogger.LogMethodBegin(string methodName, string format, params object[] args)
        {
        }

        [DebuggerStepThrough]
        void ILogger.LogMethodEnd(string methodName, string format, params object[] args)
        {
        }

        [DebuggerStepThrough]
        void ILogger.LogWarn(string format, params object[] args)
        {
        }

        #endregion
    }
}