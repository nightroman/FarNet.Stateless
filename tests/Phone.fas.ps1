Import-Module $PSScriptRoot/About.psm1

### LeftMessage
Use-PhoneStart
keys 3
job {
	Assert-Far $Far.Dialog[0].Text -eq 'OffHook'
	Assert-Far ($Far.Dialog[3].Text -match 'Joe .*?\d\d\d\d.*? - .*?\d\d\d\d')
}
Use-Exit

### Destroyed
Use-PhoneStart
keys 4
job {
	Assert-Far $Far.Dialog[0].Text -eq 'OnHold'
}
keys 2
job {
	Assert-Far $Far.Dialog.Focused.Text -eq '&2. HurlAgainstWall'
}
keys Enter
job {
	Assert-Far $Far.Dialog[0].Text -eq 'Destroyed'
	Assert-Far ($Far.Dialog[3].Text -match 'Joe .*?\d\d\d\d.*? - .*?\d\d\d\d')
}
Use-Exit

### OnHook
Use-PhoneStart
keys 5
job {
	Assert-Far $Far.Dialog[0].Text -eq 'OnHook'
}
job {
	Assert-Far $Far.Dialog[0].Text -eq 'OnHook'
	Assert-Far ($Far.Dialog[3].Text -match 'Joe .*?\d\d\d\d.*? - .*?\d\d\d\d')
}
Use-Exit
