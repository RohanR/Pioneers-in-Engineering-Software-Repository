using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;

namespace PiEAPI
{
    public class ThreeReadLineTracker : Sensor
    {
        private InputPort[] irs;
        private InputPort ir1;
        private InputPort ir2;
        private InputPort ir3;

        public ThreeReadLineTracker(int pin1, int pin2, int pin3)
        {
            // learn how to enumerate in c#
            ir1 = GetPort(pin1);
            ir2 = GetPort(pin2);
            ir3 = GetPort(pin3);
            irs = new InputPort[] {ir1, ir2, ir3};
        }

        public void InitializeSensor()
        {
        }

        public int ReadSensor()
        {
            return -1;
        }

        public int ReadSensor(int portNum)
        {
            if (irs[portNum - 1].Read()) // decrement b/c starts from 0
                return 1;
            else
                return 0; 
        }

        public void KillSensor()
        {
        }

        private InputPort GetPort(int num)
        {
            if (num == 0)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di0, false, Port.ResistorMode.PullUp);
            }
            else if (num == 1)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di1, false, Port.ResistorMode.PullUp);
            }
            else if (num == 2)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di2, false, Port.ResistorMode.PullUp);
            }
            else if (num == 3)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di3, false, Port.ResistorMode.PullUp);
            }
            else if (num == 4)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di4, false, Port.ResistorMode.PullUp);
            }
            else if (num == 5)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di5, false, Port.ResistorMode.PullUp);
            }
            else if (num == 6)
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di6, false, Port.ResistorMode.PullUp);
            }
            else
            {
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.Di7, false, Port.ResistorMode.PullUp);
            }
        }
    }
}
