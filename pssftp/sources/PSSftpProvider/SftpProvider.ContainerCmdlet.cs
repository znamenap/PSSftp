using System.Management.Automation;

namespace PSSftpProvider
{
    public partial class SftpCmdletProvider
    {
        // The ConvertPath method can be used by the provider class to create a new providerspecific path from a specified path. 
        protected override bool ConvertPath(string path, string filter, ref string updatedPath,
            ref string updatedFilter)
        {
            log.LogMethodBegin(nameof(ConvertPath), "input: path={0}; filter={1}, updatedPath={2}, updatedFilter={3}",
                path, filter, updatedPath, updatedFilter);

            updatedPath = GetSftpLocalPath(path);
            var result = true;

            log.LogMethodEnd(nameof(ConvertPath), "result: result={0}; updatedPath={1}, updatedFilter={2}",
                result, updatedPath, updatedFilter);
            return result;
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            log.LogMethodBegin(nameof(GetChildNames), "input: path={0}, retrunContainers={1}", path, returnContainers);
            base.GetChildNames(path, returnContainers);
            log.LogMethodEnd(nameof(GetChildNames), "void");
        }
    }
}