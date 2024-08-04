
Import-Module $PSScriptRoot/About.psm1

$root = "$PSScriptRoot\..\examples"
Test-StateMachineScript "$root\All-Action-Menu.ps1"
Test-StateMachineScript "$root\All-Choice-Menu.ps1"
Test-StateMachineScript "$root\EditProperties.ps1"
Test-StateMachineScript "$root\OnOff.ps1"
Test-StateMachineScript "$root\Steps.ps1"
