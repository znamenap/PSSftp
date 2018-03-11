﻿using System.Management.Automation;

namespace PSSftpProvider
{
    internal sealed class NewSftpDriveDynamicParameters
    {
        [Parameter]
        public SwitchParameter Offline { get; set; }

        [Parameter]
        public string ServerFingerPrint { get; set; }

        [Parameter]
        public SwitchParameter SkipFingerPrintCheck { get; set; } = new SwitchParameter();

        public override string ToString()
        {
            return $"SkipFingerPrintCheck={SkipFingerPrintCheck.IsPresent}; ServerFingerPrint={ServerFingerPrint}";
        }
    }
}