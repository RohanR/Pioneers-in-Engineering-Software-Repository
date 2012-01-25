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
using System.Threading;
using Microsoft.SPOT;
using GHIElectronics.NETMF.FEZ;
using GHIElectronics.NETMF.Hardware;


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
        private SimpleMotorController armMotor;
        private SharpDistanceTracker distanceS;
        private AnalogIn pot;
        private AnalogIn IR;
        private int timer;

        // Constructor
        public StudentCode(Robot robot)
        {
            this.robot = robot;
//            pot = new AnalogIn(AnalogIn.Pin.Ain0);
  //          IR = new AnalogIn(AnalogIn.Pin.Ain1);

            smcR = new SimpleMotorController(robot, 13); //let's say 13 is right motor on drivetrain
            smcL = new SimpleMotorController(robot, 14); //let's say 14 is left motor on drivetrain
//            armMotor = new SimpleMotorController(robot, 15); // TODO: Port 15 is the arm motor
            distanceS = new SharpDistanceTracker(0, 2); //We are using a long distance sensor
            timer = 0;
        }

        /// <summary>
        /// The robot will call this method every time it needs to run the user-controlled student code
        /// The StudentCode should basically treat this as a chance to read all the new PiEMOS analog/digital values
        /// and then use them to update the actuator states
        /// </summary>
        public void UserControlledCode()
        {
            /*//Arm positions
            const int upPosition = 500; //TODO: Set to correct value (potentiometer value when the arm is in the up position)
            const int downPosition = 100; //TODO: Set to correct value (potentiometer value when the are is in the down position)
            const int zombieZone = 30; //TODO: Tune this constant (the "deadband", when the arm is +/- up/downPosition the arm power is armBalancePower)

            //IR values
            const int pillowDistance = 400;  //TODO: set to a value corresponding to a distance just inside the pillow

            //Arm powers
            const int armBalancePower = 10; //TODO: determine using code 1 (a motor power level that should result in the arm moving neither up nor down)
            const int armUpPower = 50; //TODO: set using code 1 (arm motor power to use when raising the arm)
            const int armDownPower = -20; //TODO: set using code 1 (arm motor power when lowering the arm)

            Debug.Print("Potentiometer: " + pot.Read());
            Debug.Print("IR: " + IR.Read());
           // Debug.Print("User Controlled")
              */
            // always first set brake to 0 to move the motors
            smcL.motorBrake = 0;
            smcR.motorBrake = 0;
            int reverseDistance = 25; //distance in cm?
            bool reverse = false;
            if (distanceS.GetDistance() <= reverseDistance)
            {
                reverse = true;
            }
            Debug.Print("" + distanceS.GetDistance());
            // Observe two values from the PiEMOS interface (like left and right joysticks) and map them directly to the speeds of the motors
            // PiEMOS interface values will be between 0 and 255, but I'm centering motor speed = 0 at 128 (halfway between 0 and 255), so that I
            // can get negative and positive speeds.
            // Because SimpleMotorController's motorSpeed only accepts between -100 and 100, I have to map the values to that range.
            // Ex. PiEMOS Interface value of 255 --> (255 - 128) * 100 / 128 = 99.23 (basically 100, the highest forward motor speed)
            // EX. PiEMOS Interface value of 0 --> (0 - 128) * 100 / 128 = -100 (the highest backward motor speed)
            // The nice thing is that this will automatically change the motor speed to things like joystick values when the joysticks are moved
            float leftMotorSpeed = ((float)(robot.UIAnalogVals[1] - 128) * 100 / (float)128);
            float rightMotorSpeed = ((float)(robot.UIAnalogVals[3] - 128) * 100 / (float)128);
            if(reverse){
                leftMotorSpeed = -leftMotorSpeed;
                rightMotorSpeed = -rightMotorSpeed;
            }
            smcR.motorSpeed = rightMotorSpeed;
            smcL.motorSpeed = leftMotorSpeed;



            //The following could be used to tune the arm control algorithm
            //code 1:
            /*
            armMotor.motorSpeed = ((float)(robot.UIAnalogVals[2] - 128) * 100 / (float)128);
            Debug.Print("" + armMotor.motorSpeed);
            */

            // Arm code here
            //If the pillow is detected, then move the arm up
            //If the pillow is not detected, then move the arm down

 /*           bool armShouldBeUp = false; //True if the arm should be in the UP position
            //Determine whether the arm should be up or down (set armIsUp)
            int IRValue = IR.Read(); //IRValue is inversly related to distance

            if (IRValue >= pillowDistance)
            {
                armShouldBeUp = true;
            }
            
            int armGoalPosition; //Pot value when the arm is at the correct value
            
            if (armShouldBeUp)
            {
                armGoalPosition = upPosition;
            }
            else
            {
                armGoalPosition = downPosition;
            }

            //Set the arm motor power
            //Assuming that pot values increase with height, if not then potValue = 1024-potValue
            int potValue = pot.Read();
            int armMotorPower;
            if (potValue < (armGoalPosition - zombieZone))
            {
                armMotorPower = armUpPower;
            }
            else if (potValue > (armGoalPosition + zombieZone))
            {
                armMotorPower = armDownPower;
            }
            else
            {
                armMotorPower = armBalancePower;
            }
            armMotor.motorSpeed = armMotorPower;
            */
            /*// Observe a certain button being pressed on PiEMOS interface, if true (meaning "if pressed"), then brake
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
            }*/
            // at any point, if the button is released, turn braking off
            /*else
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
       /*     string s = "";
            foreach(int i in robot.UIAnalogVals)
            {
                s += i + " ";
            }
            Debug.Print(s);
            if (robot.UIAnalogVals[5] < 100)
            {
                mm.speeds[0] = 20;
                Debug.Print("0");
                mm.targets[0] = 0;
            }
            else
            {
                mm.speeds[0] = 50;
                Debug.Print("180");
                mm.targets[0] = 180;
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
