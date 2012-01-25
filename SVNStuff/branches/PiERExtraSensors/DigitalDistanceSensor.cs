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
    class DigitalDistanceSensor: DigitalSensor
    {
        InputPort DistanceSensor;

        public DigitalDistanceSensor(int Pin)
        {
            DistanceSensor = GetPort(Pin);
        }

        /*
         * Returns true if an object in the sensor's range is detected. Otherwise, returns false.
         */
        public bool Read()
        {
            return !DistanceSensor.Read(); //The digital distance sensor is LOW when it detects something in its specified range.
        }


    }
}
