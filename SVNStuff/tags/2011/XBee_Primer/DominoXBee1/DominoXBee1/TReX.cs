using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

namespace DominoXBee1
{
    class TReX
    {
        public SerialPort port;
        public Thread thread;
        public byte[] motor1Buffer;

        public TReX(string portName){
            port = new SerialPort(portName, 19200, Parity.None, 8, StopBits.One);
            port.ReadTimeout = 2;
            port.Open();

            motor1Buffer = new byte[16];

            thread = new Thread(new ThreadStart(this.Poll));
            thread.Start();
        }

        public void Poll(){
            //poll feedback from  motor controller
            Thread.Sleep(250);
        }

        public void SetMotor1(byte spd)
        {
            if (spd < 127)
            {
                motor1Buffer[0] = 0xC1;
                motor1Buffer[1] = (byte)(127 - spd);
            }
            else if (spd > 127)
            {
                motor1Buffer[0] = 0xC2;
                motor1Buffer[1] = (byte)(127 - (255 - spd));
            }
            else
            {
                motor1Buffer[1] = 0;
            }

            //Debug.Print("SetMotor1: " + motor1Buffer[1].ToString());
            port.Write(motor1Buffer, 0, 2);
            return;
        }

        public void SetMotor2(byte spd)
        {
            if (spd < 127)
            {
                motor1Buffer[0] = 0xC9;
                motor1Buffer[1] = (byte)(127 - spd);
            }
            else if (spd >= 127)
            {
                motor1Buffer[0] = 0xCA;
                motor1Buffer[1] = (byte)(255 - spd);
            }

            port.Write(motor1Buffer, 0, 2);
            return;
        }
    }
}
