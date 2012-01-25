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
        private int speed, brakeAmount;

        public int motorSpeed
        {
            get {return speed;}
            set
            {
                if (-100 <= value && value <= 100)
                    speed = value;
                SetMotor(speed);
            }
        }

        public int motorBrake
        {
            get { return brakeAmount; }
            set
            {
                if (0 <= value && value <= 10)
                    brakeAmount = value;
            }
        }

        public SimpleMotorController(Robot robot, int deviceNum)
        {
            this.robot = robot;
            this.deviceNumber = deviceNum;
            this.port = robot.motorPort;

            // Enable the motor controller (exit Safe-Start mode)
            byte[] exitSafe = new byte[3];
            exitSafe[0] = (byte)0xAA;
            exitSafe[1] = (byte)deviceNumber;
            exitSafe[2] = (byte)0x03;
            port.Write(exitSafe, 0, 3);
            //Thread.Sleep(1000);
        }

        private void SetMotor(int speed)
        {
            // Brake
            if (speed == 0)
            {
                byte[] buffer = new byte[4];

                buffer[0] = 0xAA;
                buffer[1] = (byte)deviceNumber;
                buffer[2] = 0x12;
                buffer[3] = (byte)((brakeAmount * 32) / 10);
                port.Write(buffer, 0, 4);
            }
            // Forward or reverse
            else {
                byte[] buffer = new byte[5];

                buffer[0] = 0xAA;
                buffer[1] = (byte) deviceNumber;
                if (speed > 0) //turns forward
                    buffer[2] = 0x05;
                else //turns backward
                {
                    buffer[2] = 0x06;
                    speed = -speed;
                }
                speed = (speed * 3200) / 100; // speed scaled to 0-3200
                buffer[3] = (byte)(speed % 32); // low byte
                buffer[4] = (byte)(speed / 32); // high byte
                port.Write(buffer, 0, 5);
            }
        }
    }
}
