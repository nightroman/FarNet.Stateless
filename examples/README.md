# PowerShell examples

Scripts `*.stateless.ps1` are for `Invoke-Stateless`. They are interactive
prompt loops with checkpoints. The flow of stepping through states may be
interrupted and resumed later in the same or new session.

Example:

```powershell
Invoke-Stateless Phone.stateless.ps1
```

Other scripts use `Invoke-StateMachine` and may be invoked directly. They are
simple interactive prompt loops, not persistent.

Example:

```powershell
.\Phone.ps1
```
