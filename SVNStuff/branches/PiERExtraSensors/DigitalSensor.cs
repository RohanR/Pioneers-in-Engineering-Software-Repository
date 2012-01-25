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
    class DigitalSensor //Basic class for digital sensors
    {
        InputPort Digital;

        public DigitalSensor()
        {
        }

        public DigitalSensor(int Pin)
        {
            Digital = GetPort(Pin);
        }


        public bool Read() //Generic output-value-getting method
        {
            return Digital.Read();
        }

        public InputPort GetPort(int num) //Generic pin-getting method
        {

            if (num == 0)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO51, false, Port.ResistorMode.Disabled);
            }
            else if (num == 1)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO53, false, Port.ResistorMode.Disabled);
            }
            else if (num == 2)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO55, false, Port.ResistorMode.Disabled);
            }
            else if (num == 3)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO57, false, Port.ResistorMode.Disabled);
            }
            else if (num == 4)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO58, false, Port.ResistorMode.Disabled);
            }
            else if (num == 5)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO56, false, Port.ResistorMode.Disabled);
            }
            else if (num == 6)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO54, false, Port.ResistorMode.Disabled);
            }
            else
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO52, false, Port.ResistorMode.Disabled);
            }

        }
    }
}
