<#
.Synopsis
	State machine script for Invoke-Stateless or Invoke-StateMachine.

.Description
	This example is based on https://github.com/dotnet-state-machine/stateless/tree/dev/example/TelephoneCallExample

	Script parameters are automatically persisted by Invoke-Stateless.
	Script extra data are persisted by Export/Import-StatelessData.
	Custom prompt uses Get-StatelessCaption, Get-StatelessMessage.

.Parameter State
		Specifies the state, usually as string, with some default value.
		This parameter is required for Invoke-Stateless.

.Parameter Volume
		Specifies the volume.

.Example
	Invoke-Stateless Phone.stateless.ps1

.Example
	Invoke-StateMachine (& ./Phone.stateless.ps1)
#>

param(
	[string]$State = 'OnHook',
	[int]$Volume = 10
)

$ErrorActionPreference = 1
Import-Module FarNet.Stateless

enum PhoneState {
	OnHook
	OffHook
	Ringing
	Connected
	OnHold
	Destroyed
}

enum PhoneTrigger {
	OnHook
	OffHook
	Dial
	CallConnected
	LeftMessage
	OnHold
	OffHold
	HurlAgainstWall
	SetVolume
	MuteMicrophone
	UnmuteMicrophone
}

# These data are exported / imported.
$Callee = ''
$Muted = $false
$StartedAt = $null
$EndedAt = $null

# Exports some data in addition to automatically exported parameters.
function Export-StatelessData {
	$Script:Callee, $Script:Muted, $Script:StartedAt, $Script:EndedAt
}

# Imports data, the argument is the output of Export-StatelessData.
function Import-StatelessData($Data) {
	$Script:Callee, $Script:Muted, $Script:StartedAt, $Script:EndedAt = $Data
}

# Gets the custom prompt caption.
function Get-StatelessCaption {
	$machine.State
}

# Gets the custom prompt message.
function Get-StatelessMessage {
	@"
Volume : $Volume
Muted  : $Muted
Call   : $Callee $StartedAt - $EndedAt
"@
}

# Create the state machine using the provided state parameter.
$machine = [Stateless.StateMachine[PhoneState, PhoneTrigger]]::new($State)

$setCalleeTrigger = $machine.SetTriggerParameters([PhoneTrigger]::Dial, [string])
$setVolumeTrigger = $machine.SetTriggerParameters([PhoneTrigger]::SetVolume, [int])

### OnHook
$config = $machine.Configure([PhoneState]::OnHook)
$null = $config.Permit([PhoneTrigger]::OffHook, [PhoneState]::OffHook)

### OffHook
$config = $machine.Configure([PhoneState]::OffHook)
$null = $config.Permit([PhoneTrigger]::Dial, [PhoneState]::Ringing)
$null = $config.Permit([PhoneTrigger]::OnHook, [PhoneState]::OnHook)

### Ringing
$config = $machine.Configure([PhoneState]::Ringing)
$null = $config.Permit([PhoneTrigger]::CallConnected, [PhoneState]::Connected)
$null = $config.OnEntryFrom(
	$setCalleeTrigger,
	{
		param($Transition)
		$newCallee = $Transition.Parameters[0]
		if ($newCallee) {
			$Script:Callee = $newCallee
		}
		$Script:StartedAt = $Script:EndedAt = $null
	},
	"Number to call"
)

### Connected
$config = $machine.Configure([PhoneState]::Connected)
$null = $config.InternalTransitionIf([PhoneTrigger]::MuteMicrophone, {!$Muted}, [System.Action]{
	$Script:Muted = $true
})
$null = $config.InternalTransitionIf([PhoneTrigger]::UnmuteMicrophone, {$Muted}, [System.Action]{
	$Script:Muted = $false
})
$null = $config.InternalTransition([PhoneTrigger]::SetVolume, [System.Action[Stateless.StateMachine`2+Transition[PhoneState, PhoneTrigger]]]{
	param($Transition)
	$Script:Volume = $Transition.Parameters[0]
})
$null = $config.Permit([PhoneTrigger]::LeftMessage, [PhoneState]::OffHook)
$null = $config.Permit([PhoneTrigger]::OnHold, [PhoneState]::OnHold)
$null = $config.Permit([PhoneTrigger]::OnHook, [PhoneState]::OnHook)
$null = $config.OnEntry([System.Action]{
	$Script:StartedAt = Get-Date
})
$null = $config.OnExit([System.Action]{
	$Script:EndedAt = Get-Date
})

### OnHold
$config = $machine.Configure([PhoneState]::OnHold)
$null = $config.SubstateOf([PhoneState]::Connected)
$null = $config.Permit([PhoneTrigger]::OffHold, [PhoneState]::Connected)
$null = $config.Permit([PhoneTrigger]::HurlAgainstWall, [PhoneState]::Destroyed)

# result
$machine
