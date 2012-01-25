using System;
using Microsoft.SPOT;

namespace PiEProject1
{
    public interface Sensor
    {
        void InitializeSensor();
        int ReadSensor();
        void KillSensor();
    }
}
