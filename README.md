[Stateless]: https://github.com/dotnet-state-machine/stateless

# FarNet.Stateless

Interactive workflows using dotnet-state-machine/stateless, PowerShell module and FarNet library.

The package provides interactive helpers for [Stateless] state machines.\
Machines may be created in PowerShell scripts or in imported assemblies.

Packages:
- PowerShell 5.1+ module, PSGallery [FarNet.Stateless](https://www.powershellgallery.com/packages/FarNet.Stateless)
- FarNet library, NuGet [FarNet.Stateless](https://www.nuget.org/packages/FarNet.Stateless)

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
The [NuGet package](https://www.nuget.org/packages/FarNet.Stateless) is installed to `%FARHOME%\FarNet\Lib\FarNet.Stateless`.

The PowerShell module may be imported as:

```powershell
Import-Module $env:FARHOME\FarNet\Lib\FarNet.Stateless
```

**Expose the module as a symbolic link or junction**

Consider exposing this module, so that you can:

```powershell
Import-Module FarNet.Stateless
```

(1) Choose one of the module directories, see `$env:PSModulePath`.

(2) Change to the selected directory and create the symbolic link

```powershell
New-Item FarNet.Stateless -ItemType SymbolicLink -Value $env:FARHOME\FarNet\Lib\FarNet.Stateless
```

(3) Alternatively, you may create the similar folder junction point in Far
Manager using `AltF6`.

Then you may update the FarNet package with new versions. The symbolic link or
junction do not have to be updated, they point to the same location.

## See also

- [FarNet.Stateless Release Notes](https://github.com/nightroman/FarNet.Stateless/blob/main/Release-Notes.md)
- [FarNet.Stateless examples](https://github.com/nightroman/FarNet.Stateless/tree/main/examples)
- [The Stateless library](https://github.com/dotnet-state-machine/stateless)
