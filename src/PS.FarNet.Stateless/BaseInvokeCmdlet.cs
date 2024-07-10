﻿using FarNet.Stateless;
using System;
using System.Collections.ObjectModel;
using System.Management.Automation;
using System.Management.Automation.Host;

namespace PS.FarNet.Stateless;

public abstract class BaseInvokeCmdlet : PSCmdlet
{
    // '0' and 's' excluded
    const string HotKeys = "123456789abcdefghijklmnopqrtuvwxyz";

    [Parameter]
    public SwitchParameter Show { get; set; }

    protected MetaMachine Helper { get; private set; }

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

            var captionText = caption.ToBaseObject() switch
            {
                ScriptBlock script => script.InvokeReturnAsIs()?.ToString(),
                string text => text,
                _ => null
            };
            if (string.IsNullOrEmpty(captionText))
                captionText = $"State: {Helper.State}";

            var messageText = message.ToBaseObject() switch
            {
                ScriptBlock script => script.InvokeReturnAsIs()?.ToString(),
                string text => text,
                _ => null
            };
            if (string.IsNullOrEmpty(messageText))
                messageText = "Select trigger";

            Collection<ChoiceDescription> choices = [];
            int n = -1;
            foreach (var trigger in triggers)
            {
                ++n;
                var choice = new ChoiceDescription($"&{HotKeys[n]}. {trigger.Trigger}") { HelpMessage = trigger.Trigger.ToString() };
                choices.Add(choice);
            }

            choices.Add(new ChoiceDescription("&0. Exit") { HelpMessage = "Exit" });

            if (Show)
                choices.Add(new ChoiceDescription("&Show") { HelpMessage = "Show the state machine graph." });

            var index = Host.UI.PromptForChoice(captionText, messageText, choices, 0);

            if (Show && index == choices.Count - 1)
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
