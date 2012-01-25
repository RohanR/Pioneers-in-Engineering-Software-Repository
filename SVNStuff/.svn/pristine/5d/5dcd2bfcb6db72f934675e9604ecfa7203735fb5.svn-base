/* FILE        aliases.h
   VERSION     0.1
   DATE        14 Mar 2009
   AUTHOR      John Wang (johnjwang@berkeley.edu)
   DESCRIPTION Defines easier-to-read aliases for the controller data
               inputs and PWM outputs.
               
               You should not need to modify this file. You can use these
               names to define your own aliases, for example:
               #define  p1_right_trigger  p1_b8
   CHANGELOG
   14 Mar 2009    0.1    Initial version.
   19 Apr 2009    0.2    Changes made for updated radio code.
*/

#ifndef __aliases_h_
#define __aliases_h_

#include "radio.h"

/////////////
// Aliases //
/////////////


// Controller 1 (joysticks X1, Y1, X2, Y2; buttons 1-16)
//  Joystick values range from 0 to 255 (center = 127).
//  Buttons values are 0 (not pressed) or 1 (pressed).
#define p1_x1   rxdata[0].allbits
#define p1_y1   rxdata[1].allbits
#define p1_x2   rxdata[2].allbits
#define p1_y2   rxdata[3].allbits
#define p1_b1   rxdata[4].bits.bit0
#define p1_b2   rxdata[4].bits.bit1
#define p1_b3   rxdata[4].bits.bit2
#define p1_b4   rxdata[4].bits.bit3
#define p1_b5   rxdata[4].bits.bit4
#define p1_b6   rxdata[4].bits.bit5
#define p1_b7   rxdata[4].bits.bit6
#define p1_b8   rxdata[4].bits.bit7
#define p1_b9   rxdata[5].bits.bit0
#define p1_b10  rxdata[5].bits.bit1
#define p1_b11  rxdata[5].bits.bit2
#define p1_b12  rxdata[5].bits.bit3
#define p1_b13  rxdata[5].bits.bit4
#define p1_b14  rxdata[5].bits.bit5
#define p1_b15  rxdata[5].bits.bit6
#define p1_b16  rxdata[5].bits.bit7

// Controller 2 (joysticks X, Y, Z, R, buttons 1-16)
#define p2_x1   rxdata[6].allbits
#define p2_y1   rxdata[7].allbits
#define p2_x2   rxdata[8].allbits
#define p2_y2   rxdata[9].allbits
#define p2_b1   rxdata[10].bits.bit0
#define p2_b2   rxdata[10].bits.bit1
#define p2_b3   rxdata[10].bits.bit2
#define p2_b4   rxdata[10].bits.bit3
#define p2_b5   rxdata[10].bits.bit4
#define p2_b6   rxdata[10].bits.bit5
#define p2_b7   rxdata[10].bits.bit6
#define p2_b8   rxdata[10].bits.bit7
#define p2_b9   rxdata[11].bits.bit0
#define p2_b10  rxdata[11].bits.bit1
#define p2_b11  rxdata[11].bits.bit2
#define p2_b12  rxdata[11].bits.bit3
#define p2_b13  rxdata[11].bits.bit4
#define p2_b14  rxdata[11].bits.bit5
#define p2_b15  rxdata[11].bits.bit6
#define p2_b16  rxdata[11].bits.bit7


// Field status bits
#define field_auto      fieldData[0].bits.bit0
#define field_disable   fieldData[0].bits.bit1

// Field timer value (in seconds)
#define field_timer     fieldData[1].allbits

#endif
