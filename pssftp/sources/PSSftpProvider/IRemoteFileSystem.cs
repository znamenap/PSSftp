using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PSSftpProvider
{
    interface IRemoteFileSystem
    {
        /// <summary>
        /// Connects to remote file system.
        /// </summary>
        void Connect();

        /// <summary>
        /// It tests connection state and ensures the remote file system is available and responsive.
        /// </summary>
        /// <returns>Returns true if all tests passed.</returns>
        bool TestConnection();

        /// <summary>
        /// Disconnects from the remote file system.
        /// </summary>
        void Disconnect();
    }
}
