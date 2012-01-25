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
    class ReflectanceSensor
    {
        enum InputType
        {
            digital = 0,
            analog = 1
        };

        AnalogIn ReflectSensor;
        InputPort DReflectSensor;
        InputType input_T;
        public ReflectanceSensor(int PinNumber, int Input_Type)
        {
            input_T = (InputType)Input_Type;
            if ((InputType)Input_Type == InputType.analog)
            {
                this.ReflectSensor = GetAnalogPort(PinNumber);
                ReflectSensor.SetLinearScale(0, 1023);
            }
            else
            {
                this.DReflectSensor = GetDigitalPort(PinNumber);
            }
        }

        public Object Read()
        {
            if (input_T == InputType.analog)
                return ReflectSensor.Read();
            else
                return !DReflectSensor.Read();
        }

        private AnalogIn GetAnalogPort(int num)
        {
            if (num == 0)
            {
                return new AnalogIn(AnalogIn.Pin.Ain0);
            }
            else if (num == 1)
            {
                return new AnalogIn(AnalogIn.Pin.Ain1);
            }
            else if (num == 2)
            {
                return new AnalogIn(AnalogIn.Pin.Ain2);
            }
            else if (num == 3)
            {
                return new AnalogIn(AnalogIn.Pin.Ain3);
            }
            else if (num == 4)
            {
                return new AnalogIn(AnalogIn.Pin.Ain4);
            }
            else
            {
                return new AnalogIn(AnalogIn.Pin.Ain5);
            }
        }

        private InputPort GetDigitalPort(int num)
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
                return new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO50, false, Port.ResistorMode.Disabled);
            }

        }
    }
}
