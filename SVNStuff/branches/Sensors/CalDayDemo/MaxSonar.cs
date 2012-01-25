/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v2.a - 04/08/11
*/

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace PiEAPI
{
    class MaxSonar
    {

        private AnalogIn SonarSensor;

        public MaxSonar(int pin)
        {
            SonarSensor = GetPort(pin);
            SonarSensor.SetLinearScale(0, 1023);
            SonarSensor.Read();             // Self-calibration on first read cycle

        }

        // This returns the distance in INCHES
        public double GetDistance()
        {
            double Volts = SonarSensor.Read() * (3.3 / 1024);
            double Distance = Volts / (3.3 / (double)512);    // Scaling factor of (Vcc / 512) = ~6.4mV
            return Distance;
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
