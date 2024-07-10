using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Security.Cryptography;
using System.Text;
using IOFile = System.IO.File;

namespace PS.FarNet.Stateless;

[Cmdlet("Invoke", "Stateless")]
public sealed class InvokeStatelessCommand : BaseInvokeCmdlet, IDynamicParameters
{
    const string KeyFile = "File";
    const string KeyData = "Data";
    const string KeyState = "State";
    const string KeyParameters = "Parameters";

    static readonly ScriptBlock InvokeRunner = ScriptBlock.Create($"& $args[0] $args[1] $args[2] $args[3]");
    static readonly ScriptBlock ExportScript = ScriptBlock.Create("Export-StatelessData");
    static readonly ScriptBlock ImportScript = ScriptBlock.Create("Import-StatelessData $args[0]");
    static readonly ScriptBlock CaptionScript = ScriptBlock.Create("Get-StatelessCaption");
    static readonly ScriptBlock MessageScript = ScriptBlock.Create("Get-StatelessMessage");

    static readonly new string[] CommonParameters = ["Verbose", "Debug", "ErrorAction", "WarningAction", "ErrorVariable", "WarningVariable", "OutVariable", "OutBuffer", "PipelineVariable", "InformationAction", "InformationVariable", "ProgressAction"];
    static readonly string[] ReservedParameters = [nameof(File), nameof(Show)];

    [Parameter(Position = 0, Mandatory = true)]
    public string File { get; set; }

    bool _import;
    object _data;
    string _clixmlFile;
    Hashtable _scriptParameters;
    RuntimeDefinedParameterDictionary _dynamicParameters;

    public object GetDynamicParameters()
    {
        if (string.IsNullOrEmpty(File))
            throw new PSArgumentException("Specify required parameter 'File'.", nameof(File));

        File = GetUnresolvedProviderPathFromPSPath(File);
        if (!IOFile.Exists(File))
            throw new PSArgumentException($"File does not exist: {File}", nameof(File));

        if (!Path.GetExtension(File).Equals(".ps1", StringComparison.OrdinalIgnoreCase))
            return null;

        _dynamicParameters = [];

        var command = InvokeCommand.GetCommand(File, CommandTypes.ExternalScript);
        if (!command.Parameters.ContainsKey(KeyState))
            throw new PSInvalidOperationException($"Stateless script requires parameter '{KeyState}'.");

        foreach (var parameter in command.Parameters.Values)
        {
            if (CommonParameters.Contains(parameter.Name))
                continue;

            if (ReservedParameters.Contains(parameter.Name))
                throw new PSInvalidOperationException($"Stateless script uses reserved parameter '{parameter.Name}'.");

            foreach (var attribute in parameter.Attributes)
            {
                if (attribute is ParameterAttribute pa && pa.Position >= 0)
                    pa.Position += 1;
            }

            _dynamicParameters.Add(parameter.Name, new RuntimeDefinedParameter(parameter.Name, parameter.ParameterType, parameter.Attributes));
        }

        return _dynamicParameters;
    }

    protected override void BeginProcessing()
    {
        if (_dynamicParameters is null)
        {
            _import = true;
            _clixmlFile = File;
        }
        else
        {
            using var writer = new StringWriter();
            writer.WriteLine(File);

            _scriptParameters = [];
            var savedParameters = new Hashtable();
            foreach (var parameter in _dynamicParameters.Values)
            {
                savedParameters.Add(parameter.Name, parameter.Value);
                if (parameter.IsSet)
                {
                    _scriptParameters.Add(parameter.Name, parameter.Value);
                    writer.WriteLine(parameter.Name);
                    writer.WriteLine(parameter.Value);
                }
            }

            var guid = new Guid(MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(writer.ToString())));

            var clixmlRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "FarNet.Stateless");
            Directory.CreateDirectory(clixmlRoot);

            _clixmlFile = Path.Combine(clixmlRoot, $"{guid}.stateless.clixml");
            _import = IOFile.Exists(_clixmlFile);
        }

        if (_import)
        {
            var data = new PSObject(PSSerializer.Deserialize(IOFile.ReadAllText(_clixmlFile)));
            File = (string)data.Properties[KeyFile].Value;
            _scriptParameters = (Hashtable)data.Properties[KeyParameters].Value.ToBaseObject();
            _data = data.Properties[KeyData].Value;
        }

        string runner = Path.Combine(Path.GetDirectoryName(typeof(InvokeStatelessCommand).Assembly.Location), "Invoke-Stateless.ps1");
        try
        {
            InvokeRunner.Invoke(runner, File, _scriptParameters, new Action<object>(InvokeStateMachine));
        }
        catch (CmdletInvocationException ex)
        {
            if (ex.InnerException is MethodInvocationException ex2 && ex2.InnerException is PSInvalidOperationException ex3)
                throw ex3;

            throw;
        }
    }

    void InvokeStateMachine(object machine)
    {
        machine = machine.ToBaseObject();
        if (machine is null)
            throw new PSInvalidOperationException("Stateless script must return the state machine but returns null.");

        if (machine.GetType().Name != "StateMachine`2")
            throw new PSInvalidOperationException($"Stateless script must return the state machine but returns {machine.GetType()}.");

        if (_import)
            ImportScript.Invoke(_data);

        InvokeStateMachine(machine, CaptionScript, MessageScript, Fired, Exited);
    }

    void Exited()
    {
        IOFile.Delete(_clixmlFile);
    }

    void Fired()
    {
        // make parameters to export
        Hashtable newParameters = [];
        var names = _dynamicParameters is { } ? _dynamicParameters.Keys : _scriptParameters.Keys;
        foreach (string name in names)
        {
            // get state from the machine
            if (name.Equals(KeyState, StringComparison.OrdinalIgnoreCase))
            {
                newParameters.Add(KeyState, Helper.State.ToString());
                continue;
            }

            // get current variable value
            var value = GetVariableValue(name);

            // export switch as bool
            if (LanguagePrimitives.TryConvertTo<SwitchParameter>(value, out var sp))
            {
                newParameters.Add(name, sp.IsPresent);
                continue;
            }

            // export other types as is
            newParameters.Add(name, value);
        }

        // get custom data to export
        var raw = ExportScript.Invoke();
        object data = raw.Count == 0 ? null : raw.Count == 1 ? raw[0] : raw.Cast<object>().ToArray();

        // serialize
        var pso = new PSObject();
        pso.Properties.Add(new PSNoteProperty(KeyFile, File));
        pso.Properties.Add(new PSNoteProperty(KeyParameters, newParameters));
        pso.Properties.Add(new PSNoteProperty(KeyData, data));
        var text = PSSerializer.Serialize(pso);

        // save to file
        IOFile.WriteAllText(_clixmlFile, text);
    }
}
