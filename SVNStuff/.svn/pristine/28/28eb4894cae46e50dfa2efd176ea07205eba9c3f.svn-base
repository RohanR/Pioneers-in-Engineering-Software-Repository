/* main.c
 *
 * AUTHOR: John Wang
 * VERSION: 1.2  (20 Mar 2010)
 *
 * HISTORY:
 * 1.2 - Fix disable mode
 * 1.1 - Fix to update local motor and servo values
 * 1.0 - Implemented packet format v1
 * 0.2 - Added ADC conversion and response packet
 *
 * DESCRIPTION:
 * This is the firmware for the Orangutan X2 motor
 * controller / IO module. It receives commands from
 * the main processor via UART.
 *
 */
 
#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/delay.h>

#include "uart.h"
#include "analog.h"
#include "LCD.h"
#include "SPI.h"
#include "main.h"
#include "devices.h"
#include "user.h"

//#define DEBUG



// Globals
SerialPacket packet;
TRexValues trex;
X2Values x2;
ServoValues sc;
RabbitValues rabbit;
ControlValues controls;

unsigned int batteryVoltage = 0;

// Flags + state
unsigned char newData = FALSE;
unsigned char rabbitConnection = FALSE;

unsigned char robotState = ROBOT_ENABLE;

// Packet data
unsigned char aPacket[A_PACKET_LENGTH];

// LED Pins
const unsigned char ledPin[] = {PC0, PC2, PC3, PC5, PC7};


#define isValidCommand(x) ((x) == 'X' || (x) == 'F' || (x) == 'A')

/* Sends a packet over the serial link (to the Rabbit) */
void sendPacket(unsigned char cmd, 
				unsigned char length, 
				const unsigned char *data ) {
	unsigned char checksum = 0;
	uart_tx(0xFF);
	uart_tx(cmd);
	uart_tx(length);
	while (length > 0) {
		unsigned char c = *(data++);
		uart_tx(c);
		checksum ^= c;
		length -= 1;
	}
	uart_tx(checksum);
}


/* Called every time a VALID packet is received
 * from the main processor.
 */
void packetReceived () {
	
	switch (packet.cmd) {
	case 'A':
		if (!rabbitConnection && 
				(packet.data[0] & (A_FLAGS_SYN | A_FLAGS_ACK))) {
			// SYN+ACK received; fire back an ACK packet and establish connection
			aPacket[0] = A_FLAGS_ACK;
			sendPacket('A', A_PACKET_LENGTH, aPacket);
			//LCDString("SYN+ACK received ");
			rabbitConnection = TRUE;
		}
		break;
		
	case 'X':
		for (uint8_t i = 0; i < 12; i += 1)
			controls.analog[i] = packet.data[i];
		for (uint8_t i = 0; i < 8; i += 1)
			controls.digital[i].allbits = packet.data[12 + i];
		rabbit.digitalIn[0].allbits = packet.data[20];
		rabbit.digitalIn[1].allbits = packet.data[21];
		// data[22:29] = encoder counts
		// data[30:31] = timestamp
		
		newData = TRUE;
		//LCDString("Valid data");
		break;

	case 'F':
		robotState = packet.data[0];
		
		if (robotState & ROBOT_DISABLE) {
			x2.motor[0] = 127;
			x2.motor[1] = 127;
			setMotor1(0);
			setMotor2(0);
		}
		break;
	}
}

