<#
.Synopsis
	State machine script for Invoke-Stateless or Invoke-StateMachine.

.Description
	Wizard like workflow with three steps.

.Example
	Invoke-Stateless Three.stateless.ps1

.Example
	Invoke-StateMachine (& ./Three.stateless.ps1)
#>

param(
	$State = 'step1'
)

$ErrorActionPreference = 1
Import-Module FarNet.Stateless

$1 = 'step1'
$2 = 'step2'
$3 = 'step3'

$machine = [Stateless.StateMachine[string, string]]($State)

$machine.OnTransitioned({
	param($Transition)
	Write-Host OnTransitioned $Transition.Trigger $Transition.Source $Transition.Destination
})

$machine.OnTransitionCompleted({
	param($Transition)
	Write-Host OnTransitionCompleted $Transition.Trigger $Transition.Source $Transition.Destination
})

$entry = [Action[Stateless.StateMachine`2+Transition[string, string]]]{
	param([Stateless.StateMachine`2+Transition[string, string]]$Transition)
	Write-Host OnEntry $Transition.Trigger $Transition.Source $Transition.Destination
}

$config = $machine.Configure($1)
$null = $config.Permit('next', $2)
$null = $config.OnEntry($entry, 'entry')

$config = $machine.Configure($2)
$null = $config.Permit('next', $3)
$null = $config.Permit('back', $1)
$null = $config.OnEntry($entry, 'entry')

$machine
