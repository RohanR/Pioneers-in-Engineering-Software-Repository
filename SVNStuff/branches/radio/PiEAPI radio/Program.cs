using System;
using Microsoft.SPOT;
using System.Threading;
namespace PiEAPI
{
    public class Program
    {
        private SimpleMotorController smc;
        private SharpDistanceTracker sdt;

        public Program()
        {
            Robot robot = new Robot("1", "COM4");
            smc = new SimpleMotorController(robot, "COM1", 14);
            sdt = new SharpDistanceTracker(robot, 1);
            Thread.Sleep(1000);
            while (true)
            {
                if (true)//sdt.ReadSensor() > 200)
                {
                    // read the values controlling the left joystick, map to values from -100 to 100
                    // set the motor speed magnitude as the 100 * square root of |spd|/100, for one nonlinear joystick to motor speed mapping
                    smc.motorBrake = 0;
                    double spd = ((double)(robot.UIAnalogVals[1] - 128) * 100 / (double)128);
                    if(spd > 0)
                        smc.motorSpeed = System.Math.Pow((spd / (double)100), .5) * 100;
                    else
                        smc.motorSpeed = -1 * System.Math.Pow(((-1*spd) / (double)100), .5) * 100;
                }
                else
                {
                    smc.motorBrake = 0;
                    double spd = -50;
                    smc.motorSpeed = System.Math.Pow((spd / (double)100), .5) * 100;
                }
                Thread.Sleep(500);
            }
        }

    }
}