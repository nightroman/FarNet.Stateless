#requires -Version 7
$ErrorActionPreference=1
Import-Module FarNet.Stateless
Add-Type -Path $PSScriptRoot\Alarm.cs -ReferencedAssemblies netstandard, Stateless, System.ComponentModel.Primitives, System.ComponentModel.TypeConverter

$test = [Extra.Alarm]::new(10, 10, 10, 10, {Write-Host $args})

Invoke-StateMachine $test.Machine -AddShow
