namespace PSSftpProvider
{
    public interface ILogger
    {
        void LogCritical(string format, params object[] args);
        void LogDebug(string format, params object[] args);
        void LogError(string format, params object[] args);
        void LogInfo(string format, params object[] args);
        void LogMethodBegin(string methodName, string format, params object[] args);
        void LogMethodEnd(string methodName, string format, params object[] args);
        void LogWarn(string format, params object[] args);
    }
}