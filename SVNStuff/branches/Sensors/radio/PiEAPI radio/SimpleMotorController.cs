using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

namespace PiERFramework
{
    /// <summary>
    /// This controls the Simple Motor Controller motor controllers
    /// </summary>
    class SimpleMotorController : ActuatorController
    {
        private const String pt = "COM1"; //simples always connected through COM1
        private SerialPort port;
        private Robot robot;
        private int deviceNumber;
        private bool canMove;

        private float speed;
        private float brake;

        private long lastTicks;

        /// <summary>
        /// StudentCode should use only motorSpeed (-100 to 100) and motorBrake (0 to 10) to control motors
        /// To move motors, first ensure motorBrake equals 0
        /// </summary>
        public float motorSpeed
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

        public float motorBrake
        {
            get { return brake; }
            set
            {
                if (canMove && 0 <= value && value <= 10)
                    brake = value;
            }
        }

        /// <summary>
        /// Ensure to instantiate the MicroMaestro with the Robot given to the student and the device num (probably 13 or 14)
        /// This will take care of connecting the MicroMaestro to the robot's system
        /// </summary>
        /// <param name="robo"></param>
        /// <param name="deviceNum"></param>
        public SimpleMotorController(Robot robo, int deviceNum)
        {

            //attach to robot, allowing itself to be updated by the Robot thread when called
            robot = robo;
            robot.actuators.Add(this);
            deviceNumber = deviceNum;

            //initialize the motor as not turning
            canMove = true;
            motorSpeed = 0;
            motorBrake = 0;

            //create new port or use already existing port and send baud rate auto-detect byte (0xAA)
            foreach (SerialPort pts in robot.ports)
            {
                if (pts.PortName == pt)
                {
                    port = pts;
                }
            }
            if (port == null)
            {
                port = new SerialPort(pt, 9600, Parity.None, 8, StopBits.One);
                robot.ports.Add(port);
                port.ReadTimeout = 2;
                port.Open();
            }
			//send autodetect baud rate
            port.Write(new byte[] { (byte)0xAA }, 0, 1);
            
            //send the command to exit the SMC's safe-mode
            byte[] exitSafe = new byte[3];
            exitSafe[0] = (byte)0xAA;
            exitSafe[1] = (byte)deviceNumber;
            exitSafe[2] = (byte)0x03;
            port.Write(exitSafe, 0, 3);
        }

        /// <summary>
        /// StudentCode should never call this. *Only* the Robot class will on its own.
        /// </summary>
        public void UpdateActuators()
        {
            long now = DateTime.Now.Ticks;
            //Debug.Print("Time since: " + (now - this.lastTicks));
            this.lastTicks = now;
            if (motorBrake == 0 && canMove) // if the motor is supposed to be moving, then execute the code to set the speed
            {
                //send "set speed" command byte
                byte[] buffer = new byte[5];
                buffer[0] = (byte) 0xAA;
                buffer[1] = (byte)deviceNumber;
                int speed = (int)((motorSpeed * 3200) / (float)100.00); //speed

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
                buffer[3] = (byte)((motorBrake * 32) / (float)10.00); //brake amount
                port.Write(buffer, 0, 4);
            }
        }

        // can be called by either StudentCode or Robot thread. Will automatically stop the motor and not allow it to move until ReviveActuators() has been called
        /// <summary>
        /// StudentCode can use this for fail-safety measures, etc.
        /// </summary>
        public void KillActuators()
        {
            if (canMove)
            {
                motorBrake = 7;
                motorSpeed = 0;
                canMove = false;

                //set motor forward speed limit to 0, so nothing can move forward
                byte[] buffer = new byte[3];
                buffer[0] = 0xAA;
                buffer[1] = (byte)deviceNumber;
                buffer[2] = (byte)0x60;
                port.Write(buffer, 0, 3);
            }
        }


        /// <summary>
        /// StudentCode can use this to revive after killing. This will not affect field killing/revival
        /// </summary>
        public void ReviveActuators()
        {
            if (robot.canMove && !canMove)
            {
                motorBrake = 0;
                canMove = true;

                byte[] exitSafe = new byte[3];
                exitSafe[0] = (byte)0xAA;
                exitSafe[1] = (byte)deviceNumber;
                exitSafe[2] = (byte)0x03;
                port.Write(exitSafe, 0, 3);
            }
        }
    }
}