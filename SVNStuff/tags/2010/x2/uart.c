/* uart.c
 *
 * AUTHOR: John Wang
 * VERSION: 1.0  (15 Mar 2010)
 *
 * DESCRIPTION:
 *  Handles UART communications with the master processor.
 *  Based on Atmel Application Note AVR306: Using the AVR UART in C
 */

#include <avr/io.h>
#include <avr/interrupt.h>

#include "uart.h"

static unsigned char UART_RxBuf[UART_RX_BUFFER_SIZE];
static volatile unsigned char UART_RxHead;
static volatile unsigned char UART_RxTail;
static unsigned char UART_TxBuf[UART_TX_BUFFER_SIZE];
static volatile unsigned char UART_TxHead;
static volatile unsigned char UART_TxTail;


void uart_init () {
	// Enable transmit and receive, and receive interrupt
	UCSR0B |= (1 << RXEN0) | (1 << TXEN0) | (1 << RXCIE0);
	// 8 data bits, 2 stop bits, no parity
	UCSR0C |= (1 << USBS0) | (1 << UCSZ00) | (1 << UCSZ01);

	// Set the baudrate
	UBRR0L = BAUD_PRESCALE;
	UBRR0H = (BAUD_PRESCALE >> 8);

	// Initialize circular buffers
	UART_RxTail = 0;
	UART_RxHead = 0;
	UART_TxTail = 0;
	UART_TxHead = 0;
}

ISR(USART0_RX_vect) {
	unsigned char data, tmphead;
	
	// Read byte
	data = UDR0;

	// Update circular buffer indices
	tmphead = ( UART_RxHead + 1 ) & UART_RX_BUFFER_MASK;
	UART_RxHead = tmphead;
	if ( tmphead == UART_RxTail ) {
		// ERROR! Receive buffer overflow
	}

	// Store received data in buffer
	UART_RxBuf[tmphead] = data; 
}

ISR(USART0_UDRE_vect) {
	unsigned char tmptail;

	// If there is data in the transmit buffer
	if (UART_TxHead != UART_TxTail) {
		// Update circular buffer indices
		tmptail = (UART_TxTail + 1) & UART_TX_BUFFER_MASK;
		UART_TxTail = tmptail;

		// Transmit byte
		UDR0 = UART_TxBuf[tmptail];
	} else {
		// Disable transmit interrupt
		UCSR0B &= ~(1 << UDRIE0);
	}
}

unsigned char uart_hasData() {
	return (UART_RxHead != UART_RxTail);
}

/* This function WILL block for data; check if
   uart_hasData() is true before calling.
*/
unsigned char uart_rx () {
	unsigned char tmptail;

	// Wait for data
	while (UART_RxHead == UART_RxTail);

	// Increment the tail index
	tmptail = (UART_RxTail + 1) & UART_RX_BUFFER_MASK;
	UART_RxTail = tmptail;

	return UART_RxBuf[tmptail];
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
	UCSR0B |= (1<<UDRIE0);
}
