properties { 
  
  $signAssemblies = $false
  $signKeyPath = ""
  $buildDocumentation = $false
  $buildNuGet = $true
  $uploadNuGet = $true
  $treatWarningsAsErrors = $false
  
  $baseDir  = resolve-path ..
  $buildDir = "$baseDir\build"
  $sourceDir = "$baseDir\src"
  $toolsDir = "$baseDir\tools"
  $docDir = "$baseDir\doc"
  $releaseDir = "$baseDir\release"
  $workingDir = "$baseDir\dist"
  
  $ns = $project
  $builds = @()
  
  if (!$ns) {
	$builds = @(
		@{
			Root = "EntityFramework.BulkInsert\"; 
			Name = "EntityFramework.BulkInsert.csproj"; 
			TestsName = "EntityFramework.BulkInsert.Test"; 
			Constants="NET45;EF6"; 
			FinalDir="Net45"; 
			NuGetDir = "net45"; 
			Framework="net-4.5"; 
			NS="EntityFramework.BulkInsert";
			Sign=$true
		},
		@{
			Root = "EntityFramework.BulkInsert.SqlServerCe\"; 
			Name = "EntityFramework.BulkInsert.SqlServerCe.csproj"; 
			TestsName = "EntityFramework.BulkInsert.Test"; 
			Constants="NET45;EF6"; 
			FinalDir="Net45"; 
			NuGetDir = "net45"; 
			Framework="net-4.5"; 
			NS="EntityFramework.BulkInsert.SqlServerCe";
			Sign=$true
		},
		@{
			Root = "EntityFramework.BulkInsert.MySql\"; 
			Name = "EntityFramework.BulkInsert.MySql.csproj"; 
			TestsName = "EntityFramework.BulkInsert.Test"; 
			Constants="NET45;EF6"; 
			FinalDir="Net45"; 
			NuGetDir = "net45"; 
			Framework="net-4.5"; 
			NS="EntityFramework.BulkInsert.MySql";
			Sign=$true
		}
	)
  }
  else {
	$builds = @(
		@{
			Root = "$ns\"; 
			Name = "$ns.csproj"; 
			TestsName = "EntityFramework.BulkInsert.Test"; 
			Constants="NET45;EF6"; 
			FinalDir="Net45"; 
			NuGetDir = "net45"; 
			Framework="net-4.5"; 
			NS="$ns";
			Sign=$true;
		}
	)
  }  
}


task default -depends Test

# Validate properties
task Validate {

	foreach ($build in $builds)
	{
		Assert ("EntityFramework.BulkInsert", "EntityFramework.BulkInsert.MySql", "EntityFramework.BulkInsert.SqlServerCe" -contains $build.NS) "Please provide a valid project parameter"
	}
}

# Ensure a clean working directory
task Clean -depends Validate {
  Set-Location $baseDir
  
  if (Test-Path -path $workingDir)
  {
    Write-Output "Deleting Working Directory"
    
    del $workingDir -Recurse -Force
  }
  
  New-Item -Path $workingDir -ItemType Directory
}

