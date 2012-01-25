/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v0.2b - 010/11/11
 * 
*/

using System;
using Microsoft.SPOT;

namespace PiEAPI
{
    /// <summary>
    /// This class is responsible for arcade style controlling with a tank drive style.
    /// </summary>
    public class Driver
    {
        //Constructor
        public Driver()
        {
            Debug.Print("INSIDE DRIVER CLASS\n");
            //Empty Constructor
        }

        //Main method that is responsible for returning the values for the left and right motor given the x/y coordinates from the joystick.
        //args: Values passed in raw number, 0-255
        //return: Values returned in array, form: [left motor val, right motor val]
        public float[] getMotorVals(int xVal, int yVal)
        {
            int x = xVal - 127; //Get a coordinate value for a mapping of the joystick coordinate
            int y = yVal - 127;

            float[] tempReturn = new float[2];
            //condition where the joystick is to the right or left, for turning the robot without forward and backward movement
            if ((y <= 20) && (y >= -20))
            {
                //This will scale the values of the x direction of the joystick to account for how fast to turn the robot
                tempReturn[0] = scaleToMotorVal(x);
                tempReturn[1] = -scaleToMotorVal(x);
                return tempReturn;
            }
            else {
                if (x < 0) { //turning to the left of some sort
                    int xMag = System.Math.Abs(x);
                    tempReturn[0] = scaleToMotorVal(y);
                    tempReturn[1] = scaleToMotorVal(127 - xMag);
                }
                else { //turning to the right of some sort
                    int xMag = System.Math.Abs(x);
                    tempReturn[0] = scaleToMotorVal(127 - xMag);
                    tempReturn[1] = scaleToMotorVal(y);
                }
            }
            return tempReturn;
        }

        //Takes in a value from the joystick axis and scales it to a range of 100 for the motor
        private float scaleToMotorVal(int num)
        {
            return ((float)num * 100 / (float) 128);
        }

    }
}
