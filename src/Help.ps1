Set-StrictMode -Version 3
Import-Module FarNet.Stateless

$_Machine = @'
		Specifies the state machine instance.
'@

### BaseInvoke
$BaseInvoke = @{
	parameters = @{
		Exit = @'
		Tells to exit the trigger choice loop when there are no triggers.
'@
		AddPrompt = @'
		Tells to add the "Prompt" choice. It enters the nested prompt.
		Type `exit` to exit the nested prompt and resume the loop.
'@
		AddShow = @'
		Tells to add the "Show" choice. It generates HTML with the state
		machine graph and opens the page by the associated program.
'@
	}
}

### Invoke-Stateless
Merge-Helps $BaseInvoke @{
	command = 'Invoke-Stateless'
	synopsis = 'Starts or resumes the Stateless script or checkpoint.'
	description = @'
	This command shows the prompt loop for choosing permitted triggers and
	stepping through the machine states. After each step the state checkpoint
	is saved to a file. The loop may be interrupted and resumed later from the
	checkpoint in the same or new session. Choosing "Exit" terminates the loop
	and deletes the checkpoint.

	The stateless script is specified by the parameter File. It is a PowerShell
	script with the following conventions:

	(1) It creates and returns the Stateless state machine.

	(2) It has the parameter State and uses it as the initial state of the
	machine. Choose and specify the appropriate default value as the initial
	state for the very first run.

	(3) It may have other parameters suitable for clixml serialization. They
	are automatically saved in the checkpoint file and restored and used on
	resuming.

	(4) Script functions Export-StatelessData and Import-StatelessData may be
	used for persisting extra data in addition to automatically persisted
	parameters.

		Export-StatelessData usually outputs one or more script scope variables
		designed and suitable for clixml serialization.

		Import-StatelessData accepts one parameter, one or more values to be
		restored as the same script scope variables.

	(5) Script functions Get-StatelessCaption and Get-StatelessMessage may be
	used for the custom prompt caption and message respectively.

	CHECKPOINTS

	Checkpoints are stored as $Home/FarNet.Stateless/<guid>.stateless.clixml
	The <guid> for a particular run is the MD5 hash created from the script
	path and the specified script parameters.

	PARAMETERS

	The Stateless script parameters, the required State and optional others,
	are exposed as the dynamic parameters of Invoke-Stateless and specified
	as if they are parameters of Invoke-Stateless:

		Invoke-Stateless My.stateless.ps1 -State Foo -MyParam Bar

	On the first run the parameters are used for the hash and for invoking the
	script. On next runs, if the checkpoint exists, the values are taken from
	the checkpoint, different if the script changes them on state transitions.
'@
	parameters = @{
		File = @'
		Specifies the Stateless script or checkpoint file.
'@
	}
}

### Invoke-StateMachine
Merge-Helps $BaseInvoke @{
	command = 'Invoke-StateMachine'
	synopsis = 'Invokes the state machine interactive loop.'
	description = @'
	This command shows the prompt loop for choosing permitted triggers and
	stepping through the machine states. Choosing "Exit" terminates the loop.
'@
	parameters = @{
		Machine = $_Machine
		Caption = @'
		Specifies the prompt caption as script block or value.
		Default: current state.
'@
		Message = @'
		Specifies the prompt message as script block or value.
		Default: empty string.
'@
	}
}

### Show-StateMachine
@{
	command = 'Show-StateMachine'
	synopsis = 'Shows the state machine graph in a generated HTML page.'
	description = @'
	This command generates an HTML page with the state machine graph and opens
	it by the associated program.

	The page uses "viz-standalone.js" for converting DOT to SVG, see
	https://github.com/mdaines/viz.js

	If "viz-standalone.js" is found in the path then it is used by the page.
	Otherwise, the online version of this script is used.
'@
	parameters = @{
		Machine = $_Machine
		Output = @'
		Tells to output the generated DOT-graph to the specified file instead
		of generating and showing HTML with it.
'@
	}
}
