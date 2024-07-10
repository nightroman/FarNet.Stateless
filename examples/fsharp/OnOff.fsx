open Stateless

// create the machine

let on = "On"
let off = "Off"
let space = ' '

let mutable state = off
let machine = StateMachine<string, char>(
    (fun () -> state),
    (fun s -> state <- s)
)

machine.Configure(off).Permit(space, on)
machine.Configure(on).Permit(space, off)

// test the machine

open Swensen.Unquote

// start with On
state <- on
test <@ machine.State = on @>
machine.Fire(space)
test <@ machine.State = off @>
machine.Fire(space)
test <@ machine.State = on @>

// start with Off
state <- off
test <@ machine.State = off @>
machine.Fire(space)
test <@ machine.State = on @>
machine.Fire(space)
test <@ machine.State = off @>
