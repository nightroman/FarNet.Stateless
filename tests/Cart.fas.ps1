# Cart.ps1 with nested prompt.

Import-Module $PSScriptRoot/About.psm1

run {
	& $PSScriptRoot\..\examples\Cart.ps1
}
job {
	Assert-Far -Dialog
	Assert-Far $Far.Dialog[0].Text -eq Draft
	Assert-Far $Far.Dialog[1].Text -eq 'Cart items: 0'
	Assert-Far $Far.Dialog[2].Text -eq ''
}

keys p
job {
	Assert-Far -Editor
	Assert-Far $Far.Editor[0].Text -eq '# Draft>'

	$Far.Editor.Line.Text = '$Items, $Note = 42, "test-note"'
}

keys ShiftEnter
job {
	Assert-Far -Editor
	Assert-Far $Far.Editor[-4].Text -eq '# Draft>'
}

keys Esc
job {
	Assert-Far -Dialog
	Assert-Far $Far.Dialog[0].Text -eq Draft
	Assert-Far $Far.Dialog[1].Text -eq 'Cart items: 42'
	Assert-Far $Far.Dialog[2].Text -eq 'test-note'
}

keys 0
