
#class auto

//Networking Headers
/*** BeginHeader ConnectToNetwork userControlInit userAutonInit udpRecieveInterfacePacket udpRecieveFieldPacket ProcessFieldPacket sendInterfaceSocket sendFieldSocket */
int ConnectToNetwork(int network, unsigned char teamNo);
void userControlInit();
void userAutonInit();
int udpRecieveInterfacePacket();
int udpRecieveFieldPacket();
int ProcessFieldPacket();
//int ProcessInterfacePacket(struct interface_struct *thisInterface);
void sendInterfaceSocket(char *data, int len);
void sendFieldSocket(char *data, int len);
/*** EndHeader */

//Control Headers
/*** BeginHeader serialInit updateTREX updateServos*/
void serialInit();
void updateTREX();
void updateServos();
/*** EndHeader */

/*** BeginHeader setTREX0 setTREX1*/
void setTREX0(unsigned char spd, unsigned char spd_prev );
void setTREX1(unsigned char spd, unsigned char spd_prev );

void setTREX1_0(unsigned char spd, unsigned char spd_prev);
void setTREX1_1(unsigned char spd, unsigned char spd_prev);
/*** EndHeader */


//SOFTWARE VERSION ID
#define SOFTWARE_VERSION_ID	4
#define SOFTWARE_VERSION_STRING "4"

//Global
#define TRUE 1
#define FALSE 0

//ROBOT MODES
#define ROBOT_ENABLE			0
#define ROBOT_DISABLE		1
#define ROBOT_AUTON			2

//ROUTER TO CONNECT
#define COMP_ROUTER			0
#define USER_ROUTER			1

#define START_BYTE 0xFF

//Packet Lengths for X2 Communication
#define A_PACKET_LENGTH    5
#define F_PACKET_LENGTH    1
#define X_PACKET_LENGTH    32
#define Y_PACKET_LENGTH    18
#define X2_RX_BURST_LENGTH X_PACKET_LENGTH

#define isValidCommand(x) ((x) == 'Y' || (x) == 'A')

// Definitions for flags field in the 'A' packet format
#define A_FLAGS_SYN	1
#define A_FLAGS_ACK	2

#define X2_CONN_SYN				1
#define X2_CONN_ESTABLISHED	2

/* Packets for Field Communication
Packet reference
'G' (8 bytes)
1 byte robot status flags (bit0 = disable, bit1 = auto)
2 bytes master software version id
2 bytes user software version id
2 bytes battery voltage

'J' (35 bytes)
8 Motor PWM values
8 Servo values
2 bytes battery voltage
16 bytes reserved
*/
#define G_PACKET_LENGTH		8
#define J_PACKET_LENGTH		35

//Defines necessary for Serial E, used for servos
#define SERE_TXPORT PEDR
#define SERE_RXPORT PEDR
#define EINBUFSIZE  255
#define EOUTBUFSIZE 255


#memmap xmem
#use "rcm56xxw.lib"


struct motor_struct {
	unsigned char trex[2][3];
   unsigned char trex_prev[2][3];
   //unsigned char trexCycle;
}	motors;

struct servo_struct {
	unsigned char positions[6];
   unsigned char positions_prev[6];
} servos;

//Controls state of robot (enable, disable, and auton)
unsigned char ROBOTMODE;

// States for X2 receive state machine
enum {
	WAIT,
	START,
   LENGTH,
	DATA,
	CHECKSUM
};

//Struct to handle Packet Communication
typedef struct {
	unsigned int state;
	unsigned char i;
   unsigned char checksum;

   unsigned char cmd;
   unsigned char length;
	unsigned char data[256];
} RxData;

RxData rx;



// Buffer for packets received via UDP
char interBuf[128];
char fieldBuf[10];

// These packets are sent via serial to X2
unsigned char aPacket[A_PACKET_LENGTH];
unsigned char fPacket[F_PACKET_LENGTH];
unsigned char xPacketDefaultValues[X_PACKET_LENGTH];

