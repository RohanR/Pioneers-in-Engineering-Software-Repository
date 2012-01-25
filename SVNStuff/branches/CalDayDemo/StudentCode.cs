/*
 * Obstacle Avoidance code for CalDay 2011 Demo
 * 
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v2.a - 04/08/11
*/

using System;
using System.Threading;
using Microsoft.SPOT.Hardware;
using Microsoft.SPOT;

namespace PiEAPI
{
    public class StudentCode
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
        private MicroMaestro mm;

        private PIDController pidL;
        private PIDController pidR;
        private Encoder encL;
        private Encoder encR;       

        private static int KP = 2;
        private static int KI = 1;
        private static int KD = 1;

        private SharpDistanceTracker sDT1;
        private SharpDistanceTracker sDT2;
        private SharpDistanceTracker sDT3;
        private SharpDistanceTracker sDT4;
        private SharpDistanceTracker sDT5;

        // Converts counts/second to inches/second
        private static int encScale = 170;
        // Scales ui stick values to inches/second
        private static int uiScale = 100;

        // Constructor
        public StudentCode(Robot robot)
        {
            this.robot = robot;
            smcR = new SimpleMotorController(robot, 13); //let's say 13 is right motor on drivetrain
            smcL = new SimpleMotorController(robot, 14); //let's say 14 is left motor on drivetrain
            mm = new MicroMaestro(robot, 12);
            encL = new Encoder(robot, 0);
            encR = new Encoder(robot, 1);
            pidL = new PIDController(KP, KI, KD);
            pidR = new PIDController(KP, KI, KD);
            sDT1 = new SharpDistanceTracker(0);
            sDT2 = new SharpDistanceTracker(1);
            sDT3 = new SharpDistanceTracker(2);
            sDT4 = new SharpDistanceTracker(3);
            sDT5 = new SharpDistanceTracker(4);
        }

        /// <summary>
        /// The robot will call this method every time it needs to run the user-controlled student code
        /// The StudentCode should basically treat this as a chance to read all the new PiEMOS analog/digital values
        /// and then use them to update the actuator states
        /// </summary>
        public void UserControlledCode()
        {
            int uiX = ScaleUI(robot.UIAnalogVals[0]);
            int uiY = ScaleUI(robot.UIAnalogVals[1]);
            // TODO: Need to check bounds on these scales
            int forwardSpeed = uiX / uiScale;
            int turnSpeed = uiY / uiScale;

            int lOutput = pidL.update(forwardSpeed + turnSpeed, encL.Speed());
            int rOutput = pidR.update(forwardSpeed - turnSpeed, encR.Speed());

            smcL.motorBrake = 0;
            smcR.motorBrake = 0;

            float refer = 1462 / 290;
            float leftActual = (float)encL.Speed() / (float)290;
            float rightActual = (float)encR.Speed() / (float)290;
            float errorK = (float)1.5;

            smcL.motorSpeed += (refer - leftActual) * errorK;
            smcR.motorSpeed += (refer - rightActual) * errorK;

            if (sDT1.GetDistance() < 100)
            {
                smcL.motorSpeed = smcL.motorSpeed * (float)sDT1.GetDistance();
                smcR.motorSpeed = smcR.motorSpeed * (float)sDT1.GetDistance();
            }
        }

        private int ScaleUI(int value)
        {
            int sign = 0;
            if(value > 0) {
                sign = 1;
            } else {
                sign = -1;
            }
            return sign * (((value - 128) ^ 2) >> 14);
        }

        /// <summary>
        /// The robot will call this method every time it needs to run the autonomous student code
        /// The StudentCode should basically treat this as a chance to change motors and servos based on
        /// non user-controlled input like sensors. But you don't need sensors, as this example demonstrates.
        /// </summary>
        public void AutonomousCode()
        {

            smcL.motorBrake = 0;
            smcR.motorBrake = 0;

            float refer = 1462 / 290;
            float leftActual = (float)encL.Speed() / (float)290;
            float rightActual = (float)encR.Speed() / (float)290;
            float errorK = (float)1.5;

            smcL.motorSpeed += (refer - leftActual) * errorK;
            smcR.motorSpeed += (refer - rightActual) * errorK;
        }

    }
}
