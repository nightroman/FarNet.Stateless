<#
.Synopsis
	Hierarchical menu of examples by groups: Tools, Simple, Persistent.

.Description
	This menu exits when an example is selected.
	Then the selected example is invoked.
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
$null = $config.Permit('EditFlags.ps1', 'EditFlags.ps1')
$null = $config.Permit('EditProperties.ps1', 'EditProperties.ps1')

$config = $machine.Configure('Simple')
$null = $config.Permit('Back', 'Main')
$null = $config.Permit('Cart.ps1', 'Cart.ps1')
$null = $config.Permit('OnOff.ps1', 'OnOff.ps1')
$null = $config.Permit('Phone.ps1', 'Phone.ps1')
$null = $config.Permit('Steps.ps1', 'Steps.ps1')

$config = $machine.Configure('Persistent')
$null = $config.Permit('Back', 'Main')
$null = $config.Permit('Phone.stateless.ps1', 'Phone.stateless.ps1')
$null = $config.Permit('Steps.stateless.ps1', 'Steps.stateless.ps1')

# Use -Exit in order to exit on any terminal state.
Invoke-StateMachine $machine -AddShow -Exit

# Process the result state, invoke an example.
$result = $machine.State
if ($result -like '*.stateless.ps1') {
	Invoke-Stateless $PSScriptRoot\$result
}
elseif ($result -like '*.ps1') {
	& $PSScriptRoot\$result
}
