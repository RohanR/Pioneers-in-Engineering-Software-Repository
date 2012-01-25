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
    class AnalogSonar: AnalogSensor
    {
        public static double InchesCalibration = .5; //Dividing the raw output value by 2 returns the distance in inches.
        private AnalogIn SonarSensor;

        public AnalogSonar(int pin)
        {
            SonarSensor = GetAnalogPort(pin);
            SonarSensor.SetLinearScale(0, 1023);
            SonarSensor.Read();             

        }

        // This returns the distance in INCHES
        public double GetDistance()
        {
            return SonarSensor.Read() * InchesCalibration;
        }

       
    }
}
