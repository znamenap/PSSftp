using System;
using System.Diagnostics;
using System.IO;

namespace PSSftpProvider.Internal.Services
{
    internal sealed class SftpLoggerSource : TraceSource, ISftpLogger
    {
        private const int BeginMethodEventId = 30030;
        private const int EndMethodEventId = 30031;
        private const int NonSpecificDebugEventId = 30040;
        private const int NonSpecificInfoEventId = 30041;
        private const int NonSpecificWarnEventId = 30042;
        private const int NonSpecificErrorEventId = 30043;
        private const int NonSpecificCriticalEventId = 30044;

        internal const string PSSftpTraceSourceName = "PSSftpProvider";

        private static readonly Lazy<SftpLoggerSource> instance =
            new Lazy<SftpLoggerSource>(() => new SftpLoggerSource(SourceLevels.All));

        private TextWriterTraceListener textListener;

        internal static ISftpLogger Instance => instance.Value;

        public SftpLoggerSource() : base(PSSftpTraceSourceName)
        {
            Initialise();
        }

        public SftpLoggerSource(SourceLevels defaultLevel) : base(PSSftpTraceSourceName, defaultLevel)
        {
            Initialise();
        }

        public void LogMethodBegin(string methodName, string format, params object[] args)
        {
            TraceEvent(TraceEventType.Verbose, BeginMethodEventId,
                string.Format("Begin {0} : {1}", methodName, format), args);
            textListener.IndentLevel++;
            textListener.Flush();
        }

        public void LogMethodEnd(string methodName, string format, params object[] args)
        {
            textListener.IndentLevel--;
            TraceEvent(TraceEventType.Verbose, EndMethodEventId,
                string.Format("End   {0} : {1}", methodName, format), args);
            textListener.Flush();
        }

        public void LogCritical(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Critical, NonSpecificCriticalEventId, format, args);
            textListener.Flush();
        }

        public void LogError(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Error, NonSpecificErrorEventId, format, args);
            textListener.Flush();
        }

        public void LogWarn(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Warning, NonSpecificWarnEventId, format, args);
            textListener.Flush();
        }

        public void LogInfo(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Information, NonSpecificInfoEventId, format, args);
            textListener.Flush();
        }

        public void LogDebug(string format, params object[] args)
        {
            TraceEvent(TraceEventType.Verbose, NonSpecificDebugEventId, format, args);
            textListener.Flush();
        }

        public void Initialise()
        {
            Switch = new SourceSwitch(PSSftpTraceSourceName, "Verbose");
            Listeners.Remove("Default");

            var logFileName = string.Format("PSSftpProvider_{0}_{1}.log", DateTime.Now.ToString("yyyyMMdd-HHmmss"),
                Process.GetCurrentProcess().Id);
#if DEBUG
            logFileName = "PSSftpProvider.log";
#endif
            var logPath = Path.Combine(Environment.GetEnvironmentVariable("TEMP"), logFileName);
            textListener = new TextWriterTraceListener(logPath)
            {
                Filter = new EventTypeFilter(SourceLevels.All),
                IndentSize = 2,
                IndentLevel = 0
            };
            // textListener.TraceOutputOptions |= TraceOptions.DateTime | TraceOptions.ProcessId | TraceOptions.ThreadId;
            Listeners.Add(textListener);

            // a naive approach
            AppDomain.CurrentDomain.ProcessExit += (s, e) =>
            {
                textListener.WriteLine(string.Format("Closing at {0:s}", DateTime.Now));
                textListener.Close();
            };
        }
    }
}