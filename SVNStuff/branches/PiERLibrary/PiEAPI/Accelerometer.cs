/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v2.a - 04/08/11
*/

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace PiEAPI
{
    public class Accelerometer
    {
        AnalogIn y_in;
        AnalogIn x_in;
        AnalogIn z_in;

        private int x_zero, y_zero, z_zero;

        public Accelerometer(int x_pin, int y_pin, int z_pin)
        {
            if (x_pin != -1)
            {
                x_in = GetPort(x_pin);
                x_in.SetLinearScale(0, 255);
            }
            if (y_pin != -1)
            {
                y_in = GetPort(y_pin);
                y_in.SetLinearScale(0, 255);
            }
            if (z_pin != -1)
            {
                z_in = GetPort(z_pin);
                z_in.SetLinearScale(0, 255);
            }
        }

        public int RawX()
        {
            if (x_in != null)
                return x_in.Read();
            else return 0;
        }

        public int RawY()
        {
            if (y_in != null)
                return y_in.Read();
            else return 0;
        }

        public int RawZ()
        {
            if (z_in != null)
                return z_in.Read();
            else return 0;
        }

        public int ReadAxisX()
        {
            if (x_in != null)
                return x_in.Read() - x_zero;
            else return 0;
        }

        public int ReadAxisY()
        {
            if (y_in != null)
                return y_in.Read() - y_zero;
            else return 0;
        }

        public int ReadAxisZ()
        {
            if (z_in != null)
                return z_in.Read() - z_zero;
            else return 0;
        }

        public void Zero()
        {
            double sum_x = 0;
            double sum_y = 0;
            double sum_z = 0;
            for (int count = 0; count < 1000; count++)
            {
                sum_x += ReadAxisX();
                sum_y += ReadAxisY();
                sum_z += ReadAxisZ();
            }

            x_zero = (int)(sum_x / 1000);
            y_zero = (int)(sum_y / 1000);
            z_zero = (int)(sum_z / 1000);
        }


        private AnalogIn GetPort(int num)
        {
            if (num == 0)
            {
                return new AnalogIn(AnalogIn.Pin.Ain0);
            }
            else if (num == 1)
            {
                return new AnalogIn(AnalogIn.Pin.Ain1);
            }
            else if (num == 2)
            {
                return new AnalogIn(AnalogIn.Pin.Ain2);
            }
            else if (num == 3)
            {
                return new AnalogIn(AnalogIn.Pin.Ain3);
            }
            else if (num == 4)
            {
                return new AnalogIn(AnalogIn.Pin.Ain4);
            }
            else
            {
                return new AnalogIn(AnalogIn.Pin.Ain5);
            }
        }
    }
}
