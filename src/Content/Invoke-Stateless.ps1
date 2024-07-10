[CmdletBinding()]
param(
	$File,
	$Parameters,
	$Action
)

function Export-StatelessData {}
function Import-StatelessData {}
function Get-StatelessCaption {}
function Get-StatelessMessage {}

$_ = $Parameters
Remove-Variable File, Parameters, Action -Scope 0
$PSBoundParameters.Action.Invoke((. $PSBoundParameters.File @_))
