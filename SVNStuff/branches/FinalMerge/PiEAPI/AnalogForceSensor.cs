/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v0.2b - 04/14/11
 * 
 * Changelog:
 * v0.2b
 *  - Split ReflectanceSensor into AnalogReflectanceSensor
 *      and DigitalReflectanceSensor
 *  - Read now returns an int instead of an Object
*/

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace PiEAPI
{
    class AnalogForceSensor : AnalogSensor //Code for pressure sensors.
    {

        AnalogIn ForceSensor;
        public AnalogForceSensor(int PinNumber)
        {
            this.ForceSensor = GetAnalogPort(PinNumber);
            ForceSensor.SetLinearScale(0, 1023);
        }

        public override int Read()
        {
            return ForceSensor.Read();
        }



    }
}
