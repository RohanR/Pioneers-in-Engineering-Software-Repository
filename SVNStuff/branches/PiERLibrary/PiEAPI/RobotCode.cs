using System;
using Microsoft.SPOT;

namespace PiEAPI
{
    /// <summary>
    /// The Student Code must implement this interface and is required to have these methods.
    /// This allows us to run the Threads in Robot but move the Main method to StudentCode.
    /// </summary>
    public interface RobotCode
    {
        void UserControlledCode();
        void AutonomousCode();
        Robot getRobot();
    }
}
