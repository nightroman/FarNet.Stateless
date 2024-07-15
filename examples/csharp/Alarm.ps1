Import-Module FarNet.Stateless
Add-Type -Path $PSScriptRoot\bin\Debug\net8.0\Extra.dll

$test = [Extra.Alarm]::new(10, 10, 10, 10, {Write-Host $args})

Invoke-StateMachine $test.Machine -AddShow
