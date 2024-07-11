<#
.Synopsis
	Runs Steps.stateless.ps1 by Invoke-StateMachine
#>

$machine = & $PSScriptRoot\Steps.stateless.ps1
Invoke-StateMachine $machine -Show
