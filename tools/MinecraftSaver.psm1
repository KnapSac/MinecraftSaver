#Requires -Modules VSSetup

function Get-MsBuildLocation {
    #Get VS2017 MsBuild instance
    $vsInstallationInfo = Get-VSSetupInstance -All | where{$_.InstallationVersion -like "15.*"}
    $msBuildLocation = "$($vsInstallationInfo.InstallationPath)\MSBuild\15.0\Bin\msbuild.exe"

    if ($null -eq $vsInstallationInfo) {
        #Get VS2019 MsBuild instance
        $vsInstallationInfo = Get-VSSetupInstance -All | where{$_.InstallationVersion -like "16.*"}
        $msBuildLocation = "$($vsInstallationInfo.InstallationPath)\MSBuild\Current\Bin\msbuild.exe"
    }

    if ($null -eq $vsInstallationInfo) {
        Write-Host "Unable to find MsBuild instance"
        return $null
    }

    return $msBuildLocation
}

function Invoke-MinecraftSaverBuild {
    $msBuildLocation = Get-MsBuildLocation
    if ($null -eq $msBuildLocation) {
        Write-Host "Unable to build because no MsBuild instance was found"
        return
    }

    .$msBuildLocation "$PSScriptRoot\..\MinecraftSaver.sln" /m @args
}

function Invoke-MinecraftSaverRun {
    $debugExe = "$PSScriptRoot\..\src\MinecraftSaver\bin\Debug\MinecraftSaver.exe"

    .$debugExe @args
}

function Invoke-MinecraftSaverBuildAndRun {
    Invoke-MinecraftSaverBuild @args

    if (0 -eq $LastExitCode) {
        $windowWidth = (Get-Host).UI.RawUI.MaxWindowSize.Width
        Write-Host $("=" * $windowWidth)

        Invoke-MinecraftSaverRun
    } else {
        Write-Host "Build failed, not running exe"
    }
}

New-Alias -Name ms -Value Invoke-MinecraftSaverRun @args
New-Alias -Name build -Value Invoke-MinecraftSaverBuild @args
New-Alias -Name run -Value Invoke-MinecraftSaverBuildAndRun @args

Export-ModuleMember -Alias * -Function *
