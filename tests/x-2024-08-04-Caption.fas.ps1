# Fixed empty caption when custom returns nothing.

Import-Module $PSScriptRoot/About.psm1

### custom returns nothing, then the state is used, the default
run {
	$machine = [Stateless.StateMachine[string, string]]::new('state1')
	Invoke-StateMachine $machine -Caption {} -Message {}
}
job {
	Assert-Far -Dialog
	Assert-Far $Far.Dialog[0].Text -eq state1
	Assert-Far $Far.Dialog[1].Text -eq ''

	$Far.Dialog.Close()
}

### custom returns empty, then empty is used
run {
	$machine = [Stateless.StateMachine[string, string]]::new('state1')
	Invoke-StateMachine $machine -Caption {''} -Message {''}
}
job {
	Assert-Far -Dialog
	Assert-Far $Far.Dialog[0].Text -eq ''
	Assert-Far $Far.Dialog[1].Text -eq ''

	$Far.Dialog.Close()
}
