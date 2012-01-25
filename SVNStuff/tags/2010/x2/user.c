/* user.c
 *
 * VERSION: 1.0  (18 Mar 2010)
 *
 * DESCRIPTION:
 * Edit this file as you like. Please do not change the provided
 * function names, but you are free to add as many new functions
 * as needed.
 *
 * Feel free to include additional source files, too. 
 * Don't forget to add *.c files to the Makefile!
*/

#include "user.h"
#include "devices.h"
#include "servo.h"
#include "SPI.h"

#include "LCD.h"	// for debugging

/* This is called ONCE before entering the main loop.
 */ 
void userInit() {
	// Initialize motor values to NEUTRAL = 127
	trex.motor[0] = 127;
	trex.motor[1] = 127;
	trex.motor[2] = 127;
	x2.motor[0] = 127;
	x2.motor[1] = 127;

	// [1 of 2] Uncomment to enable X2 servos
	// Servo 0 is pin D4 and servo 1 is pin D5
	//servo_init();
	//servo_enable(0);
	//servo_enable(1);
}


/* This is called every iteration of the main loop while the
 * robot is in Autonomous mode. The frequency at which it's called
 * is similar to the userLoopFast() function (i.e. as fast as possible).
 */
void userAutoLoop(void) {
}


/* This is called every time data is received from the Rabbit
 * via WiFi.
 */
void userLoop() {
	// Example using the LCD to print debug values
	LCDClear();
	for (unsigned char i = 0; i < 4; i += 1) {
		LCDHex(controls.analog[i]);
		LCDSendData(' ');
	}

	/////////////////////////
	// Drive code example  //
	/////////////////////////

	// Simple "tank drive" example:
	// Use the L joystick's y-axis to control the L motor
	// and the R joystick's y-axis to control the R motor

	// Tank drive using the TReX
	trex.motor[0] = controls.analog[1];
	trex.motor[1] = controls.analog[3];

	// Tank drive using the X2
	//  (see below on how to control the X2 motor directly)
	//x2.motor[0] = controls.analog[1];
	//x2.motor[1] = controls.analog[3];


	///////////////////////////
	// Servo control example //
	///////////////////////////

	// FIRST: Uncomment the TWO sections of code marked 
	//  "Uncomment to enable X2 servos"
	// This maps servos 0 and 1 to the X and Y axes (respectively) of the
	//  second controller
	// (Servo 0 is pin D4 and servo 1 is pin D5: refer to the diagram in
	//  the Electronics Assembly Guide, "Connecting the X2" section)
	//x2.servo[0] = controls.analog[4];
	//x2.servo[1] = controls.analog[5];



	//////////////////////////////////////////////////////////////////
    // Code below this point actually sets motor and servo values.  //
    // Make sure you know what you're doing before modifying!       //
	//////////////////////////////////////////////////////////////////

	// [2 of 2] Uncomment to enable X2 servos
	//servo_setPos(0, x2.servo[0]);
	//servo_setPos(1, x2.servo[1]);

	// This scales the values in x2.motor[] to the range [-255,255]
	//  needed by setMotorN()
	int motorVal;

	motorVal = ((int)x2.motor[0] - 127) * 2;
	setMotor1(motorVal ? motorVal-1 : 0);
	motorVal = ((int)x2.motor[1] - 127) * 2;
	setMotor2(motorVal ? motorVal-1 : 0);

	// For advanced users: The setMotor1() and setMotor2() functions
	//  are defined in SPI.c. Look in that file for functions
	//  that do more advanced motor control (acceleration, braking).
}

/* This is called every iteration of the main loop while the
 * robot is in Enabled mode. As the name suggests, this function
 * is called much more frequently than userLoop() !
 */
void userLoopFast() {
	
}
