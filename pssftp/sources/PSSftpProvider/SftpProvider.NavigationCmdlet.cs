using System.Management.Automation;

namespace PSSftpProvider
{
    public partial class SftpCmdletProvider
    {
        protected override string GetParentPath(string path, string root)
        {
            Logger.LogMethodBegin(nameof(GetParentPath), "input: path={0}; root={1}", path, root);
            var result = base.GetParentPath(path, root).Replace("/", "\\");
            Logger.LogMethodEnd(nameof(GetParentPath), "result: {0}", result);
            return result;
        }

        // The MakePath method defines how your provider supports the Join-Path provider cmdlet. 
        // This cmdlet allows the user to combine a parent and child path segment to create a provider-internal path. 
        protected override string MakePath(string parent, string child)
        {
            Logger.LogMethodBegin(nameof(MakePath), "input: parent={0}, child={1}", parent, child);
            string result;
            if (string.IsNullOrWhiteSpace(parent))
            {
                parent = DriveInfo.Root;
                if (!string.IsNullOrWhiteSpace(DriveInfo.CurrentLocation))
                {
                    parent += "/" + DriveInfo.CurrentLocation;
                }
            }

            if (string.IsNullOrWhiteSpace(child))
            {
                result = parent;
            }
            else
            {
                result = parent + "/" + child;
            }

            Logger.LogMethodEnd(nameof(MakePath), "result: {0}", result);
            return result;
        }

        /// <summary>
        /// Normalizes the path that was passed in and returns the normalized path
        /// as a relative path to the basePath that was passed.
        /// </summary>
        /// <param name="path">
        /// A fully qualified provider specific path to an item. The item should exist
        /// or the provider should write out an error.
        /// </param>
        /// <param name="basePath">
        /// The path that the return value should be relative to.
        /// </param>
        /// <returns>
        /// A normalized path that is relative to the basePath that was passed. The
        /// provider should parse the path parameter, normalize the path, and then
        /// return the normalized path relative to the basePath.
        /// </returns>
        /// <remarks>
        /// This method does not have to be purely syntactical parsing of the path. It
        /// is encouraged that the provider actually use the path to lookup in its store
        /// and create a relative path that matches the casing, and standardized path syntax.
        /// Note, the base class implemenation uses GetParentPath, GetChildName, and MakePath
        /// to normalize the path and then make it relative to basePath. All string comparisons
        /// are done using StringComparison.InvariantCultureIngoreCase.
        /// </remarks>
        protected override string NormalizeRelativePath(string path, string basePath)
        {
            
            Logger.LogMethodBegin(nameof(NormalizeRelativePath), "input: path={0}; basePath={1}", path, basePath);
            var result = base.NormalizeRelativePath(path, basePath);
            Logger.LogMethodEnd(nameof(NormalizeRelativePath), "result: {0}", result);
            return result;
        }

        /// <summary>
        /// Gets the name of the leaf element in the specified path.
        /// </summary>
        /// <param name="path">The fully qualified path to the item</param>
        /// <returns>The leaf element in the path.</returns>
        /// <remarks>
        /// This should be implemented as a split on the path separator. The characters
        /// in the fullPath may not be legal characters in the namespace but may be
        /// used in globing or regular expression matching. The provider should not error
        /// unless there are no path separators in the fully qualified path.
        /// </remarks>
        protected override string GetChildName(string path)
        {
            Logger.LogMethodBegin(nameof(GetChildName), "input: path={0}", path);
            var sftpPath = GetSftpLocalPath(path);
            var attr = DriveInfo.Client.GetAttributes(sftpPath);
            var result = path;
            if (!attr.IsDirectory)
            {
                result = sftpPath;
            }

            Logger.LogMethodEnd(nameof(GetChildName), "result: {0}", result);
            return result;
        }

        private string GetSftpLocalPath(string path)
        {
            Logger.LogMethodBegin(nameof(GetSftpLocalPath), "input: path={0}", path);

            var sftpPath = path.Replace("\\", "/").Replace(DriveInfo.Root, DriveInfo.Client.WorkingDirectory);
            /*if (sftpPath.EndsWith("//"))
            {
                sftpPath = sftpPath.Substring(0, sftpPath.Length - 1);
            }*/

            Logger.LogMethodEnd(nameof(GetSftpLocalPath), "result: {0}", sftpPath);
            return sftpPath;
        }
    }
}