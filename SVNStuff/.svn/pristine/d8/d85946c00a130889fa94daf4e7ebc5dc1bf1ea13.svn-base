/* uart.c
 *
 * AUTHOR: John Wang
 * VERSION: 1.1  (16 Feb 2011)
 * HISTORY: 1.0  (15 Mar 2010) - For ATmega644 / Pololu Orangutan X2
 *
 * DESCRIPTION:
 *  Interrupt-based UART handling for ATtiny2313.
 *
 *  Handles UART communications with the master processor.
 *  Based on Atmel Application Note AVR306: Using the AVR UART in C
 */

#include <avr/io.h>
#include <avr/interrupt.h>

#include "main.h"
#include "uart.h"

static unsigned char UART_TxBuf[UART_TX_BUFFER_SIZE];
static volatile unsigned char UART_TxHead;
static volatile unsigned char UART_TxTail;


void uart_init () {
	// Set the data directions (for ATtiny2313)
	//  PD0 = Rx (input, enable pullups)
	//  PD1 = Tx (output)
	DDRD &= ~(1 << PD0);
	PORTD |= (1 << PD0);
	DDRD |= (1 << PD1);

	// Enable transmit and receive, and receive interrupt
	UCSRB |= (1 << RXEN) | (1 << TXEN) | (1 << RXCIE);
	// 8 data bits, 2 stop bits, no parity
	UCSRC |= (1 << USBS) | (1 << UCSZ0) | (1 << UCSZ1);

	// Set the baudrate
	UBRRL = BAUD_PRESCALE;
	UBRRH = (BAUD_PRESCALE >> 8);

	// Initialize circular buffers
	UART_TxTail = 0;
	UART_TxHead = 0;
}

/* Byte received interrupt: call byte_received() defined in main.
 */
ISR(USART_RX_vect) {
	unsigned char data;

	data = UDR;
	byte_received(data);
}

ISR(USART_UDRE_vect) {
	unsigned char tmptail;

	// If there is data in the transmit buffer
	if (UART_TxHead != UART_TxTail) {
		// Update circular buffer indices
		tmptail = (UART_TxTail + 1) & UART_TX_BUFFER_MASK;
		UART_TxTail = tmptail;

		// Transmit byte
		UDR = UART_TxBuf[tmptail];
	} else {
		// Disable transmit interrupt
		UCSRB &= ~(1 << UDRIE);
	}
}

/* This function WILL block if the transmit buffer is full.
 */
void uart_tx (unsigned char byte) {
	unsigned char tmphead;

	// Calculate buffer index
	tmphead = ( UART_TxHead + 1 ) & UART_TX_BUFFER_MASK; 

	// Wait for free space in buffer
	while ( tmphead == UART_TxTail );

	// Store data in buffer
	UART_TxBuf[tmphead] = byte;
	UART_TxHead = tmphead;

	// Enable UDRE interrupt
	UCSRB |= (1<<UDRIE);
}
