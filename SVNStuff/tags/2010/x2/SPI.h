// Sample wrapper functions for sending SPI commands from the mega644 to
//  the mega168.  This source is considered public domain.  We encourage
//  you to understand how these functions work and edit them to suit your
//  particular needs (or to use them as a starting point for writing your own).
//  These wrappers are designed to work with X2 firmware version 1.01.
//  setMxBrakeDuration(), setMxCurrentLimit(), and waitWhileEEPROMBusy() will
//  not work correctly for X2 firmware version 1.00.

// Version: 1.01
// Last Updated: 02/15/08 15:16


#ifndef __SPI_H__
#define __SPI_H__

#include <avr/io.h>


#ifndef SPCR	// older versions of WinAVR define SPCR0, not SPCR

#define SPCR		SPCR0
#define SPE			SPE0
#define MSTR		MSTR0
#define SPR0		SPR00
#define SPSR		SPSR0
#define SPIF		SPIF0
#define SPDR		SPDR0

#endif  // SPCR


// COMMANDS
#define CMD_MOTOR1_BRAKE_LOW			128
#define CMD_MOTOR1_BRAKE_HIGH			130
#define CMD_MOTOR2_BRAKE_LOW			132
#define CMD_MOTOR2_BRAKE_HIGH			134
#define CMD_MOTOR1_FORWARD				136
#define CMD_MOTOR1_REVERSE				138
#define CMD_MOTOR2_FORWARD				140
#define CMD_MOTOR2_REVERSE				142
#define CMD_JOINT_BRAKE					144	// 150 will also joint-brake low
#define CMD_JOINT_FORWARD				146
#define CMD_JOINT_REVERSE				148
#define CMD_JOINT_ACCEL_FORWARD			228
#define CMD_JOINT_ACCEL_REVERSE			230
#define CMD_MOTOR1_ACCEL_FORWARD		232
#define CMD_MOTOR1_ACCEL_REVERSE		234
#define CMD_MOTOR2_ACCEL_FORWARD		236
#define CMD_MOTOR2_ACCEL_REVERSE		238

#define CMD_ENABLE_JOINT_MOTOR_MODE		215
#define CMD_SET_M1_ACCELERATION			208
#define CMD_SET_M2_ACCELERATION			209
#define CMD_SET_M1_BRAKE_DURATION		188
#define CMD_SET_M2_BRAKE_DURATION		190
#define CMD_SET_PWM_FREQUENCIES			210
#define CMD_SET_NUM_CURRENT_SAMPLES		212
#define CMD_SET_M1_CURRENT_LIMIT		192
#define CMD_SET_M2_CURRENT_LIMIT		194

#define CMD_GET_M1_CURRENT				216
#define CMD_GET_M2_CURRENT				217


#define CMD_BUZZER_OFF					225
#define CMD_PLAY_NOTE					152
#define CMD_PLAY_FREQUENCY				160

#define CMD_PLAY_MELODY					176
#define CMD_STORE_NOTE					168
#define CMD_END_MELODY					224
#define CMD_ERASE_MELODIES				186

#define CMD_SET_VOLUME					226
#define CMD_SET_NOTE_GAP				187


#define CMD_SEND_SERIAL					220
#define CMD_READ_SERIAL					219

#define CMD_SET_SERIAL					200
#define CMD_SET_AND_SAVE_SERIAL			201
#define CMD_SET_READ_READY_SIZE			227

#define CMD_GET_SEND_BUFF_FREE_SPACE	222
#define CMD_GET_READ_BUFF_NUM_BYTES		223
#define CMD_GET_UART_ERROR				252


#define CMD_WRITE_EEPROM				240
#define CMD_READ_EEPROM					248
#define CMD_GET_EEPROM_BUSY				254


#define CMD_GET_STATUS					218
#define CMD_GET_FIRMWARE_VERSION		253



// argument options for setPWMFrequencies() function
//  PWM frequency = 20MHz / prescaler / 2^bit-resolution
#define	RESOLUTION_7BIT				0		// pwm ranges from 0 - 127
#define RESOLUTION_8BIT				1		// pwm ranges from 0 - 255
#define PRESCALER_8					0		// 20MHz / 8
#define PRESCALER_64				1		// 20MHz / 64
#define PRESCALER_256				2		// 20MHz / 256
#define PRESCALER_1024				3		// 20MHz / 1024



