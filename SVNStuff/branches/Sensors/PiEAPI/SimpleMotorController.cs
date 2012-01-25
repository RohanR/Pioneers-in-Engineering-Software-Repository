using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

namespace PiEAPI
{
    class SimpleMotorController : ActuatorController
    {
        private SerialPort port;
        private Robot robot;
        private int deviceNumber;
        private bool canMove;
        private double speed;
        private double brake;

        public double motorSpeed
        {
            get { return speed; }
            set
            {
                if (canMove)
                {
                    if (-100 <= value && value <= 100)
                    {
                        speed = value;
                    }
                }
            }
        }

        public double motorBrake
        {
            get { return brake; }
            set
            {
                if (canMove && 0 <= value && value <= 10)
                    brake = value;
            }
        }

        public SimpleMotorController(Robot robo, String pt, int deviceNum)
        {
            //attach to robot, allowing itself to be updated by the Robot thread when called
            robot = robo;
            robot.actuators.Add(this);
            deviceNumber = deviceNum;

            //initialize the motor as not turning
            canMove = true;
            motorSpeed = 0;
            motorBrake = 0;

            //set up port and send baud rate auto-detect byte (0xAA)
           
            // port = new SerialPort(pt, 9600, Parity.None, 8, StopBits.One);
          //  port.ReadTimeout = 2;
            //port.Open();
            port = robot.motorPort;
         //   port.Write(new byte[] { (byte)0xAA }, 0, 1);
         //   Thread.Sleep(1000);
            
            //send the command to exit the SMC's safe-mode
            byte[] exitSafe = new byte[3];
            exitSafe[0] = (byte)0xAA;
            exitSafe[1] = (byte)deviceNumber;
            exitSafe[2] = (byte)0x03;
            port.Write(exitSafe, 0, 3);
          //  Thread.Sleep(1000);
        }

        // will be called by Robot thread in Run() method
        public void UpdateActuators()
        {
            if (motorBrake == 0 && canMove) // if the motor is supposed to be moving, then execute the code to set the speed
            {
                //send "set speed" command byte
                byte[] buffer = new byte[5];
                buffer[0] = (byte) 0xAA;
                buffer[1] = (byte)deviceNumber;
                int speed = (int)((motorSpeed * 3200) / (double)100.00); //speed

                if (speed > 0)
                    buffer[2] = 0x05; //turns forward
                else 
                {
                    buffer[2] = 0x06; //turns backward
                    speed = -1 * speed;
                }

                buffer[3] = (byte)(speed % 32); // speed low byte
                buffer[4] = (byte)(speed / 32); // speed high byte
                port.Write(buffer, 0, 5);
            }
            else // if students want to brake, don't change motor speed, brake instead
            {
                byte[] buffer = new byte[4];
                buffer[0] = 0xAA;
                buffer[1] = (byte)deviceNumber;
                buffer[2] = (byte) 0x12;
                buffer[3] = (byte)((motorBrake * 32) / (double)10.00); //brake amount
                port.Write(buffer, 0, 4);
            }
        }

        // can be called by either StudentCode or Robot thread. Will automatically stop the motor and not allow it to move until ReviveActuators() has been called
        public void KillActuators()
        {
            if (canMove)
            {
                motorBrake = 7;
                motorSpeed = 0;
                canMove = false;

                /*
                //set motor forward speed limit to 0, so nothing can move forward
                byte[] buffer = new byte[6];
                buffer[0] = 0xAA;
                buffer[1] = (byte)deviceNumber;
                buffer[2] = (byte)0x22;
                buffer[3] = (byte)0; // forward limit command
                buffer[4] = (byte)0; // limit low byte
                buffer[5] = (byte)0; // limit high byte
                port.Write(buffer, 0, 6);

                //set motor backward speed limit to 0, so nothing can move backward
                buffer[0] = 0xAA;
                buffer[1] = (byte)deviceNumber;
                buffer[2] = (byte)0x22;
                buffer[3] = (byte)1; // backward limit command
                buffer[4] = (byte)0; // limit low byte
                buffer[5] = (byte)0; // limit high byte
                port.Write(buffer, 0, 6);*/
            }
        }


        // will allow the motor to move again, but will not do anything if Robot's canMove bool is false, meaning it should be revived
        public void ReviveActuators()
        {
            if (robot.canMove && !canMove)
            {
                motorBrake = 0;
                canMove = true;
                /*
                //set motor forward speed limit to 3200 (out of 3200), so robot can move forward
                byte[] buffer = new byte[6];
                buffer[0] = 0xAA;
                buffer[1] = (byte)deviceNumber;
                buffer[2] = (byte)0x22;
                buffer[3] = (byte) 0;
                buffer[4] = (byte) (3200 % 32); // speed limit low byte
                buffer[5] = (byte) (3200 / 32); // speed limit low byte
                port.Write(buffer, 0, 6);

                //set motor backward speed limit to 3200 (out of 3200), so robot can move backward
                buffer[0] = 0xAA;
                buffer[1] = (byte)deviceNumber;
                buffer[2] = (byte)0x22;
                buffer[3] = (byte) 1;
                buffer[4] = (byte) (3200 % 32); // speed limit low byte
                buffer[5] = (byte) (3200 / 32); // speed limit high byte
                port.Write(buffer, 0, 6);*/
            }
        }
    }
}