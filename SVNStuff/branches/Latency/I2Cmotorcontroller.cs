/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v2.a - 04/08/11
*/

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

namespace PiEAPI
{
    /// <summary>
    /// This controls the Simple Motor Controller motor controllers
    /// </summary>
    class I2CMotorController : ActuatorController
    {
        private Robot robot;
        private ushort deviceAddress;
        private bool canMove;
        public I2CDevice.Configuration conDeviceA;
        //private I2CDevice I2CA;
        private I2CDevice.I2CTransaction[] xActions = new I2CDevice.I2CTransaction[1];
        private float speed;
        private byte[] sendbuffer = new byte[3] { 0x01, 1, 0 };
        public float brakeRangeVal;
        public float brakePowerVal;
        public float motorBrakeVal;
        public float brakeRange
        {
            get
            {
                return brakeRangeVal;
            }
            set
            {
                brakeRangeVal = value;
            }
        }

        //private long lastTicks;

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
                    else if (value > 100)
                    {
                        speed = 100;
                    }
                    else
                    {
                        speed = -100;
                    }
                }
            }

        }

        /// <summary>
        /// Set the brake duty cycle of this Simple (0-10), where
        /// 0 is no brake and 10 is max braking.
        /// </summary>
        public float motorBrake
        {
            get { return motorBrakeVal; }
            set
            {
                if (canMove && 0 <= value && value <= 10)
                    motorBrakeVal = value;
            }
        }

        public float brakePower
        {
            get { return brakePowerVal; }
            set
            {
                if (canMove && 0 <= value && value <= 255)
                    brakePowerVal = value;
            }
        }

        /// <summary>
        /// Ensure to instantiate the MicroMaestro with the Robot given to the student and the device num (probably 13 or 14)
        /// This will take care of connecting the MicroMaestro to the robot's system
        /// </summary>
        /// <param name="robo"> The robot this Simple is connected to. </param>
        /// <param name="deviceNum"> The device number of this Simple. </param>
        public I2CMotorController(Robot robo, ushort deviceAdd)
        {

            //attach to robot, allowing itself to be updated by the Robot thread when called
            robot = robo;
            robot.actuators.Add(this);
            deviceAddress = deviceAdd;

            //initialize the motor as not turning
            canMove = true;
            motorSpeed = 0;
            motorBrake = 0;
            //create I2C Device object representing both devices on our bus
            conDeviceA = new I2CDevice.Configuration(deviceAddress, 100);

            //create I2C Bus object using one of the devices on the bus
            //I2CA = new I2CDevice(conDeviceA);
        }

        /// <summary>
        /// StudentCode should never call this. *Only* the Robot class will on its own.
        /// </summary>
        public void UpdateActuators()
        {
            //long now = DateTime.Now.Ticks;
            //Debug.Print("Time since: " + (now - this.lastTicks));
            //this.lastTicks = now;
            robot.i2c.Config = conDeviceA;
            if (motorBrake == 0 && canMove) // if the motor is supposed to be moving, then execute the code to set the speed
            {
                //send "set speed" command byte
                /*byte[] buffer = new byte[5];
                buffer[0] = (byte)0xAA;
                buffer[1] = (byte)deviceAddress;*/
                //speed
                int pwm = (int)(255 * speed / 100 + .5);
                if (pwm > brakeRange)
                {
                    sendbuffer[1] = (byte)(1);
                    sendbuffer[2] = (byte)(pwm);
                }
                else if (pwm < -brakeRange)
                {
                    sendbuffer[1] = (byte)(0);
                    sendbuffer[2] = (byte)(-1 * pwm);
                }
                else
                {
                    sendbuffer[1] = (byte)(2);
                    sendbuffer[2] = (byte)(brakePower);
                }


            }
            else // if students want to brake, don't change motor speed, brake instead
            {
                sendbuffer[1] = (byte)(2);
                sendbuffer[2] = (byte)(motorBrake);
            }
            xActions[0] = I2CDevice.CreateWriteTransaction(sendbuffer);
            robot.i2c.Execute(xActions, 200);
        }

        /// <summary>
        /// Stops the motor and prevents it from driving until ReviveActuators() is called.
        /// StudentCode can use this for fail-safety measures, etc.
        /// </summary>
        public void KillActuators()
        {
            robot.i2c.Config = conDeviceA;
            if (canMove)
            {
                motorBrake = 7;
                motorSpeed = 0;
                canMove = false;

                sendbuffer[1] = (byte)(3);
                sendbuffer[2] = (byte)(7 / 10 * 255);
                xActions[0] = I2CDevice.CreateWriteTransaction(sendbuffer);
                robot.i2c.Execute(xActions, 200);
            }
        }


        /// <summary>
        /// StudentCode can use this to revive after killing. This will not affect disables sent
        /// from the field or PiEMOS.
        /// </summary>
        public void ReviveActuators()
        {
            robot.i2c.Config = conDeviceA;
            if (robot.canMove && !canMove)
            {
                motorBrake = 0;
                canMove = true;
            }
        }
    }
}