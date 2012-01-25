using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;

namespace PiEAPI
{
    public class StudentCode
    {
        // Variables
        private Robot robot;
        private SimpleMotorController rightMotor;
        private SimpleMotorController leftMotor;
        private SharpDistanceTracker sdt;
        private InputPort Button;
        private AnalogIn lLightSens;
        private AnalogIn rLightSens;

        // Lock: used for serialization
        private readonly object studentLock = new object();

        public static void Main()
        {
            /*robot object creates its own StudentCode object
            The robot object then calls the UserControlledCode
            and the AutonomousCode methods from within threads.
            The robot object is thus able to switch between autonomous
            and teleoperated period by controlling the execution of
            these threads
             */
            Robot robot = new Robot("1", "COM4");

            while (true)
            {
                Thread.Sleep(50);
            }
        }

        /* Constructor
         * Takes a robot as argument so that the code
         * knows which robot to control (the main method could
         * theoretically control multiple robots), and so that
         * the robot is accesable
         */
        public StudentCode(Robot robot)
        {
            Button = new InputPort((Cpu.Pin)FEZ_Pin.Digital.IO4, true, Port.ResistorMode.PullUp);
            lLightSens = new AnalogIn((AnalogIn.Pin) FEZ_Pin.AnalogIn.An1);
            rLightSens = new AnalogIn((AnalogIn.Pin) FEZ_Pin.AnalogIn.An0);
           // lLightSens.SetLinearScale(0, 10000);
           // rLightSens.SetLinearScale(0, 10000);
            this.robot = robot;
            leftMotor = new SimpleMotorController(robot, "COM1", 13);
            rightMotor = new SimpleMotorController(robot, "COM1", 14);  //Check
            rightMotor.motorBrake = 0;
            leftMotor.motorBrake = 0;
            
     //     sdt = new SharpDistanceTracker(robot, 0);
        }

        public void UserControlledCode()
        {
            bool tankDrive = false;
            bool polygon = false;
            bool line = false;

            int dark = 400;
            int backSlow = -50;
            
            while (true)
            {
                //robot.scWait.WaitOne();
                if (robot.isAutonomous == false)
                {
                    lock (studentLock)
                    {
                        
                        
                        if (true)
                        {

                            Debug.Print("is User Controlled");
                          //  Debug.Print(""+robot.UIAnalogVals[1]);
                           //  lLightSens = new AnalogIn((AnalogIn.Pin)FEZ_Pin.AnalogIn.An1);Debug.Print(""+robot.UIAnalogVals[2]);
                            Debug.Print(""+lLightSens.Read());
                            Debug.Print("" + rLightSens.Read());

                            if (line)
                            {
                                int leftVal = lLightSens.Read();
                                int rightVal = rLightSens.Read();
                                if (rightVal < dark)
                                {
                                    leftMotor.motorSpeed = backSlow;
                                    rightMotor.motorSpeed = -backSlow;
                                }
                                else if (leftVal < dark)
                                {
                                    leftMotor.motorSpeed = -backSlow;
                                    rightMotor.motorSpeed = backSlow;
                                }
                                else
                                {
                                    leftMotor.motorSpeed = backSlow;
                                    rightMotor.motorSpeed = backSlow;
                                }

                            }
                            if (Button.Read())
                            {
                                leftMotor.motorSpeed = 60;
                                rightMotor.motorSpeed = 30;
                                Thread.Sleep(1000);
                            }
                            else
                            if(tankDrive) {
                                leftMotor.motorSpeed = ((double)(robot.UIAnalogVals[1] - 128) * 100 / (double)128);
                                rightMotor.motorSpeed = ((double)(robot.UIAnalogVals[3] - 128) * 100 / (double)128);
                            }
                            else if (polygon)
                            {
                                for (int i = 0; i < 5; i++)
                                {
                                    leftMotor.motorSpeed = 50;
                                    rightMotor.motorSpeed = 30;
                                    Thread.Sleep(1000);
                                    leftMotor.motorSpeed = -50;
                                    Thread.Sleep(500);
                                }
                            }

                            else
                            {
                                leftMotor.motorSpeed = -60;
                                rightMotor.motorSpeed = -30;
                            }
                            Debug.Print("student set motor speed: " + rightMotor.motorSpeed);

                            if (robot.UIDigitalVals[0])
                            {
                                robot.Auton(true);
                            }
                        }
                    }
                }
            }
        }

        public void AutonomousCode()
        {
            while (true)
            {
                if (robot.isAutonomous == true)
                {
                   // Debug.Print("Sensor: " + sdt.ReadSensor());
                    if (!robot.UIDigitalVals[0])
                    {
                        robot.Auton(false);
                    }
                   // robot.autoWait.WaitOne();
                    lock (studentLock)
                    {
                        if (sdt.ReadSensor() > 85)
                        {
                          //  Debug.Print("run fwd after read sensor");
                            rightMotor.motorBrake = 0;
                            rightMotor.motorSpeed = 50;
                        }
                        else
                        {
                            Debug.Print("run fwd after read sensor");
                            rightMotor.motorBrake = 0;
                            rightMotor.motorSpeed = -50;
                        }
                    }
                }
            }
        }


    }
}
