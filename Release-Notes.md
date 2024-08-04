# FarNet.Stateless Release Notes
See also [Stateless/CHANGELOG](https://github.com/dotnet-state-machine/stateless/blob/dev/CHANGELOG.md)

## v0.1.1

Fix empty caption (should be the current state).

## v0.1.0

Changes:
- New parameter `Exit` (`Invoke-Stateless`, `Invoke-StateMachine`).
- New parameter `Output` (`Show-StateMachine`).
- Renamed `Show` -> `AddShow`, `Prompt` -> `AddPrompt`.

Fixes:
- Unwrap `PSObject` passed as state machine.
- Catch invalid parameter input and repeat.

## v0.0.3

New parameter `Prompt`.

## v0.0.2

Tweak prompts and graphs.

## v0.0.1

`Invoke-Stateless`, `Invoke-StateMachine`, `Show-StateMachine`.