# Build each solution, optionally signed
task Build -depends Clean { 
		
	Write-Host -ForegroundColor Green "Updating assembly version"
	
	#$assemblyInfoCs = "$sourceDir\$ns\Properties\AssemblyInfo.cs"
	#Write-Host $assemblyInfoCs
	
	Update-AssemblyInfoFiles "$sourceDir\Common"
  
	foreach ($build in $builds)
	{
		$name = $build.Name
		$finalDir = $build.FinalDir
		$sign = ($build.Sign -and $signAssemblies)

		Write-Host -ForegroundColor Green "Building " $name
		Write-Host -ForegroundColor Green "Signed " $sign
		Write-Host
		exec { msbuild "/t:Clean;Rebuild" /p:Configuration=Release /p:DebugSymbols=true "/p:Platform=Any CPU" /p:OutputPath=bin\Release\$finalDir\ /p:AssemblyOriginatorKeyFile=$signKeyPath "/p:SignAssembly=$sign" "/p:TreatWarningsAsErrors=$treatWarningsAsErrors" "/p:Optimize=true" (GetConstants $build.Constants $sign) ($sourceDir + "\" + $build.Root + $build.Name) | Out-Default } "Error building $name"
	}
}

# Optional build documentation, add files to final zip
task Package -depends Build {
  
  #foreach ($build in $builds)
  #{
  #  $name = $build.TestsName
#	$root = $build.Root
 #   $finalDir = $build.FinalDir
    
#	robocopy "$sourceDir\$root\bin\Release\$finalDir" $workingDir\Package\Bin\$build.NS\$finalDir "$build.NS.*" /NP /XO /XF *.pri | Out-Default
#  }
  
  if ($buildNuGet)
  {
    New-Item -Path $workingDir\NuGet -ItemType Directory
	    
    foreach ($build in $builds)
    {
	  $buildNS = $build.NS
	  New-Item -Path $workingDir\NuGet\$buildNS -ItemType Directory
	  Copy-Item -Path "$buildDir\$buildNS.nuspec" -Destination $workingDir\NuGet\$buildNS\$buildNS.nuspec -recurse
		
      if ($build.NuGetDir -ne $null)
      {
        $name = $build.TestsName
		$root = $build.Root
        $finalDir = $build.FinalDir
        $frameworkDirs = $build.NuGetDir.Split(",")
        
        foreach ($frameworkDir in $frameworkDirs)
        {
          robocopy "$sourceDir\$root\bin\Release\$finalDir" $workingDir\NuGet\$buildNS\lib\$frameworkDir "$buildNS.*" /NP /XO /XF *.pri | Out-Default
        }
      }
	  
	  $currentVersion = GetVersion $workingDir\NuGet\$buildNS\lib\net45\$buildNS.dll
	  
	  exec { .\Tools\NuGet\NuGet.exe pack $workingDir\NuGet\$buildNS\$buildNS.nuspec -Symbols -version $currentVersion }
	  move -Path .\*.nupkg -Destination $workingDir\NuGet\$buildNS
	  
    }
  
	#$currentVersion = GetVersion $workingDir\NuGet\lib\net45\$ns.dll
  
    #exec { .\Tools\NuGet\NuGet.exe pack $workingDir\NuGet\$ns.nuspec -Symbols -version $currentVersion }
    #move -Path .\*.nupkg -Destination $workingDir\NuGet
  }
}

# Unzip package to a location
task Deploy -depends Package {
  echo deploy
}

# Run tests on deployed files
task Test -depends Deploy {
  echo test
}


function GetConstants($constants, $includeSigned)
{
  $signed = switch($includeSigned) { $true { ";SIGNED" } default { "" } }

  return "/p:DefineConstants=`"CODE_ANALYSIS;TRACE;$constants$signed`""
}

function GetVersion([string] $file)
{
	return [System.Diagnostics.FileVersionInfo]::GetVersionInfo($file).FileVersion
}

function Update-AssemblyInfoFiles ([string] $sourceDir)
{
    $assemblyVersionPattern = 'AssemblyVersion\("(.+)(\.([0-9]+)){1,3}"\)'
    $fileVersionPattern = 'AssemblyFileVersion\("(.+)(\.([0-9]+)){1,3}"\)'
    
    Get-ChildItem -Path $sourceDir -r -filter CommonAssemblyInfo.cs | ForEach-Object {
        
        $filename = $_.Directory.ToString() + '\' + $_.Name
        Write-Host $filename
        $filename + ' -> ' + $version
    
        (Get-Content $filename) | ForEach-Object {
		
			$vesion = ""
			if ($_ -match $assemblyVersionPattern)
			{
				$majorVersion = $matches[1]
				$minorVersion = [int]$matches[3] + 1;
				$version = ($majorVersion + "." + $minorVersion)
			}
			$assemblyVersion = 'AssemblyVersion("' + $version + '")';
			$fileVersion = 'AssemblyFileVersion("' + $version + '")';
		
            % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
            % {$_ -replace $fileVersionPattern, $fileVersion }
        } | Set-Content $filename -encoding UTF8
    }
}