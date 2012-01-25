using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

namespace PiEAPI
{
    class I2CEncoder : SuperEncoder
    {
        private float dPerStep;
        private Robot robot;
        private I2CDevice.Configuration conA;
        private byte[] address = new byte[1] { 0x20 };
        private ushort deviceAddress;
        private byte[] num1 = new byte[1] { 0x0A };
        private byte[] num2 = new byte[1] { 0x0A };
        private byte[] num3 = new byte[1] { 0x0A };
        private byte[] num4 = new byte[1] { 0x0A };
        //private string binToInt;
        private I2CDevice.I2CTransaction[] xActions = new I2CDevice.I2CTransaction[5];
        private float offset;
        private int steps
        {
            get
            {
                /*robot.i2c.Config = conA;
                //address[0]=(byte)deviceAddress;
                xActions[0] = I2CDevice.CreateWriteTransaction(address);
                xActions[1] = I2CDevice.CreateReadTransaction(num1);
                xActions[2] = I2CDevice.CreateReadTransaction(num2);
                xActions[3] = I2CDevice.CreateReadTransaction(num3);
                xActions[4] = I2CDevice.CreateReadTransaction(num4);
                robot.i2c.Execute(xActions, 200);
                //This changes bytes to int
                steps = (((int)num1[0]) << 24) + (((int)num2[0]) << 16) + (((int)num3[0]) << 8) + (int)(num4[0]);
                //binToInt = "" + num1[0] + num2[0] + num3[0] + num4[0];
                //steps = Convert.ToInt32(binToInt,2);*/
                return steps;
            }
            set { steps = value; }
        }
                
        public float displacement
        {
            get
            {
                displacement=steps*dPerStep-offset;
                return displacement;
            }
            set { offset=displacement-value; }
        }
        public I2CEncoder(float displacementPerStep, ushort deviceAdd, Robot robo) : base(displacementPerStep)
        {
            dPerStep=displacementPerStep;
            robot = robo;
            deviceAddress = deviceAdd;
            conA = new I2CDevice.Configuration(deviceAddress, 100);
        }

    }
}