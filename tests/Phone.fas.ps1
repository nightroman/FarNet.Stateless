Import-Module $PSScriptRoot/About.psm1

### LeftMessage
Use-PhoneStart
keys 3 Enter
job {
	Assert-Far $__[0].Text -eq 'OffHook'
	Assert-Far ($__[3].Text -match 'Joe .*?\d\d\d\d.*? - .*?\d\d\d\d')
}
keys 0 Enter

### Destroyed
Use-PhoneStart
keys 4 Enter
job {
	Assert-Far $__[0].Text -eq 'OnHold'
}
keys 2
job {
	Assert-Far $__[4].Text -eq '&2. HurlAgainstWall'
}
keys Enter
job {
	Assert-Far $__[0].Text -eq 'Destroyed'
	Assert-Far ($__[3].Text -match 'Joe .*?\d\d\d\d.*? - .*?\d\d\d\d')
}
keys 0 Enter

### OnHook
Use-PhoneStart
keys 5 Enter
job {
	Assert-Far $__[0].Text -eq 'OnHook'
}
job {
	Assert-Far $__[0].Text -eq 'OnHook'
	Assert-Far ($__[3].Text -match 'Joe .*?\d\d\d\d.*? - .*?\d\d\d\d')
}
keys 0 Enter
