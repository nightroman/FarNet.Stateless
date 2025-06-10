
namespace FarNet.Stateless;

public class MetaTrigger(object trigger, object triggerWithParameters, IReadOnlyList<Type> parameterTypes)
{
    public object Trigger => trigger;

    public object TriggerWithParameters => triggerWithParameters;

    public IReadOnlyList<Type> ParameterTypes => parameterTypes;
}
