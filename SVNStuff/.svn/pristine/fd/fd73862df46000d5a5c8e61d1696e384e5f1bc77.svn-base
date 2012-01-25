/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v0.2b - 04/14/11
 * 
 * Changelog:
 * v0.2b
 *  - Changed pullup resistor mode to PullDown
*/

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace PiEAPI
{
    class DigitalSwitch: DigitalSensor
    {
        InputPort port;
        public DigitalSwitch(int Pin)
        {
            port = GetPort(Pin);
        }

 
        public bool Read()
        {
            return port.Read(); 
        }

    }
}
