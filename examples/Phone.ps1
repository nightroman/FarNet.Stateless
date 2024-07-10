<#
.Synopsis
	Runs Phone.stateless.ps1 by Invoke-StateMachine
#>

$machine = & $PSScriptRoot\Phone.stateless.ps1
Invoke-StateMachine $machine -Show
