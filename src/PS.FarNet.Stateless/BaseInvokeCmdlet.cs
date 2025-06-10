
using FarNet.Stateless;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace PS.FarNet.Stateless;

public abstract class BaseInvokeCmdlet : PSCmdlet
{
    // '0', 'p', 's' excluded
    const string HotKeys = "123456789abcdefghijklmnoqrtuvwxyz";

    [Parameter]
    public SwitchParameter AddPrompt { get; set; }

    [Parameter]
    public SwitchParameter AddShow { get; set; }

    [Parameter]
    public SwitchParameter Exit { get; set; }

    private static readonly ScriptBlock ReadHost = ScriptBlock.Create("Read-Host $args[0]");

    protected MetaMachine Helper { get; private set; }

    private static string GetText(object value, object defaultValue)
    {
        value = value.ToBaseObject();
        if (value is null)
            return defaultValue.ToString();

        if (value is string text)
            return text;

        if (value is not ScriptBlock script)
            return LanguagePrimitives.ConvertTo<string>(value);

        var res = script.Invoke();
        return res.Count switch
        {
            0 => defaultValue.ToString(),
            1 => LanguagePrimitives.ConvertTo<string>(res[0]),
            _ => string.Join(Environment.NewLine, res.Select(LanguagePrimitives.ConvertTo<string>)),
        };
    }

    protected void InvokeStateMachine(
        object machine,
        object caption,
        object message,
        Action fired,
        Action exited)
    {
        Helper = new MetaMachine(machine.ToBaseObject());

        while (true)
        {
            var triggers = Helper.GetPermittedTriggers();
            if (Exit && triggers.Count == 0)
            {
                exited?.Invoke();
                break;
            }

            var state1 = Helper.State;
            var captionText = GetText(caption, Helper.State);
            var messageText = GetText(message, string.Empty);

            Collection<ChoiceDescription> choices = [];
            int n = -1;
            foreach (var trigger in triggers)
            {
                ++n;
                var name = trigger.Trigger.ToString();
                var label = n < HotKeys.Length ? $"&{HotKeys[n]}. {name}" : name;
                var choice = new ChoiceDescription(label) { HelpMessage = name };
                choices.Add(choice);
            }

            int indexExit = choices.Count;
            choices.Add(new ChoiceDescription("&0. Exit") { HelpMessage = "Exit" });

            int indexPrompt = -1;
            if (AddPrompt)
            {
                indexPrompt = choices.Count;
                choices.Add(new ChoiceDescription("&Prompt") { HelpMessage = "Enter nested prompt." });
            }

            int indexShow = -1;
            if (AddShow)
            {
                indexShow = choices.Count;
                choices.Add(new ChoiceDescription("&Show") { HelpMessage = "Show state graph." });
            }

            var index = Host.UI.PromptForChoice(captionText, messageText, choices, 0);

            if (AddPrompt && index == indexPrompt)
            {
                Host.EnterNestedPrompt();
                continue;
            }

            if (AddShow && index == indexShow)
            {
                ScriptBlock.Create("Show-StateMachine $args[0]").Invoke(machine);
                continue;
            }

            // exit?
            if (index == indexExit)
            {
                exited?.Invoke();
                break;
            }

            if (index < 0 || index >= triggers.Count)
                break;

            // state changed?
            if (!Equals(Helper.State, state1))
                continue;

            // fire trigger
            var selectedTrigger = triggers[index];
            if (selectedTrigger.ParameterTypes.Count == 0)
            {
                Helper.Fire(selectedTrigger.Trigger);
            }
            else
            {
                var values = new object[selectedTrigger.ParameterTypes.Count];
                for (int i = 0; i < values.Length; i++)
                {
                    var prompt = $"{selectedTrigger.Trigger} [{i}]";
                    while(true)
                    {
                        try
                        {
                            var value = ReadHost.InvokeReturnAsIs(prompt);
                            values[i] = LanguagePrimitives.ConvertTo(value, selectedTrigger.ParameterTypes[i]);
                            break;
                        }
                        catch (PSInvalidCastException)
                        {
                            continue;
                        }
                    }
                }

                // state changed?
                if (!Equals(Helper.State, state1))
                    continue;

                Helper.FireWithParameters(selectedTrigger.TriggerWithParameters, values);
            }

            // call fired
            fired?.Invoke();
        }
    }
}
