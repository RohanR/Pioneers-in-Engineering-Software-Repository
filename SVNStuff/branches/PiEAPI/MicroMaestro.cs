using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

namespace PiEAPI
{
    public class MicroMaestro : ActuatorController
    {
        private SerialPort port;
        private Robot robot;
        private int deviceNumber;
        private bool canMove;

        public double[] minRotation;
        public double[] maxRotation;
        public double[] targets;
        public double[] speeds;
        public double[] accelerations;

        public MicroMaestro(Robot robo, string portName, int deviceNum)
        {
            robot = robo;
            deviceNumber = deviceNum;
            robot.actuators.Add(this);
            canMove = true;
            minRotation = new double[6];
            maxRotation = new double[6];
            targets = new double[6];
            speeds = new double[6];
            accelerations = new double[6];

            for (int i = 0; i < 6; i++)
            {
                minRotation[i] = 0;
                maxRotation[i] = 180;
                targets[i] = -1;
                speeds[i] = 50;
                accelerations[i] = 50;
            }

            port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            port.ReadTimeout = 2;
            port.Open();
        }

        public void UpdateActuators()
        {
            for (int i = 0; i < 6; i++)
            {
                if (targets[i] != -1)
                {
                    byte[] buffer = new byte[6];

                    //update speed
                    buffer[0] = (byte)0xAA;
                    buffer[1] = (byte)deviceNumber;
                    buffer[2] = (byte)0x07;
                    //11320 = max value micro maestro accepts
                    int speed = 256 + (int)(speeds[i] * (11320 - 256) / (double)100.0);
                    buffer[3] = (byte)i;
                    buffer[4] = (byte)(speed & 0x7F);
                    buffer[5] = (byte)((speed >> 7) & 0x7F);
                    port.Write(buffer, 0, 6);

                    //update target
                    buffer[0] = (byte)0xAA;
                    buffer[1] = (byte)deviceNumber;
                    buffer[2] = (byte)0x04;
                    //11320 = max value micro maestro accepts
                    int target = 256 + (int)((targets[i] - minRotation[i]) * (11320 - 256) / (double)(maxRotation[i] - minRotation[i]));
                    buffer[3] = (byte)i;
                    buffer[4] = (byte)(target & 0x7F);
                    buffer[5] = (byte)((target >> 7) & 0x7F);
                    port.Write(buffer, 0, 6);
                }
            }
        }

        public void KillActuators()
        {
            canMove = false;
        }

        public void ReviveActuators()
        {
            canMove = true;
        }

        public void SetServoTargets()
        {
            for (int i = 0; i < 6; i++)
            {
                if (targets[i] != -1)
                {
                    byte[] buffer = new byte[6];
                    buffer[0] = (byte)0xAA;
                    buffer[1] = (byte)deviceNumber;
                    buffer[2] = (byte)0x04;
                    //16 383 = max value micro maestro accepts
                    int target = 256 + (int)((targets[i] - minRotation[i]) * (11320 - 256) / (double)(maxRotation[i] - minRotation[i]));
                    buffer[3] = (byte)i;
                    buffer[4] = (byte)(target & 0x7F);
                    buffer[5] = (byte)((target >> 7) & 0x7F);
                    port.Write(buffer, 0, 6);
                }
            }
        }

        public void SetServoSpeeds()
        {
            for (int i = 0; i < 6; i++)
            {
                if (targets[i] != -1)
                {
                    byte[] buffer = new byte[6];
                    buffer[0] = (byte)0xAA;
                    buffer[1] = (byte)deviceNumber;
                    buffer[2] = (byte)0x07;
                    //16 383 = max value micro maestro accepts
                    int speed = 256 + (int)(speeds[i] * (11320 - 256) / (double)100.0);
                    buffer[3] = (byte)i;
                    buffer[4] = (byte)(speed & 0x7F);
                    buffer[5] = (byte)((speed >> 7) & 0x7F);
                    port.Write(buffer, 0, 6);
                }
            }
        }
    }
}