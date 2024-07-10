<#
.Synopsis
	Setting/unsetting enum flags interactively using the state machine loop.
#>

param(
	[string]$State = 'Flag2'
)

$ErrorActionPreference = 1
Import-Module FarNet.Stateless

[Flags()]
enum MyFlags {
	None = 0
	Flag1 = 1
	Flag2 = 2
	Flag3 = 4
}

$machine = [Stateless.StateMachine[MyFlags, string]]([MyFlags]$State)

$values = [Enum]::GetValues([MyFlags])
$max = ($values | Measure-Object -Sum).Sum

for($s = 0; $s -le $max; ++$s) {
	$config = $machine.Configure($s)
	foreach($v in $values) {
		if ($v -ne [MyFlags]::None -and 0 -eq ($s -band [int]$v)) {
			$null = $config.Permit($v, ($s -bor [int]$v))
		}
	}
	foreach($v in $values) {
		if ($v -ne [MyFlags]::None -and 0 -ne ($s -band [int]$v)) {
			$null = $config.Permit("- $v", ($s -bxor [int]$v))
		}
	}
}

# edit flags, interactive loop
Invoke-StateMachine $machine -Show

# return result flags on exit
$machine.State
