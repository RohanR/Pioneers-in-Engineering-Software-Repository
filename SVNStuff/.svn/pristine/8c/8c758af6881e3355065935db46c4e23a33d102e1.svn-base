/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v2.a - 04/08/11
*/

using System;
using Microsoft.SPOT;

namespace PiEAPI
{
    /// <summary>
    /// All actuator controllers have these methods.
    /// *Only* the Robot class will call UpdateActuators() for each actuator when the actuators' states need updating
    /// Both the Robot and StudentCode classes can call Kill/ReviveActuators when changing all actuators is necessary
    /// </summary>
    public interface ActuatorController
    {
        void KillActuators();
        void ReviveActuators();
        void UpdateActuators();
    }
}
