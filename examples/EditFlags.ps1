<#
.Synopsis
	Setting/unsetting enum flags interactively using state machine loop.
#>

param(
	[type]$Enum = [System.ConsoleModifiers],
	[object]$Value = 0
)

$ErrorActionPreference=1
Import-Module FarNet.Stateless

$machine = New-Object "Stateless.StateMachine[$Enum, string]" $Value

$values = [Enum]::GetValues($Enum)
$max = ($values | Measure-Object -Sum).Sum

for($s = 0; $s -le $max; ++$s) {
	$config = $machine.Configure($s)
	foreach($v in $values) {
		if ($v -ne 0 -and 0 -eq ($s -band [int]$v)) {
			$null = $config.Permit($v, ($s -bor [int]$v))
		}
	}
	foreach($v in $values) {
		if ($v -ne 0 -and 0 -ne ($s -band [int]$v)) {
			$null = $config.Permit("- $v", ($s -bxor [int]$v))
		}
	}
}

# edit flags, interactive loop
Invoke-StateMachine $machine -AddShow

# return result flags on exit
$machine.State
