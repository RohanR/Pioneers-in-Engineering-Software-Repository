/* FILE        auto.pde
   VERSION     0.1
   DATE        19 Apr 2009
   AUTHOR      John Wang (johnjwang@berkeley.edu)
   DESCRIPTION Contains the main control loop. Maps joystick inputs
               to outputs.
               
               Modify userSetup(), dataLoop(), and fastLoop() to add
               your own functionality.
   CHANGELOG
   19 Apr 2009    0.1    Initial version.
*/

#include "SoftwareServo.h"

#include <avr/io.h>

unsigned char ddrb, ddrc, ddrd, pud, disabled;

/* Similar to dataLoop for autonomous mode.
   Called every ~25 ms or so when a data packet is received. */
void autoLoop() {
  static boolean ledStatus = 1;
  
  /* YOUR CODE HERE! */
  
  digitalWrite(13, ledStatus);       // Blink the LED
  ledStatus = !ledStatus;
}

/* Called every cycle. You may overload the controller if you
   read analog values too fast, so a slower loop is provided
   above. */
void autoFastLoop() {
  /* YOUR CODE HERE! */
  
}


/* Do not modify! */
void robotDisable() {
  if (!disabled) {
    // Store old data direction registers
    ddrb = DDRB;
    ddrc = DDRC;
    ddrd = DDRD;
    pud = MCUCR & (1 << PUD);
    
    // Set all servos to neutral
    for ( SoftwareServo *p = SoftwareServo::first; p != NULL; p = p->next )
      p->write(90);
    delay(20);                  // wait the maximum delay for another refresh
    SoftwareServo::refresh();
  
    // Tri-state all output pins (set DDR to input, disable pull-ups globally)
    DDRB = DDRC = DDRD = 0;
    MCUCR |= (1 << PUD);
  
    disabled = 1;
    
    // Turn on LED
    pinMode(13, OUTPUT);
    digitalWrite(13, HIGH);
  }
}

/* Do not modify! */
void robotReenable() {
  if (disabled) {
    // Restore the old DDR values
    DDRB = ddrb;
    DDRC = ddrc;
    DDRD = ddrd;
    if (!pud) MCUCR &= ~(1 << PUD);
    
    disabled = 0;
  }
}
