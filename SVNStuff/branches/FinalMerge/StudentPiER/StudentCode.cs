/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v0.2b - 04/14/11
 * 
 * Changelog:
 * v0.2b
 *  - Uncommented motor controller constructors
 *  - Moved teleoperated/autonomous checking to Robot.cs threads
*/

using System;
using PiEAPI;

namespace StudentPiER
{
    public class StudentCode : RobotCode
    {
        // Variables

        /// <summary>
        /// robot should be used to access *only* the properties fieldTime, canMove, isAutonomous, and isBlue
        /// </summary>
        private Robot robot;

        /// <summary>
        /// These variables are examples of the use of ActuatorController variables
        /// </summary>
        private SimpleMotorController smcR;
        private SimpleMotorController smcL;
        //private MicroMaestro mm;
        //private int timer;

        /// <summary>
        /// Main method which initializes the robot, and creates
        /// the radio's and student code's threads. Begins running
        /// the threads, and then runs the robot thread indefinitely.
        /// </summary>       
        public static void Main()
        {
            // Initialize robot
            Robot robot = new Robot("1", "COM4");
            robot.Auton(false);
            Master master = new Master(new StudentCode(robot));
            master.RunCode();
        }

        // Constructor
        public StudentCode(Robot robot)
        {
            this.robot = robot;
            smcR = new SimpleMotorController(robot, 13); //let's say 13 is right motor on drivetrain
            smcL = new SimpleMotorController(robot, 14); //let's say 14 is left motor on drivetrain
            //mm = new MicroMaestro(robot, 12);
            //timer = 0;
        }

        // Gets the robot associated with this StudentCode
        public Robot getRobot()
        {
            return this.robot;
        }

        /// <summary>
        /// The robot will call this method every time it needs to run the user-controlled student code
        /// The StudentCode should basically treat this as a chance to read all the new PiEMOS analog/digital values
        /// and then use them to update the actuator states
        /// </summary>
        public void UserControlledCode()
        {
           // Debug.Print("User Controlled");
            // always first set brake to 0 to move the motors
            smcL.motorBrake = 0;
            smcR.motorBrake = 0;

            // Observe two values from the PiEMOS interface (like left and right joysticks) and map them directly to the speeds of the motors
            // PiEMOS interface values will be between 0 and 255, but I'm centering motor speed = 0 at 128 (halfway between 0 and 255), so that I
            // can get negative and positive speeds.
            // Because SimpleMotorController's motorSpeed only accepts between -100 and 100, I have to map the values to that range.
            // Ex. PiEMOS Interface value of 255 --> (255 - 128) * 100 / 128 = 99.23 (basically 100, the highest forward motor speed)
            // EX. PiEMOS Interface value of 0 --> (0 - 128) * 100 / 128 = -100 (the highest backward motor speed)
            // The nice thing is that this will automatically change the motor speed to things like joystick values when the joysticks are moved
            smcR.motorSpeed = ((float)(robot.UIAnalogVals[1] - 128) * 100 / (float)128);
            smcL.motorSpeed = ((float)(robot.UIAnalogVals[3] - 128) * 100 / (float)128);

            // Observe a certain button being pressed on PiEMOS interface, if true (meaning "if pressed"), then brake
            if (robot.UIDigitalVals[0])
            {
                // treat this as a gradual brake. every time this method is called (very often!), it will slowly add more braking
                // until it reaches the max braking of 10, at which it remains at 10.
                if (smcL.motorBrake < 10)
                {
                    smcL.motorBrake = smcL.motorBrake + (float).1; //let's represent fractions with floats. always cast decimals to float
                    smcR.motorBrake = smcR.motorBrake + (float).1;
                }
                else
                {
                    smcL.motorBrake = 10;
                    smcR.motorBrake = 10;
                }
            }
            // at any point, if the button is released, turn braking off
            else
            {
                smcL.motorBrake = 0;
                smcR.motorBrake = 0;
            }
            /*
            Debug.Print("UI: " + robot.UIAnalogVals[0] + " " + robot.UIAnalogVals[1] + " " + robot.UIAnalogVals[2] + " " + robot.UIAnalogVals[3]);
            Debug.Print("canMove: " + robot.canMove);
            Debug.Print("smcL:" + smcL.motorSpeed);
            Debug.Print("smcR:" + smcR.motorSpeed);*/
            
            /*if (mm.speeds[0] == 20)
            {
                if (timer != 60)
                {
                    mm.speeds[0] = 0;
                }
            }
            if(timer == 60)
            {
                if (mm.speeds[0] == 100)
                {
                    mm.speeds[0] = 0;
                    timer = 0;
                }
                else
                {
                    mm.speeds[0] = mm.speeds[0] + 1;
                    timer = 0;
                }
            }
            timer++;
            if (timer % 10 == 0)
            {
                Random rand = new Random();
                mm.speeds[0] = rand.Next(100);
                mm.targets[0] = rand.Next(180);
                mm.speeds[1] = rand.Next(100);
                mm.targets[1] = rand.Next(180);
            }*/
        }

        /// <summary>
        /// The robot will call this method every time it needs to run the autonomous student code
        /// The StudentCode should basically treat this as a chance to change motors and servos based on
        /// non user-controlled input like sensors. But you don't need sensors, as this example demonstrates.
        /// </summary>
        public void AutonomousCode()
        {
            //Debug.Print("Auton");
            /*
            //always set motorBrake to 0 to move motors.
            smcL.motorBrake = 0;
            smcR.motorBrake = 0;

            //move one wheel full forward and one wheel full backward. Circling like a crazy person is fun.
            smcL.motorSpeed = 100;
            smcR.motorSpeed = -100;

            // But wait... there's more!
            // This will wait for 1000 milliseconds (1 second) and then reverse the direction of turning.
            // Generally *avoid* sleeping because it will add up fast and dramatically reduce responsiveness!

            Thread.Sleep(1000);
            smcL.motorSpeed = -100;
            smcR.motorSpeed = 100;*/

        }
    }
}
