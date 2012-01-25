using System;

//  fields we want:
//  "state": an integer
//  methods we want:
//  update(input) where input is an enumerated command
//  three other methods
//      enter(state)    called on entry
//      during(state)   called repeatedly while in the state (returns a state)
//      exit(state)     called on exit

//  abstract class for an FSM
//  mealy or moore?
//  to read more:
//  http://en.wikipedia.org/wiki/Finite-state_machine

public abstract class FiniteStateMachine
{
    // an integer representing state
    // okay to override?
    private int _state;
    public int state
    {
        get { return _state; }
    }

    // initialize the FSM in the default start state
	public abstract FiniteStateMachine()
	{
	}

    // change the state of the FSM based on the input
    // ?? throw an exception for invalid input
    // ?? return something: 
    //      ?? by default 0/1 for success/fail
    //      ?? by default the state value?
    //      ?? up to user
    public abstract int update(int input)
    {
    }
}
