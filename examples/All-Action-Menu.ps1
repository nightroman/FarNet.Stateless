<#
.Synopsis
	Hierarchical menu of examples by groups: Tools, Simple, Persistent.

.Description
	This menu is a loop of actions "Invoke selected example".
	The menu resumes when the selected example exits.
#>

$ErrorActionPreference=1
Import-Module FarNet.Stateless

$machine = [Stateless.StateMachine[string, string]]::new('Main')

$config = $machine.Configure('Main')
$null = $config.Permit('Tools', 'Tools')
$null = $config.Permit('Simple', 'Simple')
$null = $config.Permit('Persistent', 'Persistent')

$config = $machine.Configure('Tools')
$null = $config.Permit('Back', 'Main')
$null = $config.InternalTransition('EditFlags.ps1', [System.Action]{& $PSScriptRoot\EditFlags.ps1})
$null = $config.InternalTransition('EditProperties.ps1', [System.Action]{& $PSScriptRoot\EditProperties.ps1})

$config = $machine.Configure('Simple')
$null = $config.Permit('Back', 'Main')
$null = $config.InternalTransition('Cart.ps1', [System.Action]{& $PSScriptRoot\Cart.ps1})
$null = $config.InternalTransition('OnOff.ps1', [System.Action]{& $PSScriptRoot\OnOff.ps1})
$null = $config.InternalTransition('Phone.ps1', [System.Action]{& $PSScriptRoot\Phone.ps1})
$null = $config.InternalTransition('Steps.ps1', [System.Action]{& $PSScriptRoot\Steps.ps1})

$config = $machine.Configure('Persistent')
$null = $config.Permit('Back', 'Main')
$null = $config.InternalTransition('Phone.stateless.ps1', [System.Action]{Invoke-Stateless $PSScriptRoot\Phone.stateless.ps1})
$null = $config.InternalTransition('Steps.stateless.ps1', [System.Action]{Invoke-Stateless $PSScriptRoot\Steps.stateless.ps1})

Invoke-StateMachine $machine -AddShow
