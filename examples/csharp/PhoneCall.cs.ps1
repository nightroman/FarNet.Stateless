#requires -Version 7
$ErrorActionPreference=1
Import-Module FarNet.Stateless
Add-Type -Path $PSScriptRoot\PhoneCall.cs -ReferencedAssemblies netstandard, Stateless

$test = [Extra.PhoneCall]::new({Write-Host $args})

Invoke-StateMachine $test.Machine -AddShow
