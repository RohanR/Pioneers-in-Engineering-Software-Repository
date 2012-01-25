using System;

// an interface to facilitate the implementation
// of a state machine (probably modeling robot behavior)
namespace PiEAPI
{

    public class noOp: State
    {
        public int stateNumber = 0;

        public abstract static int during(int input)
        {

        }

        public abstract static void enter()
        {

        }

        public abstract static void exit()
        {

        }

    }

    public class spinRight : State
    {
        public int stateNumber = 1;

        public abstract static int during(int input)
        {

        }

        public abstract static void enter()
        {

        }

        public abstract static void exit()
        {

        }

    }

    public class spinLeft : State
    {
        public int stateNumber = 2;

        public abstract static int during(int input)
        {

        }

        public abstract static void enter()
        {

        }

        public abstract static void exit()
        {

        }

    }

}