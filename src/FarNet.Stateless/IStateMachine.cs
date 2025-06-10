
using Stateless.Reflection;

namespace FarNet.Stateless;

public interface IStateMachine
{
    object State { get; }

    StateMachineInfo GetInfo();

    IReadOnlyList<MetaTrigger> GetPermittedTriggers();

    void Fire(object trigger);

    void FireWithParameters(object triggerWithParameters, object[] values);
}
