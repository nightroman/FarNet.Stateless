<#
.Synopsis
	Runs Three.stateless.ps1 by Invoke-StateMachine
#>

$machine = & $PSScriptRoot\Three.stateless.ps1
Invoke-StateMachine $machine -Caption {$machine.State} -Message three-step-demo -Show
