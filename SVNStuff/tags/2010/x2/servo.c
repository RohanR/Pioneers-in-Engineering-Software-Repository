/* servo.c
 *
 * AUTHOR: John Wang
 * VERSION: 1.0 (25 Oct 2009)
 *
 * DESCRIPTION:
 */

#include <avr/io.h>

#define NUM_SERVOS 2

/* Servo pins are assigned indexes:
 *  0  =  D4
 *  1  =  D5
 */

// Maps servo index to their output compare registers
volatile unsigned int *OCRnx[NUM_SERVOS] = {
	&OCR1B,
	&OCR1A
};


// 1/8 prescaler
#define TICKS_PER_uS  (F_CPU / 1000000UL) / 8UL

#define MIN_PULSE_uS		544UL	// the shortest pulse sent to a servo  
#define DEFAULT_PULSE_uS	1500UL	// default pulse width when servo is attached
#define MAX_PULSE_uS		2400UL	// the longest pulse sent to a servo 


void servo_init () {
	TCCR1A = (1 << WGM11);	// Fast PWM mode, count up to ICR1
	TCCR1B = (1 << WGM13) | (1 << WGM12);
	TCCR1B |= _BV(CS11);     // set prescaler of 8 
	TCNT1 = 0;		// clear timer
	ICR1 = 0xFFFF;	// set TOP to MAX
}

void servo_enable (unsigned char index) {
	DDRD |= (1 << (PD4 + index));
	switch (index) {
	case 0:
		TCCR1A |= (1 << COM1B1);
		break;
		
	case 1:
		TCCR1A |= (1 << COM1A1);
		break;
	}
}

void servo_disable (unsigned char index) {
	switch (index) {
	case 0:
		TCCR1A &= ~(0b11 << COM1B0);
		break;
		
	case 1:
		TCCR1A &= ~(0b11 << COM1A0);
		break;
	}
}

void servo_setPos (unsigned char index, unsigned char value) {
	unsigned int ticks = ((unsigned long)value * (MAX_PULSE_uS - MIN_PULSE_uS) / 255UL + MIN_PULSE_uS) * TICKS_PER_uS;
	switch (index) {
	case 0:
		OCR1BH = ticks >> 8;
		OCR1BL = ticks & 0xFF;
		break;

	case 1:
		OCR1AH = ticks >> 8;
		OCR1AL = ticks & 0xFF;
		break;
	}
}
