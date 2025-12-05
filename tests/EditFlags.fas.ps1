#! Covers unwrapping PSObject machine created by New-Object.

Import-Module $PSScriptRoot/About.psm1

run {
	$r = & $PSScriptRoot\..\examples\EditFlags.ps1
	Assert-Far "$r" -eq 'Alt, Control'
}

job {
	Assert-Far $__[0].Text -eq None
}

keys Enter
job {
	Assert-Far $__[0].Text -eq Alt
}

keys Enter
job {
	Assert-Far $__[0].Text -eq 'Alt, Shift'
}

keys Enter
job {
	Assert-Far $__[0].Text -eq 'Alt, Shift, Control'
}

keys 2 Enter
job {
	Assert-Far $__[0].Text -eq 'Alt, Control'
}

keys 0 Enter
