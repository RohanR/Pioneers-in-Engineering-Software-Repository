using System;

//  fields we want:
//  "state": an integer
//  methods we want:
//  update(input) where input is an enumerated command
//  three other methods
//      enter(state)    called on entry
//      during(state)   called repeatedly while in the state (returns a state)
//      exit(state)     called on exit

// notes on threading:
// state machine has to encapsulate feedback
// have your different state machines (running concurrently)
// relatively independent!

namespace PiEAPI
{

    public class FiniteStateMachine
    {
        // an integer representing state
        // okay to override?
        private int _state;
        private State[] _stateObjects;
        public int state
        {
            get { return _state; }
        }

        // initialize the FSM in the default start state
        // its sole argument is an array of State type objects
        // each state's myNumber variable should be equal to its
        // index in this array.
        // the initial state is by default the state in the 0 index.
	    public FiniteStateMachine(State[] enumStates)
	    {
            _stateObjects = enumStates;
            _state = 0; // starting state is enumStates[0];
	    }

        class InvalidStateException : System.Exception {}

        // change the state of the FSM based on current conditions
        // and call State object methods for related side effects
        // ?? throw an exception for invalid state
        public void update(int input)
        {
            // this assignment does side effects!
            if (!isValid(_state))
            {
                throw new InvalidStateException();
            }
            int nextState = _stateObjects[_state].during(input);
            if (!isValid(nextState))
            {
                throw new InvalidStateException();
            }
            if (_state != nextState)
            {
                _stateObjects[_state].exit();
                _stateObjects[nextState].enter();
                _state = nextState;
            }
        }

        // checks if a state is valid
        private bool isValid(int s)
        {
            if (s > 0 & s <= _stateObjects.Length)
            {
                if (_stateObjects[s] != null)
                {
                    return true;
                }
            }
            return false;
        }
    }

}
/* NOTES:
 * 
 * threading? where? (need 1 per state machine)
 * simple: have them call update every time they want it to update in StudentCode
*/