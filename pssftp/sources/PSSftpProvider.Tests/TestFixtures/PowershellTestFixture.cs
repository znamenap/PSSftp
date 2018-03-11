using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using NUnit.Framework;

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

    public class PowerShellTestFixture
    {
        public InitialSessionState SessionState { get; set; }
        public PowerShell Shell { get; set; }

        public PowerShellTestFixture()
        {
            SessionState = InitialSessionState.CreateDefault2();
        }

        [OneTimeSetUp]
        public void SetUpShell()
        {
            Shell = PowerShell.Create(SessionState);
        }

        [OneTimeTearDown]
        public void TearDownShell()
        {
            if (Shell != null)
            {
                Shell.Dispose();
            }
        }

        public PowerShellExecutionResult Execute(IEnumerable inputs, params string[] lines)
        {
            var outputLines = new List<string>();

            foreach (var line in lines)
            {
                Shell.AddScript(line);
            }

            Shell.Streams.ClearStreams();
            var outputObjects = inputs != null ? Shell.Invoke(inputs) : Shell.Invoke();
            var errorObjects = Shell.Streams.Error.ToList();
            errorObjects.ForEach(record => Console.Error.WriteLine("{0}{1}{2}{3}", record.ToString(),
                Environment.NewLine,
                record.ErrorDetails, Environment.NewLine));

            return new PowerShellExecutionResult(outputLines.ToArray(), outputObjects.ToArray(), errorObjects.ToArray(),
                Shell.InvocationStateInfo, Shell.HadErrors);
        }
    }
}