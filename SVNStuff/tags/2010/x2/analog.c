// analog
//
// Tom Benedict


// The ATMega processors that are used by the Orangutan family of
// robot controllers have eight analog inputs on them.  Most of these
// can also double as digital I/O lines, but for now let's focus on
// the analog inputs.
//
// The ATMega only has one analog to digital converter, however.  The
// way each of the lines is converted in turn is by using a MUX.  The
// idea is you tell the MUX which pin to connect to the ADC, then you
// tell the ADC to do the conversion.
//
// The ADC on the ATMega is very flexible.  Among other things you
// can power it on, power it off (saves a lot of power), sample
// continously, sample when polled, sample when triggered by a timer
// compare, etc.  It will do eight or ten-bit conversions, and can
// run conversions at a whole host of speeds (faster means more
// throughput but more noise.)
//
// Please don't take this example as the One True Way to use the
// ATMega's ADC.  It's ONE way to use it.  But it's a pretty convenient
// way at that.
//
// The way this example uses the ADC is to do fairly slow, fairly low
// noise 10-bit conversions.  If this isn't right for your application,
// by all means tweak things until they do what you want.  And by all
// means read the ATMega datasheet for the device you're programming.
// The chapter on the ADC is quite good, goes into every little detail,
// and even has example code in C and Assembly.
//
// For this example we'll be reading the on-board potentiometer hooked
// up to ADC7.  But by changing what channel you're reading you can use
// it to look at any analog device you've got hooked up to an ADC 
// input.
//
// So let's get to it!


// All device-specific dependencies are stored in this file.  You must
// edit it first before using any of the library routines!



#include <avr/io.h>


// Initialize the ADC to do 10-bit conversions:
void analog_init(void) 
{
	// The Orangutans have an external reference voltage on the AREF
	// pin tied to our +5V regulated Vcc.  We want to set our ADC to
	// use this as our reference.  The ADMUX register needs REFS0
	// set to 1, and REFS1 set to zero for this mode.
	ADMUX = (1 << REFS0);

	// The ADC Control and Status Register A sets up the rest of 
	// what we need.  Three bits ADPS0, 1, and 2, set the prescale
	// for how fast our conversions are done.  In this example
	// we use CPU/64, or mode 6:

	ADCSRA = (6 << ADPS0);

	// Conversions take 13 ADC cycles to complete the sample and hold.
	// Dividing our CPU by 64 gives us the following samples/sec for
	// our devices:
	//
	// Orangutan	 9615
	// Baby-O		24038
	// X2			24038

	// Finally, we enable the ADC subsystem:

	ADCSRA |= (1 << ADEN);
}


// Read out the specified analog channel to 10 bits
uint16_t analog10(uint8_t channel) 
{
	// Begin by setting up the MUX so it knows which channel
	// to connect to the ADC:

	// Clear the channel selection (low 5 bits in ADMUX)
	ADMUX &= ~0x1F;

	// Select the specified channel
	ADMUX |= channel;

	// Now we initiate a conversion by telling the ADC's
	// control register to start conversion (ADSC):

	// ADC start conversion
	ADCSRA |= (1 << ADSC);

	// We wait for conversion to complete by watching the
	// ADSC bit on the ADCSRA register.  When it goes away,
	// the conversion is done:
	while (bit_is_set(ADCSRA, ADSC));

	// Since we're reading out ten bits, we have to read 
	// the results out of two different registers: ADCL for
	// the low byte, and ADCH for the high byte.  Caution
	// here:  We have to read ADCL before we read ADCH.
	// Since they're both just bytes, we have to shift the
	// high byte over by 8-bits and or the two together
	// to make the full 10-bit value:

	return (ADCL | ADCH << 8);	// read ADC (full 10 bits);
}


// And now for the maximum in cheesiness:  How do you do an
// 8-bit conversion if you're in 10-bit mode?  Simple!  Do a
// 10-bit conversion and ditch the two least significant bits.
// Laugh all you want, that's how AVRLIB does it, too.

// Read out the specified analog channel to 8 bits
uint8_t analog8(uint8_t channel) 
{
	return((analog10(channel) >> 2) & 0xFF);
}


// That's it for the ADC stuff.  Remember, there are lots of other
// ways the ADC subsystem can be used.  This is just ONE way.  If it
// doesn't meet your needs, read the data sheet and experiment.
