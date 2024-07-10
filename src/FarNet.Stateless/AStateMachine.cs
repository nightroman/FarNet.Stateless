using Stateless;
using Stateless.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace FarNet.Stateless;

class AStateMachine<TState, TTrigger>(StateMachine<TState, TTrigger> machine) : IStateMachine
{
    private readonly StateMachine<TState, TTrigger> _machine = machine;

    public object State => _machine.State;

    public void Fire(object trigger)
    {
        _machine.Fire((TTrigger)trigger);
    }

    public void FireWithParameters(object triggerWithParameters, object[] values)
    {
        _machine.Fire((StateMachine<TState, TTrigger>.TriggerWithParameters)triggerWithParameters, values);
    }

    public StateMachineInfo GetInfo()
    {
        return _machine.GetInfo();
    }

    public IReadOnlyList<MetaTrigger> GetPermittedTriggers()
    {
        var triggers = _machine.GetDetailedPermittedTriggers();

        List<MetaTrigger> infos = [];
        foreach (var trigger in triggers)
        {
            var triggerWithParameters = trigger.Parameters;
            infos.Add(new(
                trigger.Trigger,
                triggerWithParameters,
                trigger.HasParameters ? triggerWithParameters.ArgumentTypes.ToArray() : []));
        }
        return infos;
    }
}
