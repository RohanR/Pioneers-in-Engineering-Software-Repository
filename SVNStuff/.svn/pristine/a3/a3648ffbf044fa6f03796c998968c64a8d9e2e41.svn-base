using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace PiEProject1
{
    public class SharpDistanceTracker : Sensor
    {
        AnalogIn sharp;
        public SharpDistanceTracker(int pin)
        {
            sharp = GetPort(pin);
            sharp.SetLinearScale(0, 255);
        }

        public void InitializeSensor()
        {
        }

        public int ReadSensor()
        {
            return sharp.Read();
        }

        public void KillSensor()
        {
        }

        private AnalogIn GetPort(int num)
        {
            if (num == 0)
            {
                return new AnalogIn(AnalogIn.Pin.Ain0);
            }
            else if (num == 1)
            {
                return new AnalogIn(AnalogIn.Pin.Ain1);
            } 
            else if (num == 2)
            {
                return new AnalogIn(AnalogIn.Pin.Ain2);
            }
            else if (num == 3)
            {
                return new AnalogIn(AnalogIn.Pin.Ain3);
            }
            else if (num == 4)
            {
                return new AnalogIn(AnalogIn.Pin.Ain4);
            }
            else
            {
                return new AnalogIn(AnalogIn.Pin.Ain5);
            }
    }
    }
}
