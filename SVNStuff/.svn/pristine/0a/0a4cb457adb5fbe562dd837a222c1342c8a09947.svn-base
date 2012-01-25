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
    class DigitalReflectanceSensor: DigitalSensor
    {
        public TristatePort ReflectanceSensor;
        private int timer = 0;
        private int pinnumber;

        public DigitalReflectanceSensor(int Pin)
        {
            ReflectanceSensor = new TristatePort((Cpu.Pin)FEZ_Pin.Digital.IO44, true, false, Port.ResistorMode.Disabled);
            ///ReflectanceSensor = GetPort(Pin);
            pinnumber = Pin;
        }


        /*
         * Returns true if an object in the sensor's range is detected. Otherwise, returns false.
         */
        public int Read()
        {
            if (!ReflectanceSensor.Active)
            {
                ReflectanceSensor.Active = true;
            }
            ReflectanceSensor.Write(true);
            if (ReflectanceSensor.Active) { ///set to input
                ReflectanceSensor.Active = false;
                }
            while (ReflectanceSensor.Read() == true)
            {
                timer = timer + 1;
                //System.Threading.Thread.Sleep(0);
            }



            //System.Threading.Thread.Sleep(100);
            int temp = timer;
            timer = 0;
            
            ///ReflectanceSensor.Dispose();
            
            ///ReflectanceSensor = GetPort(pinnumber);
            return temp;

            ///return ReflectanceSensor.Read();
                
            
            
            ///return threshold > timer; //The digital distance sensor is LOW when it detects something in its specified range.
            
        }



        private TristatePort GetPort(int num)
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
                return new TristatePort((Cpu.Pin)FEZ_Pin.Digital.IO51, true, false, Port.ResistorMode.Disabled);
            }
            else if (num == 1)
            {
                return new TristatePort((Cpu.Pin)FEZ_Pin.Digital.IO53, true, false, Port.ResistorMode.Disabled);
            }
            else if (num == 2)
            {
                return new TristatePort((Cpu.Pin)FEZ_Pin.Digital.IO55, true, false, Port.ResistorMode.Disabled);
            }
            else if (num == 3)
            {
                return new TristatePort((Cpu.Pin)FEZ_Pin.Digital.IO57, true, false, Port.ResistorMode.Disabled);
            }
            else if (num == 4)
            {
                return new TristatePort((Cpu.Pin)FEZ_Pin.Digital.IO58, true, false, Port.ResistorMode.Disabled);
            }
            else if (num == 5)
            {
                return new TristatePort((Cpu.Pin)FEZ_Pin.Digital.IO56, true, false, Port.ResistorMode.Disabled);
            }
            else if (num == 6)
            {
                return new TristatePort((Cpu.Pin)FEZ_Pin.Digital.IO54, true, false, Port.ResistorMode.Disabled);
            }
            else
            {
                return new TristatePort((Cpu.Pin)FEZ_Pin.Digital.IO52, true, false, Port.ResistorMode.Disabled);
            }

        }
    }
}
