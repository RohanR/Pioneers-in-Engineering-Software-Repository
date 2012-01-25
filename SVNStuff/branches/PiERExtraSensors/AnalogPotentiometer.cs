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
    class AnalogPotentiometer : AnalogSensor
    {
        AnalogIn Potentiometer;
        public static double AngleCalibration = .29326; //Multiplying the raw output value by .29326 returns the angle in degrees.
        public AnalogPotentiometer(int PinNumber)
        {
            this.Potentiometer = GetAnalogPort(PinNumber);
            Potentiometer.SetLinearScale(0, 1023);
        }

        public double Read()
        {
            return Potentiometer.Read() * AngleCalibration;
        }


    }
}
