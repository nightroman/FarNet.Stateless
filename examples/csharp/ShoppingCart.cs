// Based on https://github.com/Siphonophora/StatelessAndBlazor

using Stateless;
using System;
using System.Linq;

namespace Extra;
#nullable enable

public class ShoppingCart
{
    private readonly StateMachine<ShoppingCartState, ShoppingCartTrigger> stateMachine;
    public object Machine => stateMachine;

    private readonly Action<string> _log;

    public ShoppingCart(Action<string> log)
    {
        _log = log;

        // This constructor for the state machine allows us to store the actual state outside of
        // the state machine, in the State property, where we can persist it to the database.
        stateMachine = new(() => State, (state) => State = state);
        stateMachine.OnTransitionCompleted(t => _log($"Items: {ItemCount}, {t.Trigger}: {t.Source} -> {t.Destination}"));

        stateMachine.Configure(ShoppingCartState.Draft)
            .PermitReentry(ShoppingCartTrigger.AddItem)
            .OnEntryFrom(ShoppingCartTrigger.AddItem, () => ++ItemCount)
            .PermitReentryIf(ShoppingCartTrigger.RemoveItem, CartHasItems)
            .OnEntryFrom(ShoppingCartTrigger.RemoveItem, () => --ItemCount)
            .Permit(ShoppingCartTrigger.DeleteCart, ShoppingCartState.Deleted)
            .PermitIf(ShoppingCartTrigger.PurchaseCart, ShoppingCartState.Purchased, CartHasItems)
            .Permit(ShoppingCartTrigger.SaveCart, ShoppingCartState.Saved)
            ;

        stateMachine.Configure(ShoppingCartState.Saved)
            .Permit(ShoppingCartTrigger.DeleteCart, ShoppingCartState.Deleted)
            .Permit(ShoppingCartTrigger.EditCart, ShoppingCartState.Draft)
            .PermitIf(ShoppingCartTrigger.PurchaseCart, ShoppingCartState.Purchased, CartHasItems)
            ;

        // Note, we can often make the declaration of the state machine shorter and easier to
        // manage with code like this, which adds the AddNote ability to all states in two lines
        // of code, regardless of the total number of states.
        Enum.GetValues<ShoppingCartState>().ToList()
            .ForEach(x => stateMachine.Configure(x).PermitReentry(ShoppingCartTrigger.AddNote));
    }

    public ShoppingCartState State { get; private set; } = ShoppingCartState.Draft;

    public int ItemCount { get; private set; }

    private bool CartHasItems() => ItemCount > 0;
}

public enum ShoppingCartState
{
    Draft = 0,
    Purchased,
    Deleted,
    Saved,
}

public enum ShoppingCartTrigger
{
    AddItem,
    RemoveItem,
    PurchaseCart,
    DeleteCart,
    AddNote,
    SaveCart,
    EditCart,
}
