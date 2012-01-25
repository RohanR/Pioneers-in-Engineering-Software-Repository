using System;

// an interface to facilitate the implementation
// of a state machine (probably modeling robot behavior)
namespace PiEAPI
{
    // delegate declarations to pass in the functions for
    // enter/during/exit methods

    public delegate int DuringDelegate();
    public delegate void EnterDelegate();
    public delegate void ExitDelegate();

    public class State
    {


        // the enumeration value of this state
        // this number uniquely determines this state
        public int stateNumber;

        // delegates (essentially boxes holding functions)
        // this allows you to pass in arbitrary functions as arguments to the constructor below
        // so long as the type signature matches that of the delegate.
        private EnterDelegate enterMethod;
        private DuringDelegate duringMethod;
        private ExitDelegate exitMethod;

        public State(int stateNumber, EnterDelegate enter, DuringDelegate during, ExitDelegate exit)
        {
            this.stateNumber = stateNumber;
            enterMethod = enter;
            duringMethod = during;
            exitMethod = exit;
        }


        // code to execute during this state.
        // this method is constantly called by FiniteStateMachine.update()
        // returns the myNumber of the next state (could be this one, could be different)
        public int during()
        {
            return duringMethod();
        }

        // code to execute on entering this state.
        // this method is called if the return of during() changes
        public void enter()
        {
            enterMethod();
        }

        // code to execute when exiting this state.
        // this method is called if the return of during() changes
        public void exit()
        {
            exitMethod();
        }

    }
}

/* NOTES:
 * 
 * delegates: allow them pass in functions to define the enter/during/exit methods
 *      might be worth making documentation about delegate usage.
 * the default State class allows them to pass in delegates that more or less define their state.
*/