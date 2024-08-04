# Extra examples

These PowerShell examples use Stateless state machines created in .NET assemblies.

## Using C# file

Scripts `*.cs.ps1` use dynamic assemblies built from the `*.cs` files.

These scripts are ready to use right away.

## Using assembly

Scripts `*.dll.ps1` use the existing assembly.

Before using the scripts, build the assembly:

    dotnet build

Then invoke example scripts.
