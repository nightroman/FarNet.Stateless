
param(
	[string]$State = 'Draft',
	[string]$Note = '',
	[int]$Items = 0
)

$ErrorActionPreference = 1
Import-Module FarNet.Stateless

enum CartState {
	Cart
	Draft
	Saved
	Deleted
	Purchased
}

enum CartTrigger {
	Note
	AddItem
	RemoveItem
	SaveCart
	EditCart
	DeleteCart
	PurchaseCart
}

$machine = [Stateless.StateMachine[CartState, CartTrigger]]::new($State)

$setNote = $machine.SetTriggerParameters([CartTrigger]::Note, [string])

### Cart
$config = $machine.Configure([CartState]::Cart)
$null = $config.InternalTransition([CartTrigger]::Note, [System.Action[Stateless.StateMachine`2+Transition[CartState, CartTrigger]]]{
	param($Transition)
	$Script:Note = $Transition.Parameters[0]
})

### Draft
$config = $machine.Configure([CartState]::Draft)
$null = $config.SubstateOf([CartState]::Cart)
$null = $config.InternalTransition([CartTrigger]::AddItem, [System.Action]{++$Script:Items})
$null = $config.InternalTransitionIf([CartTrigger]::RemoveItem, {$Items -ge 1}, [System.Action]{--$Script:Items})
$null = $config.Permit([CartTrigger]::SaveCart, [CartState]::Saved)
$null = $config.Permit([CartTrigger]::DeleteCart, [CartState]::Deleted)

### Saved
$config = $machine.Configure([CartState]::Saved)
$null = $config.SubstateOf([CartState]::Cart)
$null = $config.Permit([CartTrigger]::EditCart, [CartState]::Draft)
$null = $config.Permit([CartTrigger]::DeleteCart, [CartState]::Deleted)
$null = $config.Permit([CartTrigger]::PurchaseCart, [CartState]::Purchased)

function Get-StatelessMessage {
	"Cart items: $Items"
	$Note
}

Invoke-StateMachine $machine -Show -Message {Get-StatelessMessage}