void byteReceived (unsigned char byte) {
	static unsigned char state = WAIT;
	static unsigned char i = 0;
	static unsigned char checksum = 0;
	
	switch (state) {
		case WAIT:
			if (byte == START_BYTE) {
				state = READ_CMD;
#ifdef DEBUG
				LCDString("Command: ");
#endif
			} else {
				// Print non-command packets as text
				LCDSendData(byte);
			}
		break;
		
		case READ_CMD:
			packet.cmd = byte;
#ifdef DEBUG
				LCDHex(byte);
#endif
			if (isValidCommand(byte)) {
				state = READ_LENGTH;
			} else {
				state = WAIT;
#ifdef DEBUG
				LCDAddString("invalid");
#endif
			}
		break;
		
		case READ_LENGTH:
			packet.length = byte;
			checksum = 0;
			i = 0;
			state = READ_DATA;
#ifdef DEBUG
			LCDAddString(" Length: ");
			LCDHex(byte);
#endif
		break;
		
		case READ_DATA:
			if (i < packet.length) {
				packet.data[i++] = byte;
				checksum = checksum ^ byte;
#ifdef DEBUG
				LCDString("Data[");
				LCDUInt(i);
				LCDAddString("]: ");
				LCDHex(byte);
#endif
			}
			
			if (i >= packet.length)
				state = READ_CHECKSUM;
		break;
		
		case READ_CHECKSUM:
#ifdef DEBUG
			LCDString("Checksum: ");
			LCDHex(byte);
#endif
			if (byte == checksum) {
#ifdef DEBUG
				LCDAddString(" OK");
#endif
				packetReceived();
			}
#ifdef DEBUG
			else {
				LCDSendData(' ');
				LCDHex(checksum);
			}
#endif
			state = WAIT;
		break;
		
		default:
			state = WAIT;
		break;
	}
}


int main (void) {

	unsigned int bytesRead;

	SPIInit();
	LCDInit();
	uart_init();
	analog_init();
	
	// call user initialization
	userInit();
	
	// set LED pins to output
	DDRC |= (1 << PC0) | (1 << PC2) | (1 << PC3) | (1 << PC5) | (1 << PC7);
	
	LCDString("X2 Software v" SOFTWARE_VERSION_STRING);
	LCDMoveCursor(LCD_ROW_1);
	LCDAddString("Team: ");
	LCDUInt(TEAM_NUMBER);
	LCDMoveCursor(LCD_ROW_2);
	
	// enable global interrupts
	sei();
	
	// initialize A packet with software version and team number
	aPacket[0] = A_FLAGS_SYN;
	aPacket[1] = SOFTWARE_VERSION_ID >> 8;
	aPacket[2] = SOFTWARE_VERSION_ID & 0xFF;
	aPacket[3] = TEAM_NUMBER >> 8;
	aPacket[4] = TEAM_NUMBER & 0xFF;
	
	// wait for Rabbit handshake (SYN+ACK)
	while (!rabbitConnection) {
		// periodically send a SYN packet
		sendPacket('A', A_PACKET_LENGTH, aPacket);
		//LCDString("SYN sent");
		_delay_ms(500);
		
		bytesRead = 0;
		while (uart_hasData() && bytesRead < UART_RX_BURST_LENGTH) {
			byteReceived(uart_rx());
			bytesRead += 1;
		}
	}
	
	// main program loop
	for (;;) {
		// read data in bursts
		bytesRead = 0;
		while (uart_hasData() && bytesRead < UART_RX_BURST_LENGTH) {
			byteReceived(uart_rx());
			bytesRead += 1;
		}

		// call the user program
		if (robotState == ROBOT_ENABLE) {
			// new UDP packet received by Rabbit
			if (newData) {
				userLoop();
			}
			userLoopFast();
		} else if (robotState == ROBOT_AUTONOMOUS) {
			userAutoLoop();
		}
		
		// send a response packet and reset the newData flag
		if (newData) {
			unsigned char data[Y_PACKET_LENGTH];

			// 8 bytes motor values
			data[0] = trex.motor[0];
			data[1] = trex.motor[1];
			data[2] = trex.motor[2];
			// bytes 4-6 are unused, left for expansion
			data[6] = x2.motor[0];
			data[7] = x2.motor[1];
			
			// 8 bytes servo values
			data[8] = sc.servo[0];
			data[9] = sc.servo[1];
			data[10] = sc.servo[2];
			data[11] = sc.servo[3];
			data[12] = sc.servo[4];
			data[13] = sc.servo[5];
			data[14] = x2.servo[0];
			data[15] = x2.servo[1];
			
			batteryVoltage = analog10(6);	// update the battery voltage
			data[16] = batteryVoltage >> 8;
			data[17] = batteryVoltage & 0xFF;
			sendPacket('Y', Y_PACKET_LENGTH, data);
			
			newData = FALSE;
		}
	}
	//*/
}
