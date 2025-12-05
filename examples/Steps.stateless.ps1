<#
.Synopsis
	State machine script for Invoke-Stateless or Invoke-StateMachine.

.Description
	Wizard like workflow with three steps.

.Example
	Invoke-Stateless Steps.stateless.ps1
#>

param(
	[string]$State = 'Step1'
)

$ErrorActionPreference=1
Import-Module FarNet.Stateless

$1 = 'Step1'
$2 = 'Step2'
$3 = 'Step3'

$machine = [Stateless.StateMachine[string, string]]::new($State)

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
$null = $config.Permit('Next', $2)
$null = $config.OnEntry($entry, 'entry')

$config = $machine.Configure($2)
$null = $config.Permit('Next', $3)
$null = $config.Permit('Back', $1)
$null = $config.OnEntry($entry, 'entry')

$machine
