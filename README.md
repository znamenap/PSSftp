# PSSFTP

## PowerShell Provider for SFTP connected drives.

### Motivation

Do you connect to SFTP servers and check several files daily? Personally, I don't. 
Although, I do use it occasianly, I want to be sure, I'm able to work with SFTP quickly and effectively. 
I wasn't happy on using other GUI based tools with lot's of settings and combinations which never actually helped me to do the job.
When I found the feature of having a PSDrive connected into PowerShell session, I just realized that's it, that'd give me the ability to script,
speed to check and power to workout my goal using the object pipeline based power of this tool.
Though, I started searching for already existing solutions of SFTP support in PowerShell. Guess what, I failed, have found some partial solution, but complete PSDrive implementation.   

### Feature Requirements

- *First of all, I need to be able to do this:*

    ```New-PSDrive -Name edu -PSProvider SFtp -Root sftp://node1.echoservice.io/cellular5 -Credential $c```

- *secondly, I want to do also this:*

    ```Get-ChildItem -Path edu:\net3  -Filter update*.scn | Where-Object { $_.LastWriteTime -gt (Get-Date).AddDays(-1) }```

- *and following on to do somthing like this:*

    ```Copy-Item edu:\net3\update1.scn edu:\net.bak\update1.scn```
    
    ```Copy-Item edu:\net3\update1.scn c:\temp\```

- *plus, I want to be able to delete or rename the files aswell*

- *and after all, I want to work with it as it was a standard file system attached at my machine, so I can also do this:*

    ```Move-Item -Force c:\temp\update1.scn edu:\net3```

