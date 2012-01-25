using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

namespace PiEProject1
{
    public class TRex : ActuatorController
    {
        public SerialPort port;
        public byte[] motor1Buffer;
        public byte[] motor2Buffer;
        public Robot robot;

        public int motor1Direction;
        public int motor2Direction;
        public byte motor1_ID;
        public byte motor2_ID;
        Thread thread;

        public bool disabled;

        public TRex(string portName, Robot robot)
        {
            this.disabled = false;
            this.motor1Direction = 0;
            this.motor2Direction = 0;
            this.robot = robot;
            port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            port.ReadTimeout = 2;
            port.Open();

            motor1Buffer = new byte[16];
            motor2Buffer = new byte[16];

            thread = new Thread(new ThreadStart(this.Poll));
            thread.Start();
        }

        public void Poll()
        {
            while (true)
            {
                // poll feedback from motor controller
                Thread.Sleep(1000); // milliseconds
                updateMotors();
            }
        }

        //public void SetMotor1(int spd, int dir) // spd from 1 to 127. dir: 0 = back, 1 = forw
        //{
        //    if (dir == 0)
        //    {
        //        motorBuffer[0] = 0xC9;
        //    }
        //    else
        //    {
        //        motorBuffer[0] = 0xCA;
        //    }
        //    motorBuffer[1] = (byte) spd;
        //    port.Write(motorBuffer, 0, 2);
        //    return;
        //}

         public void setMotorID(int motorNumber, byte motorID) {
            switch(motorNumber) {
                case(1):
                    this.motor1_ID = motorID;
                    break;
                case(2):
                    this.motor2_ID = motorID;
                    break;
            }
        }

        public byte getMotorID(int motorNumber) {
           switch(motorNumber) {
                case(1):
                    return this.motor1_ID;
               case(2):
                    return this.motor2_ID;
               default:
                   return 0;
            }

        }

        public int getMotorDir(int motorNumber) {
           switch(motorNumber) {
                case(1):
                    return this.motor1Direction;
               case(2):
                    return this.motor2Direction;
               default:
                   return 0;
            }

        }

        public void updateMotors()
        {
            if (this.disabled == true) return;
            int spd1 = this.robot.getActuatorValue(getMotorID(motor1_ID));
            int spd2 = this.robot.getActuatorValue(getMotorID(motor2_ID));

            if (getMotorDir(1) == 0)
            {
                motor1Buffer[0] = 0xC1;
            }
            else
            {
                motor1Buffer[0] = 0xC2;
            }
            motor1Buffer[1] = (byte)spd1;
            port.Write(motor1Buffer, 0, 2);

            if (getMotorDir(2) == 0)
            {
                motor2Buffer[0] = 0xC9;
            }
            else
            {
                motor2Buffer[0] = 0xCA;
            }
            motor2Buffer[1] = (byte)spd1;
            port.Write(motor2Buffer, 0, 2);
            return;
        }

        public void disable()
        {
            this.disabled = true;
            motor1Buffer[0] = 0xC1;
            motor1Buffer[1] = 0;
            port.Write(motor1Buffer, 0, 2);

            motor2Buffer[0] = 0xC9;
            motor2Buffer[1] = 0;
            port.Write(motor2Buffer, 0, 2);
            return;
        }

        public void enable()
        {
            this.disabled = false;
        }


        public void kill()
        {
            this.disable();
            return;
        }

    }
}
