using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace PiEProject1
{
    class DigitalDistanceSensor
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

        private InputPort GetPort(int num)
        {
            /*
             * Digital Pin 0: Mode/IO4
             * Digital Pin 1: IO44
             * Digital Pin 2: IO45
             * Digital Pin 3: IO46
             * Digital Pin 4: IO47
             * Digital Pin 5: IO48
             * Digital Pin 6: IO49
             * Digital Pin 7: IO50
             */
            if (num == 0)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO4, false, Port.ResistorMode.Disabled);
            }
            else if (num == 1)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO44, false, Port.ResistorMode.Disabled);
            }
            else if (num == 2)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO45, false, Port.ResistorMode.Disabled);
            }
            else if (num == 3)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO46, false, Port.ResistorMode.Disabled);
            }
            else if (num == 4)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO47, false, Port.ResistorMode.Disabled);
            }
            else if (num == 5)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO48, false, Port.ResistorMode.Disabled);
            }
            else if (num == 6)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO49, false, Port.ResistorMode.Disabled);
            }
            else
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO50, true, Port.ResistorMode.Disabled);
            }

        }
    }
}
