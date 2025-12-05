Import-Module FarNet.Stateless

function Test-StateMachineScript($ScriptPath) {
	$data.ScriptPath = $ScriptPath
	run {
		& $data.ScriptPath
	}
	job {
		Assert-Far -Dialog
	}
	keys 0 Enter
	job {
		Assert-Far -Panels
	}
}

function Use-PhoneStart {
	run {
		& $PSScriptRoot\..\examples\Phone.ps1
	}
	job {
		Assert-Far $__[0].Text -eq 'OnHook'
	}

	keys Enter
	job {
		Assert-Far $__[0].Text -eq 'OffHook'
	}

	keys Enter
	job {
		Assert-Far $__[1].Text -eq 'Dial [0]'
	}

	keys J o e Enter
	job {
		Assert-Far $__[0].Text -eq 'Ringing'
		Assert-Far $__[3].Text -eq 'Call   : Joe  -'
	}

	keys Enter
	job {
		Assert-Far $__[0].Text -eq 'Connected'
		Assert-Far ($__[3].Text -match 'Joe .*?\d\d\d\d.*? -$')
	}

	keys Enter
	job {
		Assert-Far $__[2].Text -eq 'Muted  : True'
	}

	keys Enter
	job {
		Assert-Far $__[2].Text -eq 'Muted  : False'
	}

	keys 2 Enter
	job {
		Assert-Far $__[1].Text -eq 'SetVolume [0]'
	}

	keys 1 2 Enter
	job {
		Assert-Far $__[1].Text -eq 'Volume : 12'
	}
}
