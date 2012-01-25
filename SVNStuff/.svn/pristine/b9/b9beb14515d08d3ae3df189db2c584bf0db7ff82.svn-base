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
    class AnalogReflectanceSensor: AnalogSensor
    {

        AnalogIn ReflectSensor;     
        public AnalogReflectanceSensor(int PinNumber)
        {
            this.ReflectSensor = GetAnalogPort(PinNumber);
            ReflectSensor.SetLinearScale(0, 1023);
        }

        public override int Read()
        {
            return ReflectSensor.Read();
        }


        
    }
}
