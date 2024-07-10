
param(
	$Configuration = (property Configuration Release),
	$FarHome = (property FarHome C:\Bin\Far\x64)
)

$ModuleName = 'FarNet.Stateless'
$ModuleRoot = "$FarHome\FarNet\Lib\$ModuleName"
$Description = 'Stateless library interactive helpers.'

task build meta, {
	Set-Location src
	exec { dotnet build -c $Configuration }
}

task clean {
	remove src\*\bin, src\*\obj, README.htm, *.nupkg, z
}

task publish {
	Set-Location $ModuleRoot
	remove *.json

	$v1 = (Select-Xml '//PackageReference[@Include="Stateless"]' "$PSScriptRoot\src\$ModuleName\$ModuleName.csproj").Node.Version
	Copy-Item -Destination $ModuleRoot $(
		"$HOME\.nuget\packages\Stateless\$v1\lib\netstandard2.0\Stateless.dll"
		"$HOME\.nuget\packages\Stateless\$v1\lib\netstandard2.0\Stateless.xml"
	)
}

task content -After publish {
	exec { robocopy src\Content $ModuleRoot } (0..3)
}

task help {
	. Helps.ps1
	Convert-Helps src\Help.ps1 $ModuleRoot\PS.FarNet.Stateless.dll-Help.xml
}

task version {
	($Script:Version = switch -Regex -File Release-Notes.md {'##\s+v(\d+\.\d+\.\d+)' {$Matches[1]; break} })
}

task markdown version, {
	assert (Test-Path $env:MarkdownCss)
	exec { pandoc.exe @(
		'README.md'
		'--output=README.htm'
		'--from=gfm'
		'--embed-resources'
		'--standalone'
		"--css=$env:MarkdownCss"
		"--metadata=pagetitle=$ModuleName $Version"
	)}
}

task meta -Inputs .build.ps1, Release-Notes.md -Outputs src\Directory.Build.props -Jobs version, {
	Set-Content src\Directory.Build.props @"
<Project>
	<PropertyGroup>
		<Company>https://github.com/nightroman/$ModuleName</Company>
		<Copyright>Copyright (c) Roman Kuzmin</Copyright>
		<Description>$Description</Description>
		<Product>$ModuleName</Product>
		<Version>$Version</Version>
		<IncludeSourceRevisionInInformationalVersion>False</IncludeSourceRevisionInInformationalVersion>
	</PropertyGroup>
</Project>
"@
}

task package help, markdown, version, {
	remove z
	$Script:PSPackageRoot = mkdir "z\tools\FarHome\FarNet\Lib\$ModuleName"

	exec { robocopy $ModuleRoot $PSPackageRoot /s /xf *.pdb } 1

	Copy-Item -Destination z @(
		'README.md'
	)

	Copy-Item -Destination $PSPackageRoot @(
		"README.htm"
		"LICENSE"
	)

	Import-Module PsdKit
	$xml = Import-PsdXml $PSPackageRoot\$ModuleName.psd1
	Set-Psd $xml $Version 'Data/Table/Item[@Key="ModuleVersion"]'
	Export-PsdXml $PSPackageRoot\$ModuleName.psd1 $xml

	$result = Get-ChildItem $PSPackageRoot -Recurse -File -Name | Out-String
	$sample = @'
about_FarNet.Stateless.help.txt
FarNet.Stateless.dll
FarNet.Stateless.ini
FarNet.Stateless.psd1
Invoke-Stateless.ps1
LICENSE
PS.FarNet.Stateless.dll
PS.FarNet.Stateless.dll-Help.xml
README.htm
Stateless.dll
Stateless.xml
'@
	Assert-SameFile.ps1 -Text $sample $result $env:MERGE
}

task nuget package, version, {
	equals $Version (Get-Item "$ModuleRoot\$ModuleName.dll").VersionInfo.ProductVersion

	Set-Content z\Package.nuspec @"
<?xml version="1.0"?>
<package xmlns="http://schemas.microsoft.com/packaging/2010/07/nuspec.xsd">
	<metadata>
		<id>$ModuleName</id>
		<version>$Version</version>
		<authors>Roman Kuzmin</authors>
		<owners>Roman Kuzmin</owners>
		<license type="expression">MIT</license>
		<readme>README.md</readme>
		<projectUrl>https://github.com/nightroman/$ModuleName</projectUrl>
		<description>$Description</description>
		<releaseNotes>https://github.com/nightroman/$ModuleName/blob/main/Release-Notes.md</releaseNotes>
		<tags>Stateless State Machine</tags>
	</metadata>
</package>
"@

	exec { NuGet.exe pack z\Package.nuspec -NoPackageAnalysis }
}

task pushNuGet nuget, version, {
	$NuGetApiKey = Read-Host NuGetApiKey
	exec { nuget push "$ModuleName.$Version.nupkg" -Source nuget.org -ApiKey $NuGetApiKey }
}

task pushPSGallery package, {
	$NuGetApiKey = Read-Host NuGetApiKey
	Publish-Module -Path $PSPackageRoot -NuGetApiKey $NuGetApiKey
}

task push pushNuGet, pushPSGallery, clean

task . build, clean