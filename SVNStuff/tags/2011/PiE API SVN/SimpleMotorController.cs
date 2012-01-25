using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

namespace PiEAPI
{
    class SimpleMotorController
    {
        private SerialPort port;
        private Robot robot;
        private int deviceNumber;

        public double motorSpeed
        {
            get {return motorSpeed;}
            set
            {
                if (-100 <= value && value <= 100)
                    motorSpeed = value;
            }
        }

        public double motorBrake
        {
            get { return motorBrake; }
            set
            {
                if (10 <= value && value <= 10)
                    motorBrake = value;
            }
        }

        public SimpleMotorController(Robot robo, String pt, int deviceNum)
        {
            robot = robo;
            deviceNumber = deviceNum;
            port = new SerialPort(pt, 9600, Parity.None, 8, StopBits.One);
            port.ReadTimeout = 2;
            port.Open();
            byte[] exitSafe = new byte[3];
            exitSafe[0] = (byte)0xAA;
            exitSafe[1] = (byte)0x0D;
            exitSafe[2] = (byte)0x03;
            port.Write(exitSafe, 0, 3);
            Thread.Sleep(1000);
        }

        public void SetMotor()
        {
            if (motorBrake == 0)
            {
                byte[] buffer = new byte[5];
                buffer[0] = 0xAA;
                buffer[1] = (byte) deviceNumber;
                if (motorSpeed > 0) //turns forward
                    buffer[2] = 0x05;
                else //turns backward
                    buffer[2] = 0x06;
                int speed = (int)((motorSpeed * 3200) / (double)100.00); //speed
                buffer[3] = (byte)(speed % 32); //low byte
                buffer[4] = (byte)(speed / 32); //high byte
                port.Write(buffer, 0, 5);
            }
            else // if students want to brake, don't change motor speed, brake instead
            {
                byte[] buffer = new byte[4];
                buffer[0] = 0xAA;
                buffer[1] = (byte) deviceNumber;
                buffer[2] = 0x12;
                buffer[3] = (byte)((motorBrake * 32) / (double)10.00); //brake amount
                port.Write(buffer, 0, 4);
            }
        }
    }
}