// buzzer note macros: key( octave )
#define C( x )					(  0 + x*12 )
#define C_SHARP( x )			(  1 + x*12 )
#define D_FLAT( x )				(  1 + x*12 )
#define D( x )					(  2 + x*12 )
#define D_SHARP( x )			(  3 + x*12 )
#define E_FLAT( x )				(  3 + x*12 )
#define E( x )					(  4 + x*12 )
#define F( x )					(  5 + x*12 )
#define F_SHARP( x )			(  6 + x*12 )
#define G_FLAT( x )				(  6 + x*12 )
#define G( x )					(  7 + x*12 )
#define G_SHARP( x )			(  8 + x*12 )
#define A_FLAT( x )				(  8 + x*12 )
#define A( x )					(  9 + x*12 )
#define A_SHARP( x )			( 10 + x*12 )
#define B_FLAT( x )				( 10 + x*12 )
#define B( x )					( 11 + x*12 )

#define SILENT_NOTE				0xFF


// UART error byte bits
#define UART_SEND_BUFF_OVERRUN		1		// UART send buffer overflow
#define UART_READ_BUFF_OVERRUN		2		// UART read buffer overflow
#define UART_FRAME_ERROR			4
#define UART_DATA_OVERRUN			8
#define UART_PARITY_ERROR			16
#define UART_READ_BUFF_UNDERRUN		32		// tried reading from empty buffer


// argument options for the setSerial() function
#define UART_READ_BUFF_SZ			32	// UART read buffer can hold 32 bytes
#define UART_SEND_BUFF_SZ			32	// UART send buffer can hold 32 bytes

#define UART_NO_PARITY				0
#define UART_EVEN_PARITY			2
#define UART_ODD_PARITY				3

#define UART_ONE_STOP_BIT			0
#define UART_TWO_STOP_BITS			1

#define UART_NORMAL_SPEED			0
#define UART_DOUBLE_SPEED			1

// UBRR values that achieve various bauds at normal speed
#define UBRR_115200_BAUD			10	// -1.4% error
#define UBRR_76800_BAUD				15	// 1.7% error
#define UBRR_57600_BAUD				21	// -1.4% error
#define UBRR_38400_BAUD				32	// -1.4% error
#define UBRR_28800_BAUD				42	// .9% error
#define UBRR_19200_BAUD				64	// .2% error
#define UBRR_14400_BAUD				86	// -.2% error
#define UBRR_9600_BAUD				129	// .2% error
#define UBRR_4800_BAUD				259	// .2% error
#define UBRR_2400_BAUD				520	// 0% error



// mega168's EEPROM addresses for settings
#define ADDR_INIT_CHECK				0	// check to see if EEPROM initialized
#define ADDR_M1_PWM_FREQUENCY		1	// freq + resolution of PWM1 (timer0)
#define ADDR_M2_PWM_FREQUENCY		2	// freq + resolution of PWM2 (timer2)
#define ADDR_M1_CURRENT_SAMPLES		3	// number of M1 ADC samples to avg
										//   must be a power of 2 <= 128
#define ADDR_M2_CURRENT_SAMPLES		4	// number of M2 ADC samples to avg
										//   must be a power of 2 <= 128
#define ADDR_M1_CURRENT_LIMIT		5	// 0 = no limit
#define ADDR_M1_CL_P_CONST			6	// 7-bit P val: pwm += P * (CL - cur)
										//   0 = shutdown motor on overlimit
#define ADDR_M2_CURRENT_LIMIT		7	// 0 = no limit
#define ADDR_M2_CL_P_CONST			8	// 7-bit P val: pwm += P * (CL - cur)
										//   0 = shutdowm motor on overlimit
