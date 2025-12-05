# Cart.ps1 with nested prompt.

Import-Module $PSScriptRoot/About.psm1

run {
	& $PSScriptRoot\..\examples\Cart.ps1
}
job {
	Assert-Far -Dialog
	Assert-Far $__[0].Text -eq Draft
	Assert-Far $__[1].Text -eq 'Cart items: 0'
	Assert-Far $__[2].Text -eq '&1. AddItem (default)'
}

keys p Enter
job {
	Assert-Far -Editor
	Assert-Far $__[0].Text -eq '# Draft> '

	$__.Line.Text = '$Items, $Note = 42, "test-note"'
}

keys ShiftEnter
job {
	Assert-Far -Editor
	Assert-Far $__[-4].Text -eq '# Draft> '
}

keys Esc
job {
	Assert-Far -Dialog
	Assert-Far $__[0].Text -eq Draft
	Assert-Far $__[1].Text -eq 'Cart items: 42'
	Assert-Far $__[2].Text -eq 'test-note'
}

keys 0 Enter
