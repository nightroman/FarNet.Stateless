# FarNet.Stateless

Interactive workflows using [dotnet-state-machine/stateless](https://github.com/dotnet-state-machine/stateless).

**Packages**
- PowerShell 5.1 and 7.x module, PSGallery [FarNet.Stateless](https://www.powershellgallery.com/packages/FarNet.Stateless)
- FarNet library, NuGet [FarNet.Stateless](https://www.nuget.org/packages/FarNet.Stateless)

**Use cases**
- Interactive workflows using PowerShell host UI.
- Persistent workflows using checkpoint files.
- Testing and visualizing state machines.
- Interactive input of complex data.
- Hierarchical action/choice menus.

## Examples

See [FarNet.Stateless/examples](https://github.com/nightroman/FarNet.Stateless/tree/main/examples)

[Phone.stateless.ps1](https://github.com/nightroman/FarNet.Stateless/blob/main/examples/Phone.stateless.ps1) example with interactive loop in PowerShell:\
(`OnHook` -> `OffHook` -> `Dial` (+ prompt for number) -> `Ringing` -> `Connected` -> ...)

![image](https://github.com/nightroman/FarNet/assets/927533/0fa4bc5e-7e69-4f37-aee7-36d8f710251f)

## PowerShell module

You may install the PowerShell module by this command:

```powershell
Install-Module -Name FarNet.Stateless
```

Explore, see also [about_FarNet.Stateless.help.txt](https://github.com/nightroman/FarNet.Stateless/blob/main/src/Content/about_FarNet.Stateless.help.txt):

```powershell
# import and get module commands
Import-Module -Name FarNet.Stateless
Get-Command -Module FarNet.Stateless

# get module and commands help
help about_FarNet.Stateless
help Invoke-Stateless
help Invoke-StateMachine
help Show-StateMachine
```

## FarNet library

To install as the FarNet library `FarNet.Stateless`, follow [these steps](https://github.com/nightroman/FarNet#readme).\
See [PowerShell FarNet modules](https://github.com/nightroman/FarNet/wiki/PowerShell-FarNet-modules) for details.

## See also

- [FarNet.Stateless Release Notes](https://github.com/nightroman/FarNet.Stateless/blob/main/Release-Notes.md)
- [FarNet.Stateless examples](https://github.com/nightroman/FarNet.Stateless/tree/main/examples)
- [The Stateless library](https://github.com/dotnet-state-machine/stateless)
