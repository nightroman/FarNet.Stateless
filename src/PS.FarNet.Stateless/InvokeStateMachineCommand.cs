using System.Management.Automation;

namespace PS.FarNet.Stateless;

[Cmdlet("Invoke", "StateMachine")]
public sealed class InvokeStateMachineCommand : BaseInvokeCmdlet
{
    [Parameter(Position = 0, Mandatory = true)]
    public object Machine { get; set; }

    [Parameter]
    public object Caption { get; set; }

    [Parameter]
    public object Message { get; set; }

    protected override void BeginProcessing()
    {
        InvokeStateMachine(Machine, Caption, Message, null, null);
    }
}
