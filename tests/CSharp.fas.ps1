
Import-Module $PSScriptRoot/About.psm1

$root = "$PSScriptRoot\..\examples\csharp"
if (!(Test-Path $root\bin\Debug\net8.0\Extra.dll)) {
	$r = dotnet build $root\Extra.csproj || $(throw $r)
}

foreach($item in Get-Item $root\*.cs) {
	$baseName = [System.IO.Path]::GetFileNameWithoutExtension($item.Name)
	Test-StateMachineScript "$root\$baseName.cs.ps1"
	Test-StateMachineScript "$root\$baseName.dll.ps1"
}