// These packets are sent via WiFi to field control
unsigned char gPacket[G_PACKET_LENGTH];
unsigned char jPacket[J_PACKET_LENGTH];

// Flags + globals
unsigned char x2OkToSend = FALSE,	// response to joystick data received
		x2Connection = 0,					// stores X2 connection state
		udpNewData = FALSE;				// new *interface* packet received
unsigned int teamNo = 0;


void sendPacket (unsigned char cmd, unsigned char length, unsigned char *data) {
	unsigned char checksum, i;

   serCputc(0xFF); // start
	serCputc(cmd);
   serCputc(length);
   checksum = 0;
   //printf("[packet: %x %x %x ", 0xFF, cmd, length);
   for (i = 0; i < length; i += 1) {
   	serCputc(data[i]);
   	//printf("%x ", data[i]);
   	checksum ^= data[i];
   }
   //printf("%x]\n", checksum);
   serCputc(checksum);
}

void packetReceived() {
	if (rx.cmd == 'A') {
   	if (!x2Connection) {
      	// SYN packet received
      	if (rx.data[0] & A_FLAGS_SYN) {
				x2Connection = X2_CONN_SYN;

            // Send back SYN+ACK packet with team number echoed
				aPacket[0] = A_FLAGS_SYN | A_FLAGS_ACK;
            aPacket[3] = rx.data[3];
            aPacket[4] = rx.data[4];
            sendPacket('A', A_PACKET_LENGTH, aPacket);
         }
      } else if (x2Connection == X2_CONN_SYN) {
			if (rx.data[0] & A_FLAGS_ACK) {
         	x2Connection = X2_CONN_ESTABLISHED;

            // store User Processor software version ID
            gPacket[4] = rx.data[1];
            gPacket[5] = rx.data[2];

            // store team #
            teamNo = (rx.data[3] << 8) | rx.data[4];

            x2OkToSend = TRUE;
         }
      }
   } else if (rx.cmd == 'Y') {
   	// Don't update motor or servo values in disable mode
		if (ROBOTMODE & ROBOT_DISABLE) return;

   	//printf ("packet received\n");
		x2OkToSend = TRUE;

      // Update motor values
		motors.trex[0][0] = rx.data[0];
      motors.trex[0][1] = rx.data[1];
      // TODO rx.data[2] is TReX unidirectional motor
      // rx.data[3:5] is reserved for future expansion
	   motors.trex[1][0] = rx.data[3];
		motors.trex[1][1] = rx.data[4];
      updateTREX();

      // rx.data[6:7] are X2 motor values

      // rx.data[8:13] are sent to servo controller
      servos.positions[0] = rx.data[8];
      servos.positions[1] = rx.data[9];
      servos.positions[2] = rx.data[10];
      servos.positions[3] = rx.data[11];
      servos.positions[4] = rx.data[12];
      servos.positions[5] = rx.data[13];
     	updateServos();
      //printf("motor1: %d,    motor2: %d,    servo1: %d,   servo2: %d\n", motors.trex[0], motors.trex[1], servos.positions[0], servos.positions[1]);
      // rx.data[14:15] are X2 servo values
      // rx.data[16:17] are battery voltage

      // store battery voltage in field control response packet
      gPacket[6] = rx.data[16];
      gPacket[7] = rx.data[17];

      // send J response packet to UDP Interface
      memcpy(jPacket, rx.data, 18);
      sendInterfaceSocket(jPacket, J_PACKET_LENGTH);

      // send G response packet to field control for telemetry (battery voltage)
      sendFieldSocket(gPacket, G_PACKET_LENGTH);
   }
}

