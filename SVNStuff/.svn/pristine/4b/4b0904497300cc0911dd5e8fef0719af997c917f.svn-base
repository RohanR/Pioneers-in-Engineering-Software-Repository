// Last Updated: 05/16/07 11:28


#include <avr/io.h>
#include "SPI.h"
#include "LCD.h"



// Initialize the LCD according to the procedure specified in its datasheet.
//  If you have the LCD connected to your X2, you should call this function
//  before attempting to use any of the user pushbuttons.  Before the LCD is
//  initialized it will drive the pushbutton lines high and give you false
//  button-press readings.
void LCDInit()
{ 
	// set the three LCD control pins to outputs
	LCD_COM_DDR |= ( 1 << LCD_RW ) | ( 1 << LCD_RS ) | ( 1 << LCD_E );

	// the busy flag cannot be used in this first section
	delay_ms( 30 );			// wait more than 15ms after Vcc rises to 4.5V
	LCDSendCommand( 0x30 );	// Function Set
	delay_ms( 5 );			// wait more than 4.1ms
	LCDSendCommand( 0x30 );	// Function Set
	delay_us( 150 );		// wait more than 100us
	LCDSendCommand( 0x30);	// Function Set

	// it is now possible to use the busy flag rather than fixed delays
	//  if so desired

	// these calls should be customized for your desired LCD settings 
	LCDSendCommand( 0x38 );	// 8-bit, 2 line, 5x8 dots char
	LCDSendCommand( 0x08 );	// display off, cursor off, blinking off
	LCDClear();
	LCDSendCommand( 0x06 );	// entry mode set (set cursor dir I, shift disable)
	LCDSendCommand( 0x0C );	// display on, cursor off, blinking off
}


void LCDWaitWhileBusy()
{
	unsigned char temp_ddr, read_data;
	temp_ddr = LCD_DATA_DDR;

	LCD_DATA_DDR = 0;

	// output the busy flag on LCD's DB7 (LCD_BF)
	LCD_COM_PORT = (LCD_COM_PORT & ~(1 << LCD_RS)) | (1 << LCD_RW);
	do
	{
		LCD_COM_PORT |= (1 << LCD_E);
		asm(						// delay at least 120ns
			"nop" "\n\t"			//  each nop is 50ns with IO clk = 20MHz
			"nop" "\n\t"
			"nop" "\n\t"
			::);
		read_data = LCD_DATA_PIN;
		LCD_COM_PORT &= ~(1 << LCD_E);
		asm(							// E cycle time is minimum of 500ns
		"nop" "\n\t"
		"nop" "\n\t"
		"nop" "\n\t"
		"nop" "\n\t"
		"nop" "\n\t"
		"nop" "\n\t"
		"nop" "\n\t"
		"nop" "\n\t"
		"nop" "\n\t"
		"nop" "\n\t"
		"nop" "\n\t"
		"nop" "\n\t"
		::);
	}
	while (read_data & (1 << LCD_BF));	// while LCD is busy, wait

	LCD_DATA_DDR = temp_ddr;
}


// Send a command or data byte to the LCD
void LCDSend( unsigned char data, unsigned char RS_bv )
{ 
	unsigned char temp_ddr, temp_port;

	LCDWaitWhileBusy();

	temp_ddr = LCD_DATA_DDR;		// save current DDRC state
	temp_port = LCD_DATA_PORT;		// save current PORTC state

	LCD_COM_PORT &= ~(( 1 << LCD_RW ) | ( 1 << LCD_RS ));// LCD write, clear RS
	LCD_COM_PORT |= RS_bv;			// select command or data send
	LCD_DATA_DDR = 0xFF;			// LCD data pins set as outputs
	LCD_DATA_PORT = 0;
	LCD_COM_PORT |= ( 1 << LCD_E );	// set LCD chip-enable signal
	asm( "nop" "\n\t" :: );			// delay briefly
	LCD_DATA_PORT = data;			// send data
	asm(							// enable line must stay high for > 230ns
		"nop" "\n\t"				//   each nop is 50ns with IO clk = 20MHz
		"nop" "\n\t"
		"nop" "\n\t"
		:: );
   LCD_COM_PORT &= ~( 1 << LCD_E );	// end chip-enable signal
   LCD_DATA_PORT = temp_port;		// restore original PORTC state
   LCD_DATA_DDR = temp_ddr;			// restore original DDRC state
}


void LCDSendCommand( unsigned char command )
{
	LCDSend( command, 0 );
}


void LCDSendData( unsigned char data )
{
	LCDSend( data, 1 << LCD_RS );
}


// display a character string
void LCDAddString( const char *str )
{
	while ( *str != 0 )
		LCDSendData( *str++ );
} 


// clear the LCD, then display a character string
void LCDString( const char *str )
{
	LCDClear();
	LCDAddString( str );
}


// display a byte in hex 
void LCDHex( unsigned char byte )
{
	unsigned char nibble;

	// display high nibble
	nibble = byte >> 4;
	if ( nibble < 10 )
		LCDSendData( '0' + nibble );
	else
		LCDSendData( 'A' + ( nibble - 10 ));

	// display low nibble
	nibble = byte & 0x0F;
	if ( nibble < 10 )
		LCDSendData( '0' + nibble );
	else
		LCDSendData( 'A' + ( nibble - 10 ));
}


// display a byte in binary
void LCDBinary( unsigned char byte )
{
	unsigned char i, bitmask;

	bitmask = 1 << 7;
	for ( i = 0; i < 8; i++ )
	{
		if ( byte & bitmask )
			LCDSendData( '1' );
		else
			LCDSendData( '0' );
		bitmask >>= 1;
	}
}


// display an unsigned integer in decimal
void LCDUInt( unsigned int word )
{
	unsigned char str[5];
	unsigned char i = 5;
	unsigned char digit;

	unsigned int val = word;
	do
	{
		digit = val;
		val /= 10;
		digit -= val * 10;
		str[--i] = '0' + digit;
	}
	while ( val > 0 );

	for( ; i < 5; i++ )
		LCDSendData( str[i] );
}


// display a signed integer in decimal
void LCDInt( int word )
{
	if (word < 0)
	{
		LCDSendData('-');
		word = -word;
	}

	LCDUInt( (unsigned int) word );
}
