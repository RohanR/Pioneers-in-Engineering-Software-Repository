// Last Updated: 05/16/07 11:28

#ifndef __LCD_H__
#define __LCD_H__


// special function register mapping 
#define LCD_DATA_DDR	DDRC		// LCD data DDR 
#define LCD_COM_DDR		DDRB		// LCD control DDR 
#define LCD_DATA_PORT	PORTC		// LCD data PORT 
#define LCD_COM_PORT	PORTB		// LCD control PORT 
#define LCD_DATA_PIN	PINC		// LCD data PIN 

// LCD control/status pins
#define LCD_RS			PB0	// register selector (H: DATA, L: instruction code)
#define LCD_RW			PB1	// read/write pin (H: Read, L: Write) 
#define LCD_E			PB3	// chip enable signal pin 
#define LCD_BF			PC7	// data bit 7/busy flag


#define LCD_ROW_0		0x80	// DDRAM address of top row
#define LCD_ROW_1		0xC0	// DDRAM address of second row
#define LCD_ROW_2		0x94	// DDRAM address of third row
#define LCD_ROW_3		0xD4	// DDRAM address of bottom row


// some convenient LCD commands
#define LCDClear()				LCDSendCommand( 0x01 )	// clear LCD, go home
#define LCDHome()				LCDSendCommand( 0x02 )	// cursor-> row 0 col 0
#define LCDMoveCursor( x )		LCDSendCommand( x )		// x = LCD_ROW_# + col
#define LCDMoveCursorLeft()		LCDSendCommand( 0x10 )	// cursor left one
#define LCDMoveCursorRight()	LCDSendCommand( 0x14 )	// cursor right one

#define LCDChar( c )			LCDSendData( c )		// display character c

// function prototypes 
void LCDInit(void); 
void LCDWaitWhileBusy(void); 
void LCDSend( unsigned char data, unsigned char RS_bv ); 
void LCDSendCommand( unsigned char command ); 
void LCDSendData( unsigned char data ); 
void LCDAddString( const char *str ); 
void LCDString( const char *str ); 
void LCDHex( unsigned char byte ); 
void LCDBinary( unsigned char byte ); 
void LCDUInt( unsigned int word ); 
void LCDInt( int word );


#endif
