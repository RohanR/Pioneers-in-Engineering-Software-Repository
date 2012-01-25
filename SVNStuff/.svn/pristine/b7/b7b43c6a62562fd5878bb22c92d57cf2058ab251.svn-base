/* main.c
 *
 * AUTHOR: John Wang
 * VERSION: 0.2 (21 Feb 2011)
 *  Updated for the ATtiny2313A. The new PCINTx vectors allow us to
 *  write smaller interrupt handlers.
 *
 * HISTORY: 0.1 (6 Nov 2010) - ATtiny2313
 *
 * DESCRIPTION:
 *
 *  This implements a quadrature encoder counter for up to 4 encoder channels.
 *  Values can be read via UART using the Pololu protocol.
 *  (http://www.pololu.com/docs/0J44/6.2) The default address is 10 (0xA).
 *
 *  The pin connections are:
 *
 *  Encoder    A    B
 *  0         PD3  PD4
 *  1         PD2  PD5
 *  2         PA0  PA1
 *  3         PB0  PB1
 *
 */

// WARNING: Serial will not work properly if DEBUG is enabled
//#define DEBUG
 
#include <avr/io.h>
#include <avr/interrupt.h>
#include <util/delay.h>

#include "uart.h"
#include "main.h"


// globals
EncoderConfig config;
volatile long count[4];


/*
 * ISR for encoder 0. Triggers on the encoder B pin (PD3).
 *
 * This code works for both rising and falling edge triggered
 *  interrupt. Can be optimized further for either one.
 */
ISR(INT1_vect) {
	// PD3/INT1 is encoder 0A
	// PD4 is encoder 0B
	unsigned char tmpD = PIND;
	
	// If inputs are different, increment counter
	// -----A---    (tmpD)
	//  ----B----   (tmpD >> 1)
	if ((tmpD ^ (tmpD >> 1)) & (1 << PD3))
		count[0] += 1;
	else
		count[0] -= 1;
}

/*
 * ISR for encoder 1. Triggers on the encoder B pin (PD2).
 */
ISR(INT0_vect) {
	// PD2/INT0 is encoder 1A
	// PD5 is encoder 1B
	unsigned char tmpD = PIND;
	
	// If inputs are different, increment counter
	// -----A--    (tmpD)
	//    --B----- (tmpD >> 3)
	if ((tmpD ^ (tmpD >> 3)) & (1 << PD2))
		count[1] += 1;
	else
		count[1] -= 1;
}

/* ISR for encoder 2.
 * Called on BOTH the rising and falling edge.
 */
ISR(PCINT_A_vect) {
	unsigned char tmpA = PINA;
	
	// If we're not counting both edges, check for rising edge
	if (config.count_both_edges_enc2 || (tmpA & (1 << PA0))) {
		if ((tmpA ^ (tmpA >> 1)) & (1 << PA0))
			count[2] += 1;
		else
			count[2] -= 1;
	}
}


/*
 * ISR for encoder 3.
 * Called on BOTH the rising and falling edge.
 */
ISR(PCINT_B_vect) {
	unsigned char tmpB = PINB;
	
	// If we're not counting both edges, check for rising edge
	if (config.count_both_edges_enc3 || (tmpB & (1 << PB0))) {
		if ((tmpB ^ (tmpB >> 1)) & (1 << PB0))
			count[3] += 1;
		else
			count[3] -= 1;
	}
}

void byte_received(unsigned char data) {
	static unsigned char state = STATE_IDLE;
	
	// At any point, the start byte (0xAA) causes switch into start condition
	if (data == 0xAA) {
		state = STATE_START;
		PORTA = state + 1;
		return;
	}
	
	switch (state) {
	// Received start byte (0xAA)
	// Check for device address match
	case STATE_START:
		if (data == MY_DEVICE_NUM)
			state = STATE_CMD;
		else
			state = STATE_IDLE;
		break;
	
	case STATE_CMD:
		// Process the (single byte) command
		cmd_received(data);
		state = STATE_IDLE;
		break;
	
	default:
		state = STATE_IDLE;
	}
	
	PORTA = state + 1;
}

