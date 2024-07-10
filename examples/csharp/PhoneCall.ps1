Import-Module FarNet.Stateless
Add-Type -Path $PSScriptRoot\bin\Debug\net8.0\Extra.dll

$test = [Extra.PhoneCall]::new({Write-Host $args})

Invoke-StateMachine $test.Machine -Show