void byteReceived(unsigned char c) {
	switch (rx.state) {
	case WAIT:
   	if (c == START_BYTE)
			rx.state = START;
   break;

   case START:
   	if (isValidCommand(c)) {
      	rx.cmd = c;
			rx.state = LENGTH;
      } else {
			rx.state = WAIT;
      }
   break;

   case LENGTH:
   	rx.length = c;
      rx.i = 0;
      rx.checksum = 0;
      rx.state = DATA;
   break;

   case DATA:
		rx.data[rx.i++] = c;
      rx.checksum ^= c;
   	if (rx.i == rx.length)
      	rx.state = CHECKSUM;
   break;

   case CHECKSUM:
		if (c == rx.checksum)
      	packetReceived();
      rx.state = WAIT;
   break;

   default:
   	rx.state = WAIT;
   break;
   } // end switch
}

void setMode(unsigned char mode) {
	ROBOTMODE = mode;
   fPacket[0] = mode;
   gPacket[1] = mode;
}


void disableRobot() {
	// Turn off motors
   motors.trex[0][0] = 127;
	motors.trex[0][1] = 127;
   motors.trex[1][0] = 127;
	motors.trex[1][1] = 127;
	updateTREX();

   // Enter disable mode
   setMode(ROBOT_DISABLE);
   sendPacket('F', F_PACKET_LENGTH, fPacket);
   sendFieldSocket(gPacket, G_PACKET_LENGTH);
}

