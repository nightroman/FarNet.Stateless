
using Stateless.Reflection;

namespace FarNet.Stateless;

public class MetaMachine : IStateMachine
{
    private readonly IStateMachine _machine;

    public MetaMachine(object machine)
    {
        if (machine is null)
            throw new ArgumentNullException(nameof(machine));

        Type machineType = machine.GetType();
        if (machineType.Name != "StateMachine`2")
            throw new ArgumentException($"Invalid type, expected: 'StateMachine`2', actual: '{machineType}'.");

        Type[] arguments = machine.GetType().GetGenericArguments();

        Type generic = typeof(AStateMachine<,>);
        Type constructed = generic.MakeGenericType(arguments);

        var value = Activator.CreateInstance(constructed, machine);
        _machine = (IStateMachine)value;
    }

    public object State => _machine.State;

    public void Fire(object trigger) =>
        _machine.Fire(trigger);

    public void FireWithParameters(object triggerWithParameters, object[] values) =>
        _machine.FireWithParameters(triggerWithParameters, values);

    public StateMachineInfo GetInfo() =>
        _machine.GetInfo();

    public IReadOnlyList<MetaTrigger> GetPermittedTriggers() =>
        _machine.GetPermittedTriggers();
}
