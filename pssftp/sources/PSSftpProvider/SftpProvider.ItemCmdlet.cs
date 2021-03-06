﻿using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace PSSftpProvider
{
    public partial class SftpCmdletProvider
    {
        // The ItemExists method is called by the PowerShell engine when a user calls the 
        // T:Microsoft.PowerShell.Commands.Test-Path cmdlet. 
        // The method determines whether there is an item at the specified path.
        protected override bool ItemExists(string path)
        {
            Logger.LogMethodBegin(nameof(ItemExists), "input: path={0}", path);
            var sftpPath = GetSftpLocalPath(path);
            var result = false;
            if (WildcardPattern.ContainsWildcardCharacters(sftpPath))
            {
                //var any = DriveInfo.Client.ListDirectory(sftpPath).Any(x => (x.Name != "." && x.Name != ".."));
            }
            else
            {
                result = DriveInfo.Client.Exists(sftpPath);
            }

            Logger.LogMethodEnd(nameof(ItemExists), "result: {0}", result);
            return result;
        }

        protected override bool HasChildItems(string path)
        {
            Logger.LogMethodBegin(nameof(HasChildItems), "input: path={0}", path);
            var sftpPath = GetSftpLocalPath(path);
            var result = DriveInfo.Client.ListDirectory(sftpPath).Any(x => x.Name != "." && x.Name != "..");
            Logger.LogMethodEnd(nameof(HasChildItems), "result: {0}", result);
            return result;
        }

        protected override bool IsItemContainer(string path)
        {
            Logger.LogMethodBegin(nameof(IsItemContainer), "input: path={0}", path);
            var sftpPath = GetSftpLocalPath(path);
            var attrs = DriveInfo.Client.GetAttributes(sftpPath);
            var result = attrs.IsDirectory;
            Logger.LogMethodEnd(nameof(IsItemContainer), "result: {0}", result);
            return result;
        }

        protected override void GetItem(string path)
        {
            Logger.LogMethodBegin(nameof(IsItemContainer), "input: path={0}", path);
            var sftpPath = GetSftpLocalPath(path);
            foreach (var item in DriveInfo.Client.ListDirectory(sftpPath))
            {
                Logger.LogDebug("WriteItemObject: type={0}, name={1}, isContainer={2}", item.GetType().Name, item.Name,
                    item.IsDirectory);
                WriteItemObject(item, item.Name, item.IsDirectory);
            }

            Logger.LogMethodEnd(nameof(GetItem), "void");
        }

        protected override void GetChildItems(string path, bool recurse)
        {
            Logger.LogMethodBegin(nameof(GetChildItems), "input: path={0}, recurse={1}", path, recurse);
            var directory = new Stack<string>(new[] {path});
            while (directory.Count > 0)
            {
                var stepPath = GetSftpLocalPath(directory.Pop());
                foreach (var item in DriveInfo.Client.ListDirectory(stepPath))
                {
                    if (recurse && item.IsDirectory && item.Name != "." && item.Name != "..")
                    {
                        directory.Push(item.FullName);
                    }

                    var itemName = item.FullName.Replace("/", "\\");
                    Logger.LogDebug("WriteItemObject: type={0}, name={1}, isContainer={2}", item.GetType().Name, itemName,
                        item.IsDirectory);
                    WriteItemObject(item, itemName, item.IsDirectory);
                }
            }

            Logger.LogMethodEnd(nameof(GetChildItems), "void(2)");
        }

        protected override void GetChildItems(string path, bool recurse, uint depth)
        {
            Logger.LogMethodBegin(nameof(GetChildItems), "input: path={0}, recurse={1}, depth={2}", path, recurse, depth);
            base.GetChildItems(path, recurse, depth);
            Logger.LogMethodEnd(nameof(GetChildItems), "void(3)");
        }

        // The IsValidPath is used to determine if a path is syntactically and semantically valid for the provider.
        protected override bool IsValidPath(string path)
        {
            Logger.LogMethodBegin(nameof(IsValidPath), "input: path={0}", path);
            var result = true;
            Logger.LogMethodEnd(nameof(IsValidPath), "result: {0}", result);
            return result;
        }

        // The ExpandPath method allows the user to use wildcards when specifying the provider path.
        protected override string[] ExpandPath(string path)
        {
            Logger.LogMethodBegin(nameof(ExpandPath), "input: path={0}", path);
            var sftpPath = GetSftpLocalPath(path);
            var result = DriveInfo.Client.ListDirectory(sftpPath)
                .Where(x => x.Name != "." && x.Name != "..")
                .Select(x => x.Name)
                .ToArray();
            Logger.LogMethodEnd(nameof(ExpandPath), "result: {0}", string.Join(", ", result));
            return result;
        }
    }
}