void main()
{
	//temp variable useful for loops
	int i;
   //temp variables for serial communication
   int c;
   unsigned int bytesRead;

   //variables for wifi communication
   int newMode;
   int interfacePacketRecieved;
   int network;


   #GLOBAL_INIT
	{
		setMode(ROBOT_ENABLE);
      rx.state = WAIT;

      /***********************
        Initialization of Packets
      ***********************/
      // Initialize software version
      aPacket[1] = SOFTWARE_VERSION_ID >> 8;
      aPacket[2] = SOFTWARE_VERSION_ID & 0xFF;

      // Initialize UDP response packets
      gPacket[0] = 'G';
      gPacket[1] = ROBOTMODE;
      gPacket[2] = SOFTWARE_VERSION_ID >> 8;
      gPacket[3] = SOFTWARE_VERSION_ID & 0xFF;
      gPacket[4] = 0;	// user version id
      gPacket[5] = 0;
      gPacket[6] = 0;	// battery voltage
      gPacket[7] = 0;

      // Initialize J packet values to 0
      memset(jPacket, 0, J_PACKET_LENGTH);

      // Initialize X default packet (autonomous mode dummy packet) to 0
      memset(xPacketDefaultValues, 0, X_PACKET_LENGTH);
	}


   /////////////////////////////////////////////////////////////////////////
   // Configure Port D -- Leave PD4-PD7 untouched (used for other purposes)
   /////////////////////////////////////////////////////////////////////////
   WrPortI(PDCR,  &PDCRShadow,  RdPortI(PDCR)  & 0xF0);  // clear bits to pclk/2
   WrPortI(PDFR,  &PDFRShadow,  RdPortI(PDFR)  & 0xF0);  // no special functions
   WrPortI(PDDCR, &PDDCRShadow, RdPortI(PDDCR) & 0xF0);  // clear bits to drive
                                                         //  high and low
   WrPortI(PDDR,  &PDDRShadow,  RdPortI(PDDR)  | 0x0D);  // set outputs high
   WrPortI(PDDDR, &PDDDRShadow, RdPortI(PDDDR) | 0x0D);  // set inputs and
                                                         //  outputs

   //Initialize serial communication
   serialInit();

   //Initialize controls as neutral
   //memset(interface.axis, 127, sizeof(interface.axis));
   //memset(interface.btn, 0, sizeof(interface.btn));

   //Decide which router to connect to
	if (BitRdPortI(PDDR, 1) == 1) {
   	//USER settings
   	BitWrPortI(PDDR, &PDDRShadow, 1, 0);	// turn off user LED
      network = USER_ROUTER;
		printf("USER\n");
	} else {
		//COMPETITION Settings
      BitWrPortI(PDDR, &PDDRShadow, 0, 0);   // turn on user LED
      network = COMP_ROUTER;
      setMode(ROBOT_DISABLE);
      printf("COMPETITION\n");
   }
   // */

   printf("Robot Mode: %d \n", ROBOTMODE);

   // Wait for X2 handshake (to learn team #) before attempting connection
   printf("Waiting for X2...\n");
	while (x2Connection != X2_CONN_ESTABLISHED) {
      if ((c = serCgetc()) != -1) {
   		byteReceived(c);
      }
   }
   printf("X2 connection established\n");
   sendPacket('F', F_PACKET_LENGTH, fPacket);
   ConnectToNetwork(network, teamNo);

   //Main Loop
	while(1) {
		// Receive field, interface & X2 reply packets as they come
   	costate {
			//Check to see if we have new field communication
	      if (udpRecieveFieldPacket()) {

	         newMode = ProcessFieldPacket();
	         if (newMode != -1) {

            	// Set robot mode flags
	            setMode(newMode);

            	// If disable flag set: zero motor values
               if (newMode & ROBOT_DISABLE)
               	disableRobot();

               // Send X2 packet with new mode flags
               sendPacket('F', F_PACKET_LENGTH, fPacket);

					// Send back response packet with our robot mode flags
               //  and code versions
               sendFieldSocket(gPacket, G_PACKET_LENGTH);

	            //printf("Robot Mode: %d \n", ROBOTMODE);
	         }
         }

	      //Check to see if we have new field commuication, and we can use it
	      if (udpRecieveInterfacePacket()) {
	         if (interBuf[0] == 'I') {
	            udpNewData = TRUE;
	         }
         }

      	// Receive X2 serial data in bursts of up to X2_RX_BURST_LENGTH
         bytesRead = 0;
			while ((c = serCgetc()) != -1 &&
         			bytesRead <= X2_RX_BURST_LENGTH) {
				byteReceived((unsigned char)c);
            bytesRead += 1;
         }
      }
      // */

      // Time out if no UDP packets received after 500 ms
      //  TODO: Update the interface to send an enable packet. In the meantime
      //  this feature is only activated in competition mode
      costate {
      	if (network == COMP_ROUTER) {
	         waitfor( udpNewData || DelayMs(500) );
	         if (!udpNewData)
	            disableRobot();
         }
      }

      // Send X2 packets
      costate {
      	// FOR TESTING
         // udpNewData = TRUE;
      	waitfor( udpNewData );

         // Check that disable bit is not set
         if (!(ROBOTMODE & ROBOT_DISABLE)) {

	         // If in autonomous mode, send a dummy 'X' packet to keep the
            // motor refreshing, but do not send actual joystick values
            if (ROBOTMODE & ROBOT_AUTON)
            	sendPacket('X', X_PACKET_LENGTH, xPacketDefaultValues);
            // Otherwise in enable mode, send the received joystick values
            else
	         	sendPacket('X', X_PACKET_LENGTH, interBuf+1);
	         x2OkToSend = FALSE;
	         udpNewData = FALSE;

	         // Wait for reply before sending another packet
	         waitfor( x2OkToSend || DelayMs(500) );
	         if (!x2OkToSend) {   // no reply came within 500ms timeout
	            //printf("disable");
	            disableRobot();
	         }
         }
      }

		costate {
			// If auto bit is set, blink the LED fast
         if (ROBOTMODE & ROBOT_AUTON) {
				BitWrPortI(PDDR, &PDDRShadow, 0, 0);   // turn on user LED
	         waitfor (DelayMs(100));
	         BitWrPortI(PDDR, &PDDRShadow, 1, 0);   // turn off user LED
	         waitfor (DelayMs(100));

         // Otherwise if disable bit is set, blink the LED slowly
         } else if (ROBOTMODE & ROBOT_DISABLE) {
	         BitWrPortI(PDDR, &PDDRShadow, 0, 0);   // turn on user LED
	         waitfor (DelayMs(500));
	         BitWrPortI(PDDR, &PDDRShadow, 1, 0);   // turn off user LED
	         waitfor (DelayMs(500));
         }
      }
   } // while(1)

}	// main

