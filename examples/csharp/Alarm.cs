// Based on https://github.com/dotnet-state-machine/stateless/tree/dev/example/AlarmExample

using Stateless;
using System;

namespace Extra;
#nullable enable

public class Alarm
{
    private enum State
    {
        Undefined,
        Disarmed,
        Prearmed,
        Armed,
        Triggered,
        ArmPaused,
        PreTriggered,
        Acknowledged
    }

    private enum Trigger
    {
        Startup,
        Arm,
        Disarm,
        Trigger,
        Acknowledge,
        Pause,
        TimeOut
    }

    private readonly StateMachine<State, Trigger> _machine;
    public object Machine => _machine;

    private readonly System.Timers.Timer? preArmTimer;
    private readonly System.Timers.Timer? pauseTimer;
    private readonly System.Timers.Timer? triggerDelayTimer;
    private readonly System.Timers.Timer? triggerTimeOutTimer;

    private readonly Action<string> _log;
    private bool IsConfigured { get; set; }

    public Alarm(int armDelay, int pauseDelay, int triggerDelay, int triggerTimeOut, Action<string> log)
    {
        _log = log;
        _machine = new StateMachine<State, Trigger>(State.Undefined);

        preArmTimer = new System.Timers.Timer(armDelay * 1000) { AutoReset = false, Enabled = false };
        preArmTimer.Elapsed += TimeoutTimerElapsed;
        pauseTimer = new System.Timers.Timer(pauseDelay * 1000) { AutoReset = false, Enabled = false };
        pauseTimer.Elapsed += TimeoutTimerElapsed;
        triggerDelayTimer = new System.Timers.Timer(triggerDelay * 1000) { AutoReset = false, Enabled = false };
        triggerDelayTimer.Elapsed += TimeoutTimerElapsed;
        triggerTimeOutTimer = new System.Timers.Timer(triggerTimeOut * 1000) { AutoReset = false, Enabled = false };
        triggerTimeOutTimer.Elapsed += TimeoutTimerElapsed;

        _machine.OnTransitioned(OnTransition);

        _machine.Configure(State.Undefined)
            .Permit(Trigger.Startup, State.Disarmed)
            .OnExit(() => IsConfigured = true);

        _machine.Configure(State.Disarmed)
            .Permit(Trigger.Arm, State.Prearmed);

        _machine.Configure(State.Armed)
            .Permit(Trigger.Disarm, State.Disarmed)
            .Permit(Trigger.Trigger, State.PreTriggered)
            .Permit(Trigger.Pause, State.ArmPaused);

        _machine.Configure(State.Prearmed)
            .OnEntry(() => ConfigureTimer(true, preArmTimer, "Pre-arm"))
            .OnExit(() => ConfigureTimer(false, preArmTimer, "Pre-arm"))
            .Permit(Trigger.TimeOut, State.Armed)
            .Permit(Trigger.Disarm, State.Disarmed);

        _machine.Configure(State.ArmPaused)
            .OnEntry(() => ConfigureTimer(true, pauseTimer, "Pause delay"))
            .OnExit(() => ConfigureTimer(false, pauseTimer, "Pause delay"))
            .Permit(Trigger.TimeOut, State.Armed)
            .Permit(Trigger.Trigger, State.PreTriggered);

        _machine.Configure(State.Triggered)
            .OnEntry(() => ConfigureTimer(true, triggerTimeOutTimer, "Trigger timeout"))
            .OnExit(() => ConfigureTimer(false, triggerTimeOutTimer, "Trigger timeout"))
            .Permit(Trigger.TimeOut, State.Armed)
            .Permit(Trigger.Acknowledge, State.Acknowledged);

        _machine.Configure(State.PreTriggered)
            .OnEntry(() => ConfigureTimer(true, triggerDelayTimer, "Trigger delay"))
            .OnExit(() => ConfigureTimer(false, triggerDelayTimer, "Trigger delay"))
            .Permit(Trigger.TimeOut, State.Triggered)
            .Permit(Trigger.Disarm, State.Disarmed);

        _machine.Configure(State.Acknowledged)
            .Permit(Trigger.Disarm, State.Disarmed);

        _machine.Fire(Trigger.Startup);
    }

    private void TimeoutTimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _machine.Fire(Trigger.TimeOut);
    }

    private void ConfigureTimer(bool active, System.Timers.Timer timer, string timerName)
    {
        if (timer != null)
            if (active)
            {
                timer.Start();
                _log($"{timerName} started.");
            }
            else
            {
                timer.Stop();
                _log($"{timerName} cancelled.");
            }
    }

    private void OnTransition(StateMachine<State, Trigger>.Transition transition)
    {
        _log($"Transitioned from {transition.Source} to {transition.Destination} via {transition.Trigger}.");
    }
}
