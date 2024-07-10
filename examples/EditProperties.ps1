<#
.Synopsis
	Setting object properties interactively using the state machine loop.
#>

param(
	[object]$Properties = [pscustomobject]@{Name='Joe'; Age=42}
)

$ErrorActionPreference = 1
Import-Module FarNet.Stateless

$machine = [Stateless.StateMachine[string, string]]::new('')

$config = $machine.Configure('')
foreach($p in $Properties.PSObject.Properties) {
	$null = $machine.SetTriggerParameters($p.Name, [string])
	if ($p.IsSettable) {
		$null = $config.InternalTransition($p.Name, [System.Action[Stateless.StateMachine`2+Transition[string, string]]]{
			param($Transition)
			$Properties.PSObject.Properties[$Transition.Trigger].Value = $Transition.Parameters[0]
		})
	}
}

function Get-StatelessCaption {
	'Properties'
}

function Get-StatelessMessage {
	($Properties | Format-List | Out-String).Trim()
}

# edit properties, interactive loop
Invoke-StateMachine $machine -Caption {Get-StatelessCaption} -Message {Get-StatelessMessage}

# return result object on exit
$Properties
