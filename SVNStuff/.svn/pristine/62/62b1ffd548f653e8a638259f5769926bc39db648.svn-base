#ifndef _main_h
#define _main_h


// Miscellaneous helper functions
#define FALSE 0
#define TRUE !(FALSE)

#define sbi(value, bit) ((value) |= (1UL << (bit)))
#define cbi(value, bit) ((value) &= ~(1UL << (bit)))
#define bitRead(value, bit) (((value) >> (bit)) & 0x01)


// State machine states
enum {
	WAIT,
	READ_CMD,
	READ_LENGTH,
	READ_DATA,
	READ_CHECKSUM
};

// Packet structure
typedef struct packet {
	unsigned char cmd;
	unsigned char length;	// length of data
	unsigned char data[256];
} SerialPacket;


/* Packet format
  
  START   (1 byte  =  0xFF)
  CMD     (1 byte)
  LENGTH  (1 byte)
  DATA    (LENGTH bytes)
  CHECKSUM(1 byte  = XOR of DATA bits)
 */
#define START_BYTE 0xFF


/*
  Packet Reference

  'A' packet (5 bytes)
  This lets the master processor (Rabbit) know that the user
  processor (X2) is on, and vice versa.
  
  The X2 sends out SYN packets at a regular interval until a SYN+ACK
  response is received from the Rabbit. Then it sends an ACK  packet,
  and the connection is established.
  
  1 byte   flags        (bit0 = SYN, bit1 = ACK)
  2 bytes  version  	(big endian; for field control and backwards compatibility 
                         with packet formats)
  2 bytes  teamNo       (big endian; high byte should = 0)
  

  'F' packet (1 byte)
  This is received from the master processor and sets the robot state.

  1 byte   flags        (bit0 = DISABLE, bit1 = AUTONOMOUS)
  

  'X' packet (32 bytes)
  This contains fresh data from the master processor. This is sent
  every time the master processor gets new data.

  12 analog bytes (from operator station)
  8 digital bytes (from operator station)
  2 digital in (from Rabbit)
  4 byte encoder 1 count
  4 byte encoder 2 count
  2 byte timestamp

  
  'Y' Response format (18 bytes)
  8 Motor PWM values
  8 Servo values
  2 bytes battery voltage
  
*/
#define A_PACKET_LENGTH    5
#define F_PACKET_LENGTH    1
#define X_PACKET_LENGTH    32
#define Y_PACKET_LENGTH    18

// Definitions for flags field in the 'A' packet format
#define A_FLAGS_SYN	1
#define A_FLAGS_ACK	2

// Definitions for flags field in 'F' packet format (robot state)
#define ROBOT_ENABLE      0
#define ROBOT_DISABLE     1
#define ROBOT_AUTONOMOUS  2


#define UART_RX_BURST_LENGTH   X_PACKET_LENGTH



// This (private) integer version is sent through the field control packet
#define SOFTWARE_VERSION_ID		2
// This is the (public) release version of the code
#define SOFTWARE_VERSION_STRING "1.2"


void sendResponse(void);
void packetReceived(void);
void byteReceived(unsigned char byte);
unsigned char isValidCommand(unsigned char byte);


#endif //_main_h
