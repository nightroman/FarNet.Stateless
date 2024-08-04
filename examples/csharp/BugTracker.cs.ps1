#requires -Version 7
$ErrorActionPreference = 1
Import-Module FarNet.Stateless
Add-Type -Path $PSScriptRoot\BugTracker.cs -ReferencedAssemblies netstandard, Stateless

$test = [Extra.BugTracker]::new('The bug', {Write-Host $args})

Invoke-StateMachine $test.Machine -AddShow
