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
    /// This controls the Micro Maestro servo controllers
    /// </summary>
    public class MicroMaestro : ActuatorController
    {
        private const String pt = "COM2"; //maestros always connected through COM2 via the top boards
        private SerialPort port;
        private Robot robot;
        private int deviceNumber;
        private bool canMove;

        /// <summary>
        /// students can change all of these for each channel 0-5.
        /// </summary>
        public float[] minRotation; //minimum angle (in degrees) that each channel's can turn to. default is 0
        public float[] maxRotation; //maximum angle (in degrees) that each channel's can turn to. default is 180
        public float[] targets; //changes to whatever angle you want to set the servo to within the rotatation limits above
        public float[] speeds;
        private float[] prevVal; // is the value of the target before KillActuators() was called

        /// <summary>
        /// Ensure to instantiate the MicroMaestro with the Robot given to the student and the device num (probably 12)
        /// This will take care of connecting the MicroMaestro to the robot's system
        /// </summary>
        /// <param name="robo"></param>
        /// <param name="deviceNum"></param>
        public MicroMaestro(Robot robo, int deviceNum)
        {
            robot = robo;
            deviceNumber = deviceNum;
            robot.actuators.Add(this);
            canMove = true;
            minRotation = new float[6];
            maxRotation = new float[6];
            targets = new float[6];
            speeds = new float[6];
            prevVal = new float[6];

            for (int i = 0; i < 6; i++)
            {
                minRotation[i] = 0;
                maxRotation[i] = 180;
                targets[i] = -1;
                speeds[i] = 50;
            }

            //create new port or use already existing port
            foreach (SerialPort pts in robot.ports)
            {
                if (pts.PortName == pt)
                {
                    port = pts;
                }
            }

            
            if (port == null)
            {
                port = new SerialPort(pt, 38400, Parity.None, 8, StopBits.One);
                robot.ports.Add(port);
                port.ReadTimeout = 2;
                port.Open();
            }

            //send autodetect baud rate
            port.Write(new byte[] { (byte)0xAA }, 0, 1);
        }

        /// <summary>
        /// StudentCode should never call this. *Only* the Robot class will on its own.
        /// </summary>
        public void UpdateActuators()
        {
            if (canMove)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (targets[i] != -1)
                    {
                        byte[] buffer = new byte[6];

                        //update target
                        buffer[0] = (byte)0xAA;
                        buffer[1] = (byte)deviceNumber;
                        buffer[2] = (byte)0x07;
                        //13120 = max value micro maestro accepts, 256 = min value
                        buffer[3] = (byte)i;
                        int speed;
                        if(speeds[i] == 100)
                            speed = 0; // unlimited speed
                        else
                            speed = 256 + (int)((speeds[i]) * (13120 - 256) / 100);
                        buffer[4] = (byte)(speed & 0x7F);
                        buffer[5] = (byte)((speed >> 7) & 0x7F);
                        port.Write(buffer, 0, 6);

                        //update target
                        buffer[0] = (byte)0xAA;
                        buffer[1] = (byte)deviceNumber;
                        buffer[2] = (byte)0x04;
                        //13120 = max value micro maestro accepts, 256 = min value
                        int target = 256 + (int)((targets[i] - minRotation[i]) * (13120 - 256) / (float)(maxRotation[i] - minRotation[i]));
                        buffer[3] = (byte)i;
                        buffer[4] = (byte)(target & 0x7F);
                        buffer[5] = (byte)((target >> 7) & 0x7F);
                        port.Write(buffer, 0, 6);
                    }
                }
            }
        }

        /// <summary>
        /// StudentCode can use this for fail-safety measures, etc.
        /// </summary>
        public void KillActuators()
        {
            if (canMove)
            {
                canMove = false;
                for (int i = 0; i < 6; i++)
                {
                    // sending target value of 0 will stop maestro from sending pulses to servos
                    byte[] buffer = new byte[6];
                    buffer[0] = (byte)0xAA;
                    buffer[1] = (byte)deviceNumber;
                    buffer[2] = (byte)0x04;
                    int target = 0;
                    buffer[3] = (byte)i;
                    buffer[4] = (byte)(target & 0x7F);
                    buffer[5] = (byte)((target >> 7) & 0x7F);
                    port.Write(buffer, 0, 6);
                }
            }
        }

        /// <summary>
        /// StudentCode can use this to revive after killing. This will not affect field killing/revival
        /// </summary>
        public void ReviveActuators()
        {
            if (robot.canMove && !canMove)
            {
                canMove = true;
            }
        }
    }
}

