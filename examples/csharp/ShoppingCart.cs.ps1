#requires -Version 7
$ErrorActionPreference=1
Import-Module FarNet.Stateless
Add-Type -Path $PSScriptRoot\ShoppingCart.cs -ReferencedAssemblies netstandard, Stateless, System.Collections, System.Linq

$test = [Extra.ShoppingCart]::new({Write-Host $args})

Invoke-StateMachine $test.Machine -AddShow
