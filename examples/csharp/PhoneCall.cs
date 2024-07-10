// Based on https://github.com/dotnet-state-machine/stateless/tree/dev/example/TelephoneCallExample

using Stateless;
using System;

namespace Extra;

public class PhoneCall
{
    private enum Trigger
    {
        CallDialed,
        CallConnected,
        LeftMessage,
        PlacedOnHold,
        TakenOffHold,
        PhoneHurledAgainstWall,
        MuteMicrophone,
        UnmuteMicrophone,
        SetVolume
    }

    private enum State
    {
        OffHook,
        Ringing,
        Connected,
        OnHold,
        PhoneDestroyed
    }

    private State _state = State.OffHook;

    private readonly StateMachine<State, Trigger> _machine;
    public object Machine => _machine;

    private readonly StateMachine<State, Trigger>.TriggerWithParameters<string> _setCalleeTrigger;
    private readonly StateMachine<State, Trigger>.TriggerWithParameters<int> _setVolumeTrigger;

    private readonly Action<string> _log;

    string _callee;

    public PhoneCall(Action<string> log)
    {
        _log = log;

        _machine = new StateMachine<State, Trigger>(() => _state, s => _state = s);

        _setCalleeTrigger = _machine.SetTriggerParameters<string>(Trigger.CallDialed);
        _setVolumeTrigger = _machine.SetTriggerParameters<int>(Trigger.SetVolume);

        _machine.Configure(State.OffHook)
            .Permit(Trigger.CallDialed, State.Ringing);

        _machine.Configure(State.Ringing)
            .OnEntryFrom(_setCalleeTrigger, OnDialed, "Caller number to call")
            .Permit(Trigger.CallConnected, State.Connected);

        _machine.Configure(State.Connected)
            .OnEntry(t => StartCallTimer(), "StartCallTimer")
            .OnExit(t => StopCallTimer(), "StopCallTimer")
            .InternalTransition(Trigger.MuteMicrophone, t => OnMute())
            .InternalTransition(Trigger.UnmuteMicrophone, t => OnUnmute())
            .InternalTransition(_setVolumeTrigger, (volume, t) => OnSetVolume(volume))
            .Permit(Trigger.LeftMessage, State.OffHook)
            .Permit(Trigger.PlacedOnHold, State.OnHold);

        _machine.Configure(State.OnHold)
            .SubstateOf(State.Connected)
            .Permit(Trigger.TakenOffHold, State.Connected)
            .Permit(Trigger.PhoneHurledAgainstWall, State.PhoneDestroyed);

        _machine.OnTransitioned(t => _log($"OnTransitioned: {t.Source} -> {t.Destination} via {t.Trigger}({string.Join(", ", t.Parameters)})"));
    }

    private void OnSetVolume(int volume)
    {
        _log("Volume set to " + volume + "!");
    }

    private void OnUnmute()
    {
        _log("Microphone unmuted!");
    }

    private void OnMute()
    {
        _log("Microphone muted!");
    }

    private void OnDialed(string callee)
    {
        _callee = callee;
        _log($"[Phone Call] placed for : [{_callee}]");
    }

    private void StartCallTimer()
    {
        _log($"[Timer:] Call started at {DateTime.Now}");
    }

    private void StopCallTimer()
    {
        _log($"[Timer:] Call ended at {DateTime.Now}");
    }
}
