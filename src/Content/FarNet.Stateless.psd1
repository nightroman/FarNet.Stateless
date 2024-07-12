@{
	Author = 'Roman Kuzmin'
	ModuleVersion = '0.0.0'
	Description = 'Stateless library interactive helpers.'
	Copyright = 'Copyright (c) Roman Kuzmin'
	GUID = 'a750b52b-5acb-4238-aaca-ad914f82b727'

	PowerShellVersion = '5.1'
	RootModule = 'PS.FarNet.Stateless.dll'
	RequiredAssemblies = 'FarNet.Stateless.dll', 'Stateless.dll'

	FunctionsToExport = @()
	VariablesToExport = @()
	CmdletsToExport = @(
		'Invoke-Stateless'
		'Invoke-StateMachine'
		'Show-StateMachine'
	)

	PrivateData = @{
		PSData = @{
			Tags = 'Stateless', 'State', 'Machine', 'Workflow'
			ProjectUri = 'https://github.com/nightroman/FarNet.Stateless'
			LicenseUri = 'https://github.com/nightroman/FarNet.Stateless/blob/main/LICENSE'
			ReleaseNotes = 'https://github.com/nightroman/FarNet.Stateless/blob/main/Release-Notes.md'
		}
	}
}
