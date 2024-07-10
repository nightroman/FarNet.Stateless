<#
.Synopsis
	Simple on/off example with external state storage.
#>

$ErrorActionPreference = 1
Import-Module FarNet.Stateless

$on = 'On'
$off = 'Off'
$trigger = 'Switch'

# how to use the external state storage in scripts
$state = $off
$machine = [Stateless.StateMachine[string, string]]::new(
	{$state},
	{$Script:state = $args[0]}
)

# how to define the action script block
$entry = [Action[Stateless.StateMachine`2+Transition[string, string]]]{
	param($Transition)
	Write-Host "$($Transition.Trigger): $($Transition.Source) -> $($Transition.Destination)"
}

$null = $machine.Configure($off).Permit($trigger, $on).OnEntry($entry)
$null = $machine.Configure($on).Permit($trigger, $off).OnEntry($entry)

Invoke-StateMachine $machine -Show
