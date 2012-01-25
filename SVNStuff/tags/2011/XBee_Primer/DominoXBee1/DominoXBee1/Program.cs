using System;
using System.Threading;

using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;

using GHIElectronics.NETMF.FEZ;

using System.Text;
using System.IO.Ports;

namespace DominoXBee1
{
    public class Program
    {
        public static void Main()
        {
            ushort addr = 1000;
            Radio radio = new Radio(addr);
            TReX jr = new TReX("COM2");

            while (true)
            {
                if (radio.interfacePacketReceived) //if radio interface packet has been succesfully received
                {
                    jr.SetMotor1(radio.data.analog[0]);
                    radio.interfacePacketReceived = false;
                }
                Thread.Sleep(1);
            }
        }

    }
}
