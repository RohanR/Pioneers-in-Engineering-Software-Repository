/* FILE        radio.h
   VERSION     0.2
   DATE        19 Apr 2009
   AUTHOR      John Wang (johnjwang@berkeley.edu)
   DESCRIPTION Reads XBee API-style packets through the serial interface.
               Joystick data can be accessed through the extern array "rxdata".
               Field control packet can be accessed through the extern array "fieldData".
               
   CHANGELOG
   14 Mar 2009    0.1    Initial version.
   11 Apr 2009    0.2    Changed radios to transparent mode to fix
                         intermittent dropouts. Added separate data buffer.
*/

#ifndef __radio_h_
#define __radio_h_


// TO ENABLE NEW RADIO CODE:
// Uncomment the following line.
#define XBEE_TRANSPARENT_MODE

// TO ENABLE HASH:
// Replace the number with a number of your choice (from 0 to 65535).
// Uncomment the following line.
//#define HASHCODE    12345


////////////////
// Data types //
////////////////

/* Stores data about the last XBee API packet received over
   the serial link */
typedef struct {
  unsigned int length;
  unsigned char id;
  unsigned int srcAddr;
  unsigned char rssi;
  unsigned char options;
} Packet;

/* Bitfields of an unsigned char */
typedef struct {
  unsigned int bit0:1;
  unsigned int bit1:1;
  unsigned int bit2:1;
  unsigned int bit3:1;
  unsigned int bit4:1;
  unsigned int bit5:1;
  unsigned int bit6:1;
  unsigned int bit7:1;
} Bitfield;

/* Datafield that is the width of an unsigned char */
typedef union {
  Bitfield bits;
  unsigned char allbits;
} Datafield;


//////////////////////
// Global variables //
//////////////////////

// Current control data (now buffered, data is valid at any time)
// (see aliases.h for user-friendly names)
extern Datafield rxdata[];

// Current field control data
extern Datafield fieldData[];


////////////////
// Prototypes //
////////////////

void radioSetup();
boolean radioListen();

#endif
