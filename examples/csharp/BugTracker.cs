// Based on https://github.com/dotnet-state-machine/stateless/tree/dev/example/BugTrackerExample

using Stateless;
using System;

namespace Extra;

public class BugTracker
{
    private enum State { Open, Assigned, Deferred, Closed }

    private enum Trigger { Assign, Defer, Close }

    private readonly StateMachine<State, Trigger> _machine;
    public object Machine => _machine;

    private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _assignTrigger;

    private readonly string _title;
    private string _assignee;
    private readonly Action<string> _log;

    public BugTracker(string title, Action<string> log)
    {
        _title = title;
        _log = log;

        _machine = new StateMachine<State, Trigger>(State.Open);

        _assignTrigger = _machine.SetTriggerParameters<string>(Trigger.Assign);

        _machine.Configure(State.Open)
            .Permit(Trigger.Assign, State.Assigned);

        _machine.Configure(State.Assigned)
            .SubstateOf(State.Open)
            .OnEntryFrom(_assignTrigger, OnAssigned)
            .PermitReentry(Trigger.Assign)
            .Permit(Trigger.Close, State.Closed)
            .Permit(Trigger.Defer, State.Deferred)
            .OnExit(OnDeassigned);

        _machine.Configure(State.Deferred)
            .OnEntry(() => _assignee = null)
            .Permit(Trigger.Assign, State.Assigned);
    }

    private void OnAssigned(string assignee)
    {
        if (_assignee != null && assignee != _assignee)
            SendEmailToAssignee("OnAssigned: Don't forget to help the new employee!");

        _assignee = assignee;
        SendEmailToAssignee("OnAssigned: You own it.");
    }

    private void OnDeassigned()
    {
        SendEmailToAssignee("OnDeassigned: You're off the hook.");
    }

    private void SendEmailToAssignee(string message)
    {
        _log($"{_assignee}, RE {_title}: {message}");
    }
}
