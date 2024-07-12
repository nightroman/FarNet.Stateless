using FarNet.Stateless;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace PS.FarNet.Stateless;

public abstract class BaseInvokeCmdlet : PSCmdlet
{
    // '0', 'p', 's' excluded
    const string HotKeys = "123456789abcdefghijklmnoqrtuvwxyz";

    [Parameter]
    public SwitchParameter Prompt { get; set; }

    [Parameter]
    public SwitchParameter Show { get; set; }

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
            0 => string.Empty,
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
        Helper = new MetaMachine(machine);

        while (true)
        {
            var state1 = Helper.State;
            var triggers = Helper.GetPermittedTriggers();

            var captionText = GetText(caption, Helper.State);
            var messageText = GetText(message, string.Empty);

            Collection<ChoiceDescription> choices = [];
            int n = -1;
            foreach (var trigger in triggers)
            {
                ++n;
                var choice = new ChoiceDescription($"&{HotKeys[n]}. {trigger.Trigger}") { HelpMessage = trigger.Trigger.ToString() };
                choices.Add(choice);
            }

            int indexExit = choices.Count;
            choices.Add(new ChoiceDescription("&0. Exit") { HelpMessage = "Exit" });

            int indexPrompt = -1;
            if (Prompt)
            {
                indexPrompt = choices.Count;
                choices.Add(new ChoiceDescription("&Prompt") { HelpMessage = "Enter nested prompt." });
            }

            int indexShow = -1;
            if (Show)
            {
                indexShow = choices.Count;
                choices.Add(new ChoiceDescription("&Show") { HelpMessage = "Show state graph." });
            }

            var index = Host.UI.PromptForChoice(captionText, messageText, choices, 0);

            if (Prompt && index == indexPrompt)
            {
                Host.EnterNestedPrompt();
                continue;
            }

            if (Show && index == indexShow)
            {
                ScriptBlock.Create("Show-StateMachine $args[0]").Invoke(machine);
                continue;
            }

            // exit?
            if (index == triggers.Count)
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
                    var value = ScriptBlock.Create("Read-Host $args[0]").InvokeReturnAsIs(prompt);
                    value = LanguagePrimitives.ConvertTo(value, selectedTrigger.ParameterTypes[i]);
                    values[i] = value;
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
