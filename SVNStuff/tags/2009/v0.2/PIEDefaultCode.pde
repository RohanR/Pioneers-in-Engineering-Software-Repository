/* FILE        PIEDefaultCode.pde
   VERSION     0.2
   DATE        14 Mar 2009
   AUTHOR      John Wang (johnjwang@berkeley.edu)
   DESCRIPTION Contains the main control loop. Maps joystick inputs
               to outputs.
               
               Modify userSetup(), dataLoop(), and fastLoop() to add
               your own functionality.
   CHANGELOG
   14 Mar 2009    0.1    Initial version.
   11 Apr 2009    0.2    Added software servo -- Ryan Julian.
                         Added watchdog timer (but disabled by default).
*/

#include <avr/io.h>
#include <avr/wdt.h>
#include "SoftwareServo.h"

#include "aliases.h"
#include "auto.h"
#include "radio.h"

// Disable watchdog timeout
// (the watchdog disables robot if 120ms elapses without a data packet received)
#define WATCHDOG_DISABLE

/* This is an example of defining a constant. The format is:
 *  #define  {NAME}   {REPLACEMENT TEXT}
 * Every occurrence of {NAME} is replaced with {REPLACEMENT TEXT}.
 */
#define  LEDPIN   13

/* Declare a variable name for each servo / motor output you will use here. */
SoftwareServo servo1, servo2;

/* This is called ONCE before the main loop starts to run. */
void userSetup() {
  /* YOUR CODE HERE! */
  pinMode(LEDPIN, OUTPUT);    // set up LEDPIN as a digital output
  
  servo1.attach(9);
  servo2.attach(10);
  
  Serial.println("Initialized.");
}

/* This is called every time new joystick data is released.
 * This is a good place to set outputs that depend on the joystick.
 */
void dataLoop() {
  byte angle;
  int sensor1;
  
  /* YOUR CODE HERE! */
  
  
  // Example of reading from an IR sensor connected to Analog In 0
  sensor1 = analogRead(0);
  
  // Controller 1 button 10 (start button) enables autonomous for testing purposes
  // You may remove this, or *anything* in this function you don't need
  field_auto = p1_b10;
  
  
  digitalWrite(LEDPIN, HIGH);            // blink LED to indicate packet received
  
  angle = map(p1_y1, 0, 255, 0, 180);    // scale the value from range 0-255 to 0-180
  servo1.write(angle);
  angle = map(p1_x2, 0, 255, 0, 180);
  servo2.write(angle);
  
  Serial.println("Packet received!");    // print out some debug information
  
  digitalWrite(LEDPIN, LOW);
}

/* This is called every loop, whether there is new joystick data or not.
 * This is a good place to poll the value of sensors, for example.
 */
void fastLoop() {
  /* YOUR CODE HERE! */
  
}


void setup() {
#ifndef WATCHDOG_DISABLE
  MCUSR &= ~(1 << WDRF);    // clear watchdog reset flag to prevent a restart

  wdt_enable(WDTO_120MS);
  WDTCSR |= (1 << WDIE) | (1 << WDE);    // enable watchdog interrupt + reset
  sei();
#endif

  radioSetup();
  userSetup();
  
  field_auto = 0;
  field_disable = 0;
}

void loop() {
  if (radioListen()) {
#ifndef WATCHDOG_DISABLE
//    wdt_reset();
#endif
    // Call user slow loop
    if (!field_disable) {
      if (field_auto)
        autoLoop();
      else
        dataLoop();
    }
  }
  
  // Call user fast loop
  if (!field_disable) {
    if (field_auto)
      autoFastLoop();
    else
      fastLoop();
  }
  
  SoftwareServo::refresh();
}

#ifndef WATCHDOG_ENABLE
ISR(WDT_vect) {
  robotDisable();
  
  // wait for radio signal
  while (!radioListen())
    SoftwareServo::refresh();    // make sure the servos are disabled
  
  // system resets after ISR terminates
}
#endif
