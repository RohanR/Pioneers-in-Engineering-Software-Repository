/* FILE        radio.pde
   VERSION     0.2
   DATE        19 Apr 2009
   AUTHOR      John Wang (johnjwang@berkeley.edu)
   DESCRIPTION Reads XBee API-style packets through the serial interface.
               The data can be accessed through the extern array "rxdata".
               
               You should not need to modify this file.
   CHANGELOG
   14 Mar 2009    0.1    Initial version.
   11 Apr 2009    0.2    Changed radios to transparent mode to fix
                         intermittent dropouts. Added separate data buffer.
*/

#include "aliases.h"
#include "radio.h"

// Message format:
// Start  length   id byte  src address  RSSI  options  data           checksum
// 0x7E   MSB,LSB  0x81      MSB,LSB      byte  byte     (len-8 bytes)  byte
// 126 0 18 1 77 0 5 0 74 255 255 0 140 0 0 127 127 127 127 0 0
#define START_BYTE       0x7E
#define TX_ID_BYTE       0x01
#define RX_ID_BYTE       0x81

// State machine states
#ifdef XBEE_TRANSPARENT_MODE
enum {
  STATE_IDLE,
  STATE_DATA,
  STATE_FC_DATA
};
#else
enum {
  STATE_IDLE,
  STATE_LEN_UPPER,
  STATE_LEN_LOWER,
  STATE_ID,
  STATE_SRC_UPPER,
  STATE_SRC_LOWER,
  STATE_RSSI,
  STATE_OPTIONS,
  STATE_DATA,
  STATE_CHECKSUM,
  STATE_UNRECOGNIZED_ID
};
#endif

// Data field format:
// 'J'
// Joystick 1: X, Y, Z, R   (4 bytes)
// Joystick 1: Buttons      (2 bytes)
// Joystick 2: X, Y, Z, R   (4 bytes)
// Joystick 2: Buttons      (2 bytes)
// Optional Hash            (2 bytes)

#ifdef HASHCODE
#define DATA_LENGTH           14
#else
#define DATA_LENGTH           12
#endif
#define DATA_PACKET_HEADER    'J'

// Field control packet format:
// 'F'
// Status: bit1 = disable,  bit0 = autonomous   (1 byte)
// Timer: in seconds                            (1 byte)
// Field data bits: currently unused            (2 bytes)

#define FC_LENGTH             4
#define FC_PACKET_HEADER      'F'


// Make sure this is large enough for the largest packet size
#define BUF_LENGTH    14

// Current state data
int state = STATE_IDLE;
int dataBytesRead;

// Read buffer for packets
unsigned char readbuf[BUF_LENGTH];

// Current packet (only valid when radioListen returns true)
Packet radioData;

// Current control data (now buffered, data is valid at any time)
Datafield rxdata[DATA_LENGTH];

// Current field control data
Datafield fieldData[FC_LENGTH];


/* Initialize the serial port */
void radioSetup() {
  Serial.begin(9600);
}

/* Reads a byte from the serial radio. Returns true if a new packet was just finished.
 */
#ifdef XBEE_TRANSPARENT_MODE
boolean radioListen() {
  if (Serial.available()) {
    byte val = Serial.read();
    
    switch (state) {
      case STATE_IDLE:
        if (val == DATA_PACKET_HEADER) {
          state = STATE_DATA;
        } else if (val == FC_PACKET_HEADER) {
          state = STATE_FC_DATA;
        }
        break;
        
      case STATE_DATA:
        readbuf[dataBytesRead++] = val;
        // Finished reading packet
        if (dataBytesRead == DATA_LENGTH) {
#ifdef HASHCODE
          unsigned int hash = (readbuf[dataBytesRead-2] << 8) | readbuf[dataBytesRead - 1];
          if (*hash != HASHCODE) {
            dataBytesRead = 0;
            return false;
          }
#endif    

          // Don't update data if disabled or in auto mode
          if (field_auto || field_disable)
            dataBytesRead = 0;

          // Copy packet from buffer to rxdata array
          while (dataBytesRead > 0)
            rxdata[--dataBytesRead].allbits = readbuf[dataBytesRead];
          state = STATE_IDLE;
          return true;
        }
        break;
        
      case STATE_FC_DATA:
        readbuf[dataBytesRead++] = val;
        // Finished reading packet
        if (dataBytesRead == FC_LENGTH) {
          // Copy packet from buffer to fieldData array
          while (dataBytesRead > 0)
            fieldData[--dataBytesRead].allbits = readbuf[dataBytesRead];
          state = STATE_IDLE;
        }
        break;
        
      default:
        state = STATE_IDLE;
        break;
    }
  }
  return false;
}
#else  // ifndef XBEE_TRANSPARENT_MODE
boolean radioListen() {
  if (Serial.available()) {
    byte val = Serial.read();
    
    switch (state) {
      case STATE_IDLE:
        if (val == START_BYTE) {
          dataBytesRead = 0;
          state++;
        }
        break;
        
      case STATE_LEN_UPPER:
        radioData.length = val << 8;
        state++;
        break;
        
      case STATE_LEN_LOWER:
        radioData.length |= val;
        state++;
        break;
        
      case STATE_ID:
        if (val == RX_ID_BYTE || val == TX_ID_BYTE)
          state++;
        else
          state = STATE_UNRECOGNIZED_ID;
        break;
      
      case STATE_SRC_UPPER:
        radioData.srcAddr = val << 8;
        state++;
        break;
      
      case STATE_SRC_LOWER:
        radioData.srcAddr |= val;
        state++;
        break;
        
      case STATE_RSSI:
        radioData.rssi = val;
        state++;
        break;
        
      case STATE_OPTIONS:
        radioData.options = val;
        state++;
        break;
      
      case STATE_DATA:
        readbuf[dataBytesRead++] = val;
        if (dataBytesRead == DATA_LENGTH ||
            dataBytesRead == radioData.length - 9)
          state++;
        break;
        
      // The checksum is already checked by XBee, so we are done.
      // Just copy data from the buffer to rxdata
      case STATE_CHECKSUM:
        dataBytesRead -= 1;
        while (dataBytesRead > 0)
            rxdata[--dataBytesRead].allbits = readbuf[dataBytesRead+1];
        state = STATE_IDLE;
        return readbuf[0] == DATA_PACKET_HEADER;
      
      // Discard input until packet is over (including checksum)
      case STATE_UNRECOGNIZED_ID:
        if (--radioData.length == 0)
          state = STATE_IDLE;
        break;
        
      default:
        state = STATE_IDLE;
        break;
    }
  }
  return false;
}
#endif // XBEE_TRANSPARENT_MODE
