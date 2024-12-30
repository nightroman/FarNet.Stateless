using System.Management.Automation;

namespace PS.FarNet.Stateless;

static class ExtensionMethods
{
    public static object ToBaseObject(this object value)
    {
        return value is PSObject ps ? ps.BaseObject : value;
    }
}