#define ADDR_M1_ACCELERATION		9	// M1 acceleration; 0 = instantaneous
#define ADDR_M2_ACCELERATION		10	// M2 acceleration; 0 = instantaneous
#define ADDR_M1_BRAKE_DURATION		11	// M1 time to spend hard braking (ms)
#define ADDR_M2_BRAKE_DURATION		12	// M2 time to spend hard braking (ms)
#define ADDR_SERIAL_SETTINGS		13	// parity, stop bits, 2x speed, p. mode
#define ADDR_SERIAL_UBRRH			14	// UBRRH register (determines baud)
#define ADDR_SERIAL_UBRRL			15	// UBRRL register (determines baud)
#define ADDR_SERIAL_READ_READY		16	// UART read buffer ready for reading
#define ADDR_BUZZER_VOLUME			17	// buzzer volume
#define ADDR_STARTUP_MELODY			18	// melody that plays on startup
										//  0 - 7 = melody, 8 = silence
										//  else chirp
#define ADDR_NOTE_GAP				19	// default duration in ms of silent
										//   pause inserted after each note
#define ADDR_SCK_DURATION			20	// programmer SPI SCK setting
#define ADDR_ISP_STATE				21  // 168 ISP state (in progmode or not)
#define ADDR_ISP_SW_MINOR			22	// ISP version major byte
#define ADDR_ISP_SW_MAJOR			23	// ISP version minor byte


// no pointer to the start of melody0 is needed as location never changes
#define ADDR_MELODY_START_PTR_MSBS	24	// bit i is MSB of melody i+1 pointer
#define ADDR_MELODY1_START_PTR		25	// address of pointer to melody1 start
#define ADDR_MELODY2_START_PTR		26	// melody 2
#define ADDR_MELODY3_START_PTR		27	// melody 3
#define ADDR_MELODY4_START_PTR		28	// melody 4
#define ADDR_MELODY5_START_PTR		29	// melody 5
#define ADDR_MELODY6_START_PTR		30	// melody 6
#define ADDR_MELODY7_START_PTR		31	// melody 7
#define ADDR_MELODY7_END_PTR		32	// ptr to end of melody 7

// address of 1st note of melody 0
#define ADDR_MELODY0_START			33

// there is room in EEPROM for 159 notes, distributed in any way amongst the
// eight melodies.  The mega168's EEPROM is 512 bytes in size.


// status byte bits
#define STATUS_UART_ERROR			1	// cleared only on status byte read
#define STATUS_UART_READ_READY		2	// value always reflects current state
#define STATUS_UART_SEND_FULL		4	// value always reflects current state
#define STATUS_BUZZER_FINISHED		8	// value always reflects current state
#define STATUS_M1_FAULT				16	// cleared only on status byte read
#define STATUS_M1_CURRENT_HIGH		32	// cleared only on status byte read
#define STATUS_M2_FAULT				64	// cleared only on status byte read
#define STATUS_M2_CURRENT_HIGH		128	// cleared only on status byte read




// Delay utility functions
void delay_ms(unsigned int milliseconds);

static inline void delay_us(unsigned int microseconds) 
											__attribute__((always_inline));
void delay_us(unsigned int microseconds)
{
	__asm__ volatile (
		"1: push r22"     "\n\t"
		"   ldi  r22, 4"  "\n\t"
		"2: dec  r22"     "\n\t"
		"   brne 2b"      "\n\t"
		"   pop  r22"     "\n\t"
		"   sbiw %0, 1"   "\n\t"
		"   brne 1b"
		: "=w" ( microseconds )
		: "0" ( microseconds )
	);
}



// ************************************************************************
//                         SPI Command Functions
// ************************************************************************

// *** See the function implementations in SPI.c for comments ***


// Loop until any current SPI transmissions have completed
#define waitForTransmission()			while ( ! ( SPSR & ( 1 << SPIF )))


// lowest-level SPI functions
void SPIInit(void);
void SPITransmit( unsigned char data );
unsigned char SPIReceive( unsigned char data );



// *************** MOTOR COMMANDS ***************

void sendMotorPWMCommand( unsigned char command, unsigned char pwm );

// motor 1 commands
void setMotor1( int speed );
void accelMotor1( int speed );
void brakeLowMotor1( unsigned char pwm );
void brakeHighMotor1( unsigned char pwm );

// motor 2 commands
void setMotor2( int speed );
void accelMotor2( int speed );
void brakeLowMotor2( unsigned char pwm );
void brakeHighMotor2( unsigned char pwm );

