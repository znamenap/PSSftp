namespace PSSftpProvider
{
    public partial class SftpCmdletProvider
    {
        protected override string GetParentPath(string path, string root)
        {
            log.LogMethodBegin(nameof(GetParentPath), "input: path={0}; root={1}", path, root);
            var result = base.GetParentPath(path, root).Replace("/", "\\");
            log.LogMethodEnd(nameof(GetParentPath), "result: {0}", result);
            return result;
        }

        // The MakePath method defines how your provider supports the Join-Path provider cmdlet. 
        // This cmdlet allows the user to combine a parent and child path segment to create a provider-internal path. 
        protected override string MakePath(string parent, string child)
        {
            log.LogMethodBegin(nameof(MakePath), "input: parent={0}, child={1}", parent, child);
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

            log.LogMethodEnd(nameof(MakePath), "result: {0}", result);
            return result;
        }

        // The NormalizeRelativePath method returns a path to an item that is relative to a specified base path. 
        protected override string NormalizeRelativePath(string path, string basePath)
        {
            log.LogMethodBegin(nameof(NormalizeRelativePath), "input: path={0}; basePath={1}", path, basePath);
            var result = base.NormalizeRelativePath(path, basePath);
            log.LogMethodEnd(nameof(NormalizeRelativePath), "result: {0}", result);
            return result;
        }

        /// <summary>
        ///     The GetChildName method gets the name property of the child item at the specified path.
        ///     If the item at the specified path is not a child of a container, then this method should return the path.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override string GetChildName(string path)
        {
            log.LogMethodBegin(nameof(GetChildName), "input: path={0}", path);
            var sftpPath = GetSftpLocalPath(path);
            var attr = DriveInfo.Client.GetAttributes(sftpPath);
            var result = path;
            if (!attr.IsDirectory)
            {
                result = sftpPath;
            }

            log.LogMethodEnd(nameof(GetChildName), "result: {0}", result);
            return result;
        }
    }
}