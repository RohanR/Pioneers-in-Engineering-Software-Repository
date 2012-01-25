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

        public override InputPort GetPort(int num)
        {

            if (num == 0)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO51, false, Port.ResistorMode.PullDown);
            }
            else if (num == 1)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO53, false, Port.ResistorMode.PullDown);
            }
            else if (num == 2)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO55, false, Port.ResistorMode.PullDown);
            }
            else if (num == 3)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO57, false, Port.ResistorMode.PullDown);
            }
            else if (num == 4)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO58, false, Port.ResistorMode.PullDown);
            }
            else if (num == 5)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO56, false, Port.ResistorMode.PullDown);
            }
            else if (num == 6)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO54, false, Port.ResistorMode.PullDown);
            }
            else
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO52, false, Port.ResistorMode.PullDown);
            }

        }

    }
}