// joint-motor commands
void setJointMotor( int speed );
void accelJointMotor( int speed );
void brakeJointMotor( unsigned char pwm );

// motor settings
void enableJointMotorMode(void);

void setM1Acceleration( unsigned char accel );
void setM2Acceleration( unsigned char accel );

void setM1BrakeDuration( unsigned char brakeDuration );
void setM2BrakeDuration( unsigned char brakeDuration );

void setPWMFrequencies( unsigned char M1Resolution, unsigned char M1Prescaler,
						unsigned char M2Resolution, unsigned char M2Prescaler);

void setNumCurrentSamples( unsigned char M1Exponent, unsigned char M2Exponent);

void setM1CurrentLimit( unsigned char limit, unsigned char P );
void setM2CurrentLimit( unsigned char limit, unsigned char P );

// motor info
unsigned char getM1Current(void);
unsigned char getM2Current(void);



// *************** BUZZER COMMANDS ***************

#define waitWhileBuzzerBusy()												\
	while ( !( getStatus() & STATUS_BUZZER_FINISHED )) delay_us( 20 )

// simple buzzer commands
void buzzerOff(void);
void playNote( unsigned char note, unsigned int duration );
void playFrequency( unsigned int frequency, unsigned int duration );

// melody commands
void playMelody( unsigned char melody );
void storeNote( unsigned char note, unsigned int duration );
void endMelody(void);
void eraseMelodies(void);

// buzzer settings
void setVolume( unsigned char volume );
void setNoteGap( unsigned char noteGap );



// *************** UART COMMANDS ***************

#define waitUntilUARTReadReady()											\
	while ( !( getStatus() & STATUS_UART_READ_READY )) delay_us( 20 )

// transmit/receive commands
void sendSerial( unsigned char data );
unsigned char readSerial(void);
void fastSerialRead( unsigned char numBytes, unsigned char data[] );

// serial settings
void enablePermanentProgMode(void);
void setSerial( unsigned char parity, unsigned char stopBits,
			   unsigned char speedMode, unsigned int UBRR );
void setAndSaveSerial( unsigned char parity, unsigned char stopBits,
			   unsigned char speedMode, unsigned int UBRR );
void setReadReadySize( unsigned char rrSize );

// serial info
unsigned char getSendBuffFreeSpace(void);
unsigned char getReadBuffNumBytes(void);
unsigned char getSerialError(void);



// *************** EEPROM COMMANDS ***************

#define waitWhileEEPROMBusy()		while ( getEEPROMBusy() ) delay_us( 20 )

void writeEEPROM( unsigned int address, unsigned char data );
unsigned char readEEPROM( unsigned int address );
unsigned char getEEPROMBusy(void);




// *************** MISC COMMANDS ***************
unsigned char getStatus(void);
void getFirmwareVersion( unsigned char *vMajor, unsigned char *vMinor );



// *************** SAVE SETTINGS COMMANDS ***************
void restoreDefaultSettings(void);

void saveM1PWMFrequency( unsigned char resolution, unsigned char prescaler );
void saveM2PWMFrequency( unsigned char resolution, unsigned char prescaler );

void saveM1CurrentSamples( unsigned char samples );
void saveM2CurrentSamples( unsigned char samples );

void saveM1CurrentLimit( unsigned char limit, unsigned char P );
void saveM2CurrentLimit( unsigned char limit, unsigned char P );

void saveM1Acceleration( unsigned char accel );
void saveM2Acceleration( unsigned char accel );

void saveM2BrakeDuration( unsigned char brakeDuration );
void saveM1BrakeDuration( unsigned char brakeDuration );

void saveSerialSettings( unsigned char parity, unsigned char stopBits,
						 unsigned char speedMode, unsigned int UBRR );
void saveEnablePermanentProgMode(void);
void saveReadReadySize( unsigned char readReadySize );

void saveVolume( unsigned char volume );
void saveNoteGap( unsigned char noteGap );
void saveStartupMelody( unsigned char melody );

void saveAVRISPVersion( unsigned char vMajor, unsigned char vMinor );




#endif //__SPI_H__
