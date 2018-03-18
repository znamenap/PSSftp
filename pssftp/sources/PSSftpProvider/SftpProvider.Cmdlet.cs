using System;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Management.Automation.Security;
using PSSftpProvider.Internal.Services;
using PSSftpProvider.Parameters;

namespace PSSftpProvider
{
    [CmdletProvider(Globals.ProviderName, ProviderCapabilities.Credentials | ProviderCapabilities.Filter)]
    public partial class SftpCmdletProvider : NavigationCmdletProvider
    {
        internal new SftpProviderInfo ProviderInfo => (SftpProviderInfo) base.ProviderInfo;

        internal ILogger Logger => ProviderInfo.Logger;

        /// <summary>
        ///     Gets an object that defines the additional parameters for the Start implementation
        ///     for a provider.
        /// </summary>
        /// <returns>
        ///     Overrides of this method should return an object that has properties and fields decorated with
        ///     parsing attributes similar to a cmdlet class or a
        ///     <see cref="T:System.Management.Automation.RuntimeDefinedParameterDictionary" />.
        ///     The default implemenation returns null. (no additional parameters)
        /// </returns>
        protected override object StartDynamicParameters()
        {
            return new CmdletProviderStartDynamicParameters();
        }

        /// <summary>
        ///     Gives the provider the opportunity to initialize itself.
        /// </summary>
        /// <param name="providerInfo">
        ///     The information about the provider that is being started.
        /// </param>
        /// <remarks>
        ///     The default implementation returns the ProviderInfo instance that
        ///     was passed.
        ///     To have session state maintain persisted data on behalf of the provider,
        ///     the provider should derive from <see cref="T:System.Management.Automation.ProviderInfo" />
        ///     and add any properties or
        ///     methods for the data it wishes to persist.  When Start gets called the
        ///     provider should construct an instance of its derived ProviderInfo using the
        ///     providerInfo that is passed in and return that new instance.
        /// </remarks>
        protected override ProviderInfo Start(ProviderInfo providerInfo)
        {
            var logger = CreateLogger();

            var specificProviderInfo = new SftpProviderInfo(providerInfo, logger);
            logger.LogMethodBegin(nameof(Start), "input: providerInfo={0}", specificProviderInfo);
            logger.LogInfo(specificProviderInfo.ToString());
            logger.LogMethodEnd(nameof(Start), "result: {0}", specificProviderInfo);

            return specificProviderInfo;
        }

        private ILogger CreateLogger()
        {
            SourceLevels logLevel;
            var logLevelStr = InvokeCommand.ExpandString("$env:PSSftpProviderLogLevel");
            if (Enum.TryParse(logLevelStr, true, out logLevel))
            {
                var logPathStr = InvokeCommand.ExpandString("$env:PSSftpProviderLogPath");
                return new FileLogger(logLevel); //TODO: add log file specification
            }

            return NullLogger.Null;
        }

        protected override void StopProcessing()
        {
            Logger.LogMethodBegin(nameof(StopProcessing), "void");
            base.StopProcessing();
            Logger.LogMethodEnd(nameof(StopProcessing), "void");
        }

        /// <summary>
        /// Called by session state when the provider is being removed.
        /// </summary>
        /// <remarks>
        /// A provider should override this method to free up any resources that the provider
        /// was using.
        /// The default implementation does nothing.
        /// </remarks>
        protected override void Stop()
        {
            Logger.LogMethodBegin(nameof(Stop), "void");
            Debug.Assert(ProviderInfo.Drives != null, "ProviderInfo.Drives != null");
            (ProviderInfo.Drives[0] as SftpDriveInfo)?.Client.Disconnect();
            Logger.LogMethodEnd(nameof(Stop), "void");
        }
    }
}