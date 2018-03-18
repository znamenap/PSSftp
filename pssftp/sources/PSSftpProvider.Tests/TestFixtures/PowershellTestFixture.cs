using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;

namespace PSSftpProvider.Tests.TestFixtures
{
    public class PowerShellExecutionResult
    {
        public string[] OutputLines { get; }

        public PSObject[] OutputObjects { get; }

        public ErrorRecord[] ErrorRecords { get; }

        public PSInvocationStateInfo InvocationStateInfo { get; }

        public bool ShellHadErrors { get; }

        public PowerShellExecutionResult(string[] outputLines, PSObject[] outputObjects, ErrorRecord[] errorRecords,
            PSInvocationStateInfo invocationStateInfo, bool shellHadErrors)
        {
            OutputLines = outputLines;
            OutputObjects = outputObjects;
            ErrorRecords = errorRecords;
            InvocationStateInfo = invocationStateInfo;
            ShellHadErrors = shellHadErrors;
        }
    }

    public interface ITestFixture
    {
        void SetUp();

        void TearDown();
    }

    public class SessionStateTestFixture : ITestFixture
    {
        public InitialSessionState SessionState { get; private set; }

        public void SetUp()
        {
            SessionState = InitialSessionState.CreateDefault2();
        }

        public void TearDown()
        {
        }
    }

    public class PowerShellTestFixture : ITestFixture
    {

        public PowerShell Shell { get; private set; }


        public virtual void SetUp()
        {
        }

        public virtual void TearDown()
        {
            if (Shell != null)
            {
                Shell.Dispose();
                Shell = null;
            }
        }

        public PowerShellExecutionResult Invoke(IEnumerable inputs, params string[] lines)
        {
            Shell = PowerShell.Create(SessionState);
            return Invoke(Shell, inputs, lines);
        }

        public PowerShellExecutionResult Invoke(PowerShell shell, IEnumerable inputs, params string[] lines)
        {
            foreach (var line in lines)
            {
                shell.AddScript(line);
            }

            shell.Streams.ClearStreams();
            var outputLines = new List<string>();

            var outputObjects = inputs != null ? shell.Invoke(inputs) : shell.Invoke();

            var errorObjects = shell.Streams.Error.ToList();
            errorObjects.ForEach(WriteConsoleError);

            return new PowerShellExecutionResult(outputLines.ToArray(), outputObjects.ToArray(), errorObjects.ToArray(),
                shell.InvocationStateInfo, shell.HadErrors);
        }

        private static void WriteConsoleError(ErrorRecord record)
        {
            Console.Error.WriteLine("{0}{1}{2}{3}", record, Environment.NewLine, record.ErrorDetails,
                Environment.NewLine);
        }
    }
}