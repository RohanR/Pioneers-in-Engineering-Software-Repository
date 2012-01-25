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
    enum SensorT
    {
        Default_Sensor = 0,     //default: no calibration data
        Short_Distance = 1,     //4-30cm
        Long_Distance = 2,      //20-150cm
    };
    public class SharpDistanceTracker: AnalogSensor
    {

        double x1, x2;
        int y2, y1;

        int SensorType;
        AnalogIn sharp;

        public SharpDistanceTracker(int pin, int sensor_type)
        {
            sharp = GetAnalogPort(pin);
            sharp.SetLinearScale(0, 255);
            SensorType = (int)sensor_type;

            // default calibration
            // students may need to provide their own calibration data to get acceptable values
            if ((SensorT)sensor_type == SensorT.Long_Distance)
            {
                y1 = 30;
                y2 = 180;
                x1 = .007;
                x2 = .05;
            }

            if ((SensorT)sensor_type == SensorT.Short_Distance)
            {
                y1 = 20;
                y2 = 180;
                x1 = .032;
                x2 = .226;
            }

        }

        // use this method to manually set calibration data
        public void SetCalibrationData(double x1, double x2, int y1, int y2)
        {
            this.x1 = x1;
            this.x2 = x2;
            this.y1 = y1;
            this.y2 = y2;
        }

        public SharpDistanceTracker(int pin)
        {
            sharp = GetAnalogPort(pin);
            sharp.SetLinearScale(0, 255);
            SensorType = (int)SensorT.Default_Sensor;
        }

        public double GetDistance()
        {
            if (x1 == 0 && x2 == 0 && y1 == 0 && y2 == 0) return 0;     // no data available to determine distance

            int sensordata = ReadSensor();

            if (sensordata != 0)
            {
                double temp = ((double)(sensordata) * ((double)(x2 - x1) / (y2 - y1)));
                double anotherTemp = 0;
                if (temp != 0)
                    anotherTemp = 1 / temp;
                if (SensorType == (int)SensorT.Short_Distance)
                    anotherTemp -= .42;

                return anotherTemp;
            }
            else
                return 0;
        }

        public int ReadSensor()
        {
            return sharp.Read();
        }


    }
}