/*
 * Command processing logic. Called by the state machine in byte_received().
 *
 * Single-byte Command Format
 * MSB [ . . . . . . | . . ] LSB
 *         command    index
 *
 * Commands
 *  1: read encoder #(index)
 *     Sends exactly 4 bytes.
 *     Encoder count at the time of the command will be sent out
 *      little-endian (lower-order byte first).
 *  2: reset encoder #(index) to 0
 *     Nothing is sent.
 *  3: set encoder #(index) to double-precision mode
 *  4: set encoder #(index) to single-precision mode
 */
void cmd_received(unsigned char data) {
	unsigned char cmd, index;
	long tmpCount;

	// Command processing logic
	cmd = data >> 2;
	index = data & 0x3;
	switch (cmd) {
	
	// 1: read encoder
	case 1:
		tmpCount = count[index];
		uart_tx((unsigned char)tmpCount);
		uart_tx((unsigned char)(tmpCount >> 8));
		uart_tx((unsigned char)(tmpCount >> 16));
		uart_tx((unsigned char)(tmpCount >> 24));
		break;
	
	// 2: reset encoder
	case 2:
		count[index] = 0L;
		break;
	
	// 3: set double-precision
	case 3:
		// enable interrupts on both edges
		if (index == 0) {
			MCUCR &= ~(1 << ISC11);
		} else if (index == 1) {
			MCUCR &= ~(1 << ISC01);
		} else if (index == 2) {
			config.count_both_edges_enc2 = TRUE;
		} else if (index == 3) {
			config.count_both_edges_enc3 = TRUE;
		}
		break;
	
	// 4: set single-precision
	case 4:
		// enable interrupts on only rising edge
		if (index == 0) {
			MCUCR |= (1 << ISC11);
		} else if (index == 1) {
			MCUCR |= (1 << ISC01);
		} else if (index == 2) {
			config.count_both_edges_enc2 = FALSE;
		} else if (index == 3) {
			config.count_both_edges_enc3 = FALSE;
		}
		break;
	}
}

void encoder_init(void) {
	// Set initial configuration
	config.count_both_edges_enc2 = TRUE;
	config.count_both_edges_enc3 = TRUE;
	
	// Configure pin directions
	DDRA &= ~0x3;	// Set PA0-1 to input and enable pullups
	PORTA |= 0x3;
	DDRB &= ~0x3;	// Set PB0-1 to input and enable pullups
	PORTB |= 0x3;
	DDRD &= ~0x3C;	// Set PD2-5 to input and enable pullups
	PORTD |= 0x3C;
	
	// Configure interrupts
	GIMSK |= (1 << INT1) | (1 << INT0) | (1 << PCIE0) | (1 << PCIE1);
	
	// Enable INT0 and INT1 interrupt on both rising and falling edge (initially)
	MCUCR |= (1 << ISC10) | (1 << ISC00);
	// Enable PCINT_B interrupt on pin PB0 (PCINT0)
	PCMSK = (1 << PCINT0);
	// Enable PCINT_A interrupt on pin PA0 (PCINT8)
	PCMSK1 = (1 << PCINT8);
	
	// Initialize counts to 0
	for (unsigned char i = 0; i < 4; i += 1)
		count[i] = 0L;
}

/*
 *  Master should wait 100us before sending next SPI byte.
 *  (Chip has a designed encoder frequency of 10kHz * 4 encoders.
 *  That leaves 2000 cycles in between interrupts. Therefore the encoder
 *  ISR must finish in < 500 cycles (that is, before the next
 *  set of interrupts). the theoretical maximum time.
 *  This is 100us when F_CPU is 20MHz.)
 *
 *  Assume the master will pause long enough before
 *  initiating receive. Do not check for transmit
 *  errors (will result in discarded bytes if
 *  master does not pause long enough).
 */
int main (void) {
	// disable clock prescaler
	CLKPR = 0x80;	// write change enable bit while clearing other bits
	CLKPR = 0;		// clear change enable bit while writing 0

#ifdef DEBUG
	// set PB2-7 to output for debug
	DDRB = 0xFC;
	PORTB |= 0x07;
#endif
	
	encoder_init();
	uart_init();
	
	// enable global interrupts
	sei();
	
	for (;;) {
#ifdef DEBUG
		// output lower 3 bits on PB4-7
		PORTB &= ~0xFC;
		PORTB |= ((unsigned char)count[0] << 2);
#endif
	}
}
