using System;
using System.Diagnostics;
using System.Management.Automation;
using PSSftpProvider.Internal.Services;

namespace PSSftpProvider
{
    public sealed class SftpProviderInfo : ProviderInfo
    {
        internal ILogger Logger { get; }

        /// <summary>
        ///     Constructs an instance of the class using an existing reference
        ///     as a template.
        /// </summary>
        /// <param name="providerInfo">
        ///     The provider information to copy to this instance.
        /// </param>
        /// <remarks>
        ///     This constructor should be used by derived types to easily copying
        ///     the base class members from an existing ProviderInfo.
        ///     This is designed for use by a <see cref="T:System.Management.Automation.Provider.CmdletProvider" />
        ///     during calls to their
        ///     <see cref="M:System.Management.Automation.Provider.CmdletProvider.Start(System.Management.Automation.ProviderInfo)" />
        ///     method.
        /// </remarks>
        /// <exception cref="T:System.ArgumentNullException">
        ///     If <paramref name="providerInfo" /> is null.
        /// </exception>
        public SftpProviderInfo(ProviderInfo providerInfo)
            : this(providerInfo,
#if DEBUG
                new FileLogger(SourceLevels.All)
#else
                NullLogger.Null
#endif
                )
        {
        }

        public SftpProviderInfo(ProviderInfo providerInfo, ILogger logger)
            : base(providerInfo)
        {
            Logger = logger ?? throw new ArgumentNullException("logger");
            Logger.LogMethodBegin(nameof(SftpCmdletProvider), ".ctor");
            Logger.LogMethodEnd(nameof(SftpCmdletProvider), ".ctor");
        }
    }
}