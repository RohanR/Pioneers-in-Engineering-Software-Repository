using System;
using Microsoft.SPOT;

namespace PiEAPI
{
    public interface ActuatorController
    {
        void KillActuators();
        void ReviveActuators();
        void UpdateActuators();
    }
}
