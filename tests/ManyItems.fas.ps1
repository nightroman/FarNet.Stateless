Import-Module $PSScriptRoot/About.psm1

run {
	$machine = [Stateless.StateMachine[string, string]]::new('Main')

	$config = $machine.Configure('Main')
	(1..40).ForEach{
		$item = "Item$_"
		$null = $config.Permit($item, $item)
	}

	Invoke-StateMachine $machine -AddShow -Exit
	Assert-Far $machine.State -eq Item10
}

keys a Enter
