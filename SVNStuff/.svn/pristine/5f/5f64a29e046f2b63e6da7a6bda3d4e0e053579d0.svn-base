// Sample wrapper functions for sending SPI commands from the mega644 to
//  the mega168.  This source is considered public domain.  We encourage
//  you to understand how these functions work and edit them to suit your
//  particular needs (or to use them as a starting point for writing your own).
//  These wrappers are designed to work with X2 firmware version 1.01.
//  setMxBrakeDuration(), setMxCurrentLimit(), and waitWhileEEPROMBusy() will
//  not work correctly for X2 firmware version 1.00.

// Version: 1.01
// Last Updated: 08/06/07 12:46


#include <avr/io.h>
#include "SPI.h"



// Delay utility function -- delays for time_ms milliseconds by looping
void delay_ms(unsigned int time_ms)
{
	if (time_ms == 0)
		return;

	__asm__ volatile (
		"1: push r22"		"\n\t"
		"   ldi  r22, 84"	"\n\t"
		"2: push r23"		"\n\t"
		"   ldi  r23, 77"	"\n\t"
		"3: dec  r23"		"\n\t"
		"   brne 3b"		"\n\t"
		"   pop  r23"		"\n\t"
		"   dec  r22"		"\n\t"
		"   brne 2b"		"\n\t"
		"   pop  r22"		"\n\t"
		"   sbiw %0, 1"		"\n\t"
		"   brne 1b"
		: "=w" (time_ms)
		: "0" (time_ms)
	);
}



// ***********************************************************************
// 						Lowest-level SPI Functions
// ***********************************************************************

// This global flag is used in conjunction with the SPIF bit of SPCR to
//  determine if an SPI transmission is in progress.  It is used only in
//  SPIInit(), SPITransmit(), and SPIReceive().
unsigned char SPITransmitting;


// Initialize the SPI for communication with slave mega168
void SPIInit()
{
	// make the MOSI & SCK pins outputs
	DDRB |= ( 1 << PB5 ) | ( 1 << PB7 ) | ( 1<< PB4 );

	// make sure the MISO pin is input
	DDRB &= ~( 1 << PB6 );

	// set up the SPI module: SPI enabled, MSB first, master mode,
	//  clock polarity and phase = 0, F_osc/8
	SPCR = ( 1 << SPE ) | ( 1 << MSTR ) | ( 1 << SPR0 );
	SPSR = 1;     // set double SPI speed for F_osc/8

	// the previous settings clear SPIF, but we use this flag to indicate
	//  that this does not mean we are currently transmitting
	SPITransmitting = 0;
}


// Transmit a byte to the mega168
void SPITransmit( unsigned char data )
{
	if ( SPITransmitting )				// prevent false SPIF clears from
		waitForTransmission();			//  hanging the wait loop
	SPDR = data;						// start transmission of data
	SPITransmitting = 1;				// flag transmission is in progress
}


// Initiate an exchange of data with the mega168 by transmitting a byte
unsigned char SPIReceive( unsigned char data )	// data is often a junk byte
{
	if ( SPITransmitting )
		waitForTransmission();				// #define in SPI.h
    delay_us( 3 );							// give the mega168 time to prepare
											//  return data
    SPDR = data;							// start bidirectional transfer
	waitForTransmission();					// #define in SPI.h

	// reading SPCR and SPDR will clear SPIF, so we will use this flag to
	//  indicate that this does not mean we are currently transmitting
	SPITransmitting = 0;
	return SPDR;
}




// ***********************************************************************
// 							Motor Commands
// ***********************************************************************


// The pwm byte is tacked onto all motor commands in the following way:
void sendMotorPWMCommand( unsigned char command, unsigned char pwm )
{
	// the MSB of the speed gets tacked onto the command byte
	command |= ( pwm & 0x80 ) >> 7;

	// now, send the command
	SPITransmit( command );
	SPITransmit( pwm & 0x7F );		// seven lowest bits of pwm byte
}


// Drive motor 1 at the pwm specified by speed.  The sign of speed determines
//  the direction and the magnitude of speed should be 0 - 255.  This command
//  affects the motor immediately.
void setMotor1( int speed )
{
	unsigned char command;
	
	if ( speed > 255 )
		speed = 255;
	if ( speed < -255 )
		speed = -255;

	if ( speed >= 0 )
		command = CMD_MOTOR1_FORWARD;		// motor 1, forward
	else
	{
		command = CMD_MOTOR1_REVERSE;		// motor 1, reverse
		speed = -speed;
	}

	sendMotorPWMCommand(command, (unsigned char) speed);
}


// Accelerate motor 1 from its current speed and direction to the specified
//  speed and direction.  Acceleration is only applied when the pwm is
//  increasing, which always happens when this call results in a direction
//  change.  This command updates motor state every 10ms.
void accelMotor1( int speed )
{
	unsigned char command;
	
	if ( speed > 255 )
		speed = 255;
	if ( speed < -255 )
		speed = -255;

	if ( speed >= 0 )
		command = CMD_MOTOR1_ACCEL_FORWARD;		// motor 1, accel forward
	else
	{
		command = CMD_MOTOR1_ACCEL_REVERSE;		// motor 1, accel reverse
		speed = -speed;
	}

	sendMotorPWMCommand(command, (unsigned char) speed);
}


// Variable braking for motor 1.   This command affects the motor immediately.
void brakeLowMotor1(unsigned char pwm)
{
	sendMotorPWMCommand(CMD_MOTOR1_BRAKE_LOW, pwm);
}


// Variable braking for motor 1.   This command affects the motor immediately.
void brakeHighMotor1(unsigned char pwm)
{
	sendMotorPWMCommand(CMD_MOTOR1_BRAKE_HIGH, pwm);
}


// Drive motor 2 at the pwm specified by speed.  The sign of speed determines
//  the direction and the magnitude of speed should be 0 - 255.  This command
//  affects the motor immediately.
void setMotor2( int speed )
{
	unsigned char command;
	
	if ( speed > 255 )
		speed = 255;
	if ( speed < -255 )
		speed = -255;

	if ( speed >= 0 )
		command = CMD_MOTOR2_FORWARD;		// motor 2, forward
	else
	{
		command = CMD_MOTOR2_REVERSE;		// motor 2, reverse
		speed = -speed;
	}

	sendMotorPWMCommand(command, (unsigned char) speed);
}


// Accelerate motor 2 from its current speed and direction to the specified
//  speed and direction.  Acceleration is only applied when the pwm is
//  increasing, which always happens when this call results in a direction
//  change.  This command updates motor state every 10ms.
void accelMotor2( int speed )
{
	unsigned char command;
	
	if ( speed > 255 )
		speed = 255;
	if ( speed < -255 )
		speed = -255;

	if ( speed >= 0 )
		command = CMD_MOTOR2_ACCEL_FORWARD;		// motor 2, accel forward
	else
	{
		command = CMD_MOTOR2_ACCEL_REVERSE;		// motor 2, accel reverse
		speed = -speed;
	}

	sendMotorPWMCommand(command, (unsigned char) speed);
}


// Variable braking for motor 2.   This command affects the motor immediately.
void brakeLowMotor2(unsigned char pwm)
{
	sendMotorPWMCommand(CMD_MOTOR2_BRAKE_LOW, pwm);
}


// Variable braking for motor 2.   This command affects the motor immediately.
void brakeHighMotor2(unsigned char pwm)
{
	sendMotorPWMCommand(CMD_MOTOR2_BRAKE_HIGH, pwm);
}


// Drive jointly controlled motor at the pwm specified by speed. The sign of
//  speed determines the direction and the magnitude of speed should be 0-255.
//  This command affects the motor immediately.
void setJointMotor( int speed )
{
	unsigned char command;
	
	if ( speed > 255 )
		speed = 255;
	if ( speed < -255 )
		speed = -255;

	if ( speed >= 0 )
		command = CMD_JOINT_FORWARD;		// joint motor, forward
	else
	{
		command = CMD_JOINT_REVERSE;		// joint motor, reverse
		speed = -speed;
	}

	sendMotorPWMCommand(command, (unsigned char) speed);
}


// Accelerate joint motor from its current speed and direction to the specified
//  speed and direction.  Acceleration is only applied when the pwm is
//  increasing, which always happens when this call results in a direction
//  change.  This command updates motor state every 10ms.
void accelJointMotor( int speed )
{
	unsigned char command;
	
	if ( speed > 255 )
		speed = 255;
	if ( speed < -255 )
		speed = -255;

	if ( speed >= 0 )
		command = CMD_JOINT_ACCEL_FORWARD;		// joint motor, accel forward
	else
	{
		command = CMD_JOINT_ACCEL_REVERSE;		// joint motor, accel reverse
		speed = -speed;
	}

	sendMotorPWMCommand(command, (unsigned char) speed);
}



// Variable braking for jointly controlled motor.  This command affects the
//  motor immediately.
void brakeJointMotor( unsigned char pwm )
{
	sendMotorPWMCommand(CMD_JOINT_BRAKE, pwm);
}


// Enables the mega168 to respond to joint-motor commands while disabling
//  independent-motor commands.
void enableJointMotorMode()
{
	SPITransmit( CMD_ENABLE_JOINT_MOTOR_MODE );
}


// Sets the amount of time the motor will brake low at 100% duty cycle when
//  it receives an acceleration command that will result in a change of
//  direction.  The units of brakeDuration are 10ms, which means an argument
//  value of 1 will result in a brake duration of 10ms and an argument value
//  of 127 will result in a brake duration of 1.27s.  If brakeDuration is zero,
//  there is no braking before an acceleration direction change.  brakeDuration
//  must be 127 or less.  These functions will only work with X2 firmware
//  versions 1.01 or later.
void setM1BrakeDuration( unsigned char brakeDuration )
{
	if ( brakeDuration > 127 )
		brakeDuration = 127;
	SPITransmit( CMD_SET_M1_BRAKE_DURATION );
	SPITransmit( brakeDuration );
}

void setM2BrakeDuration( unsigned char brakeDuration )
{
	if ( brakeDuration > 127 )
		brakeDuration = 127;
	SPITransmit( CMD_SET_M2_BRAKE_DURATION );
	SPITransmit( brakeDuration );
}


// Sets the accelerations used by the accelMotor functions. Acceleration 
//  commands cause the motor's pwm to be incremented by accel/10 every
//  10ms.  The net effect is that the pwm increases by accel every 100ms.
//  An accel of zero = "infinite" acceleration.  Accel must be 127 or less.
void setM1Acceleration( unsigned char accel )
{
	if ( accel > 127 )
		accel= 127;
	SPITransmit( CMD_SET_M1_ACCELERATION );
	SPITransmit( accel );
}

void setM2Acceleration( unsigned char accel )
{
	if ( accel > 127 )
		accel = 127;
	SPITransmit( CMD_SET_M2_ACCELERATION );
	SPITransmit( accel );
}


// PWM frequency is determined by the resolution (either 7-bit or 8-bit) and
//  prescaler used to convert the 20MHz IO clk into the PWM clock.  The formula
//  is frequency = 20MHz / prescaler / 2^bit-resolution
//  Arguments passed to this function should be the RESOLUTION_ and PRESCALER_
//  #defines in SPI.h.
void setPWMFrequencies( unsigned char M1Resolution, unsigned char M1Prescaler,
						unsigned char M2Resolution, unsigned char M2Prescaler)
{
	if ( M1Resolution != RESOLUTION_7BIT )
		M1Resolution = RESOLUTION_8BIT;
	if ( M2Resolution != RESOLUTION_7BIT )
		M2Resolution = RESOLUTION_8BIT;
	if ( M1Prescaler > PRESCALER_1024 )
		M1Prescaler = PRESCALER_1024;
	if ( M2Prescaler > PRESCALER_1024 )
		M2Prescaler = PRESCALER_1024;

	SPITransmit( CMD_SET_PWM_FREQUENCIES );
	SPITransmit( ( M2Resolution << 5 ) | ( M2Prescaler << 3 ) |
				 ( M1Resolution << 2 ) |   M1Prescaler );
}


// The number of current samples in the running current averages is given by:
//  M1 average current averages the last 2 ^ M1Exponent samples
//  M2 average current averages the last 2 ^ M2Exponent samples
//  The exponents may range from 0 - 7 (which equates to 1, 2, 4, 8, 16, 32,
//  64, or 128 samples).  More samples means less noise, but it also means
//  older data is affecting your present measurement.
void setNumCurrentSamples( unsigned char M1Exponent, unsigned char M2Exponent)
{
	if ( M1Exponent > 7 )
		M1Exponent = 7;
	if ( M2Exponent > 7 )
		M2Exponent = 7;
	SPITransmit( CMD_SET_NUM_CURRENT_SAMPLES );
	SPITransmit( ( M2Exponent << 3 ) | M1Exponent );
}


// If current limit is zero, there is no limit.  Otherwise, if the current
// average ever exceeds the limit, P determines what happens.  If P is zero,
// the motor shuts off.  If P is non-zero, the motor's PWM is decreased by
// P * (current - limit).  The PWM will never increase by an amount greater
// than P * | limit - current | while accelerating, either.  There are
// no restrictions on limit; P must be no greater than 127.  These functions
// will only work with X2 firmware version 1.01 or later.
void setM1CurrentLimit( unsigned char limit, unsigned char P )
{
	if (P > 127)
		P = 127;
	SPITransmit( CMD_SET_M1_CURRENT_LIMIT | ( limit & 0x80 ));
	SPITransmit( limit & 0x7F );
	SPITransmit( P );
}

void setM2CurrentLimit( unsigned char limit, unsigned char P )
{
	if (P > 127)
		P = 127;
	SPITransmit( CMD_SET_M2_CURRENT_LIMIT | ( limit & 0x80 ));
	SPITransmit( limit & 0x7F );
	SPITransmit( P );
}


// Get the running current average for the specified motor
unsigned char getM1Current()
{
	SPITransmit( CMD_GET_M1_CURRENT );
	return SPIReceive( 0 );
}

unsigned char getM2Current()
{
	SPITransmit( CMD_GET_M2_CURRENT );
	return SPIReceive( 0 );
}




// ***********************************************************************
// 							Buzzer Commands
// ***********************************************************************


// Silence the buzzer immediately
void buzzerOff()
{
	SPITransmit( CMD_BUZZER_OFF );
}


// Play the specified note for the specified duration (in ms) immediately.
//  Note enumeration is provided as a set of #define macros in SPI.h.
void playNote( unsigned char note, unsigned int duration )
{
	unsigned char *byte_ptr = (unsigned char*)&duration;
	unsigned char lo = *byte_ptr;		// low byte of duration
	unsigned char hi = *(byte_ptr + 1);	// high byte of duration

	// insert the MSBs of the three data bytes into the command byte
	SPITransmit( CMD_PLAY_NOTE | (( lo & 0x80 ) >> 5 ) | (( hi & 0x80 ) >> 6 )
							   | (( note & 0x80 ) >> 7 ));
	SPITransmit( note & 0x7F );
	SPITransmit(   hi & 0x7F );
	SPITransmit(   lo & 0x7F );
}


// Play the specified frequency (in Hz) for the specified duration (in ms)
//  immediately.  Frequency must be no greater than 0x7FFF (15-bit value).
//  The mega168 can only play frequencies as low as 40Hz and as high as
//  10kHz.  Values outside this range will just produce the nearest
//  allowed frequency.
void playFrequency( unsigned int frequency, unsigned int duration )
{
	if ( frequency > 0x7FFF )
		frequency = 0x7FFF;		// frequency must be a 15-bit value

	unsigned char *byte_ptr = (unsigned char*)&frequency;
	unsigned char loFreq = *byte_ptr;
	unsigned char hiFreq = *(byte_ptr + 1);

	byte_ptr = (unsigned char*)&duration;
	unsigned char loDur = *byte_ptr;
	unsigned char hiDur = *(byte_ptr + 1);

	// insert the MSBs of the two duration bytes and the low frequency byte
	//  MSB of the high frequency byte is guaranteed to be zero
	SPITransmit( CMD_PLAY_FREQUENCY | ((  loDur & 0x80) >> 5 )
									| ((  hiDur & 0x80) >> 6 )
									| (( loFreq & 0x80) >> 7 ));
	SPITransmit( hiFreq );
	SPITransmit( loFreq & 0x7F );
	SPITransmit(  hiDur & 0x7F );
	SPITransmit(  loDur & 0x7F );
}


// Play the specified melody, which is a sequence of notes stored in the
//  mega168's EEPROM.  You can have up to 8 stored melodies (melody ranges
//  from 0 - 7).  If the melody doesn't exist nothing will happen.
void playMelody( unsigned char melody )
{
	if ( melody < 8 )
	{
		waitWhileEEPROMBusy();	// wait for any current EEPROM writes to finish
		SPITransmit( CMD_PLAY_MELODY | melody );
	}
}


// Add this note to the melody currently under construction. There is room
//  in the mega168's EEPROM for 159 total notes.
//  Note enumeration is provided as a set of #define macros in SPI.h.
void storeNote( unsigned char note, unsigned int duration )
{
	unsigned char *byte_ptr = (unsigned char*)&duration;
	unsigned char lo = *byte_ptr;		// low byte of duration
	unsigned char hi = *(byte_ptr + 1);	// high byte of duration

	waitWhileEEPROMBusy();		// wait for any current EEPROM writes to finish
		
	// insert the MSBs of the three data bytes into the command byte
	SPITransmit( CMD_STORE_NOTE | (( lo & 0x80 ) >> 5 ) | (( hi & 0x80 ) >> 6 )
								| (( note & 0x80 ) >> 7 ));
	SPITransmit( note & 0x7F );
	SPITransmit(   hi & 0x7F );
	SPITransmit(   lo & 0x7F );
}


// Terminate the melody currently under construction by pointing the start
//  of the next melody to the next EEPROM note block.  Any subsequent
//  storeNote() calls will build upon this next melody.  After a hardware
//  reset the current melody under construction is melody 0.
void endMelody()
{
	waitWhileEEPROMBusy();		// wait for any current EEPROM writes to finish
	SPITransmit( CMD_END_MELODY );
}


// Erase all the stored melodies by effectively resetting the melody-start
//  pointers to NULL.
void eraseMelodies()
{
	waitWhileEEPROMBusy();		// wait for any current EEPROM writes to finish
	SPITransmit( CMD_ERASE_MELODIES );
}


// Sets the volume of the buzzer by changing the duty cycle of the pwm driving
//  it.  buzzer duty cycle = 1 >> ( 16 - volume ).  volume must range from
//  0 - 15.
void setVolume( unsigned char volume )
{
	if ( volume > 15 )
		volume = 15;		// must be a 4-bit value
	SPITransmit( CMD_SET_VOLUME );
	SPITransmit( volume );
}


// noteGap determines the silent pause (in ms) that's inserted after every note
//  the buzzer plays.  noteGap must be no greater than 127.
void setNoteGap( unsigned char noteGap )
{
	if ( noteGap > 127 )
		noteGap = 127;
	SPITransmit( CMD_SET_NOTE_GAP );
	SPITransmit( noteGap );
}




// ***********************************************************************
// 							UART Commands
// ***********************************************************************

// Send a byte to the mega168 to be queued for transmission over the UART.
//  While queued the byte will reside in the mega168's UART send buffer.
void sendSerial( unsigned char data )
{
	SPITransmit( CMD_SEND_SERIAL | (( data & 0x80 ) >> 7 ));
	SPITransmit( data & 0x7F );
}


// Retrieve the next byte from the mega168's UART read buffer.
unsigned char readSerial()
{
	SPITransmit( CMD_READ_SERIAL );
	return SPIReceive( 0 );		// junk data byte
}


// This function chains together serial-read commands to rapidly read the
//  desired number of bytes from the mega168's UART read buffer.  If numBytes
//  is large it can reach approximately 7us per data byte read, which is
//  equivalent to a baud rate of approximately 1Mbps.  This function would
//  be good to use in conjunction with setReadReadySize(numBytes) if you're
//  dealing with serial data packets of known sizes.
void fastSerialRead( unsigned char numBytes, unsigned char data[] )
{
	unsigned char i;

	SPITransmit( CMD_READ_SERIAL );				// transmit first read command
	waitForTransmission();

	for ( i = 0; i < numBytes - 1; i++ )
	{
		delay_us( 3 );				// give mega168 time to prepare return val
		SPDR = CMD_READ_SERIAL;		// start bidirectional data transfer:
		waitForTransmission();		//  send command while reading return value
		data[ i ] = SPDR;						// buffer returned byte
	}

	SPITransmitting = 0;					// flag we are not transmitting
	data[ numBytes - 1 ] = SPIReceive( 0 );	// send junk to get last data byte
}


// This command will reserve the UART solely for programming the mega644.  In
//  permanent program mode you do not have to explicitly put the X2 into
//  programming mode to program the mega644.  This command will override all
//  current UART settings.  This setting can be made to persist after a 
//  hardware reset by saving the value 0x40 to mega168 EEPROM address 13.
void enablePermanentProgMode()
{
	SPITransmit( CMD_SET_SERIAL );
	SPITransmit( 0x40 );
	SPITransmit( 0 );
}


// Sets the UART parameters.  Arguments to this function should be the
//  UART #defines in SPI.h.  speedMode is either UART_NORMAL_SPEED or
//  UART_DOUBLE_SPEED.  This command will disable permanent progmode
//  if it's enabled.
void setSerial( unsigned char parity, unsigned char stopBits,
			   unsigned char speedMode, unsigned int UBRR )
{
	// ensure parameter values are all valid
	if (( parity != UART_EVEN_PARITY ) && ( parity != UART_ODD_PARITY ))
		parity = UART_NO_PARITY;
	if ( stopBits != UART_ONE_STOP_BIT )
		stopBits = UART_TWO_STOP_BITS;
	if ( speedMode != UART_NORMAL_SPEED )
		speedMode = UART_DOUBLE_SPEED;
	if ( UBRR > 2047 )					// UBRR must be an 11-bit value
		UBRR = 2047;
	SPITransmit( CMD_SET_SERIAL | ( parity << 1 ));
	SPITransmit( ( stopBits << 5 ) | ( speedMode << 4 ) |
				(( UBRR & 0x0780 ) >> 7 ));	// send the four MSBs of UBRR
	SPITransmit( UBRR & 0x007F );			// send the seven LSBs of UBBR
}


// Sets the UART parameters and saves them to the mega168's EEPROM.
//  Arguments to this function should be the UART #defines in SPI.h.  
//  speedMode is either UART_NORMAL_SPEED or UART_DOUBLE_SPEED.
//  This command will disable permanent progmode if it's enabled.
void setAndSaveSerial( unsigned char parity, unsigned char stopBits,
					   unsigned char speedMode, unsigned int UBRR )
{
	// ensure parameter values are all valid
	if (( parity != UART_EVEN_PARITY ) && ( parity != UART_ODD_PARITY ))
		parity = UART_NO_PARITY;
	if ( stopBits != UART_ONE_STOP_BIT )
		stopBits = UART_TWO_STOP_BITS;
	if ( speedMode != UART_NORMAL_SPEED )
		speedMode = UART_DOUBLE_SPEED;
	if ( UBRR > 2047 )					// UBRR must be an 11-bit value
		UBRR = 2047;

	waitWhileEEPROMBusy();		// ensure mega168's EEPROM is ready for writing
	SPITransmit( CMD_SET_AND_SAVE_SERIAL | ( parity << 1 ));
	SPITransmit( ( stopBits << 5 ) | ( speedMode << 4 ) |
				(( UBRR & 0x0780 ) >> 7 ));	// send the four MSBs of UBRR
	SPITransmit( UBRR & 0x007F );			// send the seven LSBs of UBBR

	delay_us(50);		// give the mega168 time to set parameters
}


// Determines how many bytes must be in the UART read buffer before the status
//  byte and attention line indicate that it is ready to be read.
void setReadReadySize( unsigned char rrSize )
{
	// rrSize must be in the range of 1 - UART_READ_BUFF_SZ (32)
	if ( rrSize == 0 )
		rrSize = 1;
	if ( rrSize > UART_READ_BUFF_SZ )
		rrSize = UART_READ_BUFF_SZ;
	SPITransmit( CMD_SET_READ_READY_SIZE );
	SPITransmit( rrSize - 1 );	// transmit one less than desired value
}


// Get number of unoccupied bytes in the UART send buffer
unsigned char getSendBuffFreeSpace()
{
	SPITransmit( CMD_GET_SEND_BUFF_FREE_SPACE );
	return SPIReceive( 0 );			// junk data byte
}


// Get number of occupied bytes in the UART read buffer
unsigned char getReadBuffNumBytes()
{
	SPITransmit( CMD_GET_READ_BUFF_NUM_BYTES );
	return SPIReceive( 0 );			// junk data byte
}


// Returns the UART error byte.  Error byte bits are enumerated in SPI.h
unsigned char getSerialError()
{
	SPITransmit( CMD_GET_UART_ERROR );
	return SPIReceive( 0 );
}



// ***********************************************************************
// 							EEPROM Commands
// ***********************************************************************

// Write a byte to the specified mega168's EEPROM address.  This command
//  can be used to store settings so that they persist after a hardware reset.
//  Settings occupy EEPROM bytes 0 - 23 and melody pointers occupy bytes
//  24 - 32.  Melody notes are saved to EEPROM in three-byte blocks starting
//  at address 33.
void writeEEPROM( unsigned int address, unsigned char data )
{
	if ( address >= 512 )		// address out of bounds
		return;

	waitWhileEEPROMBusy();		// wait for any current EEPROM writes to finish

	// insert data MSB and address bits 7 and 8 into the command byte
	SPITransmit( CMD_WRITE_EEPROM | (( data & 0x80 ) >> 5 )
								  | (( address & 0x0080 ) >> 6 )
								  | (( address & 0x0100 ) >> 8 ));
	SPITransmit( (unsigned char) ( address & 0x007F ));
	SPITransmit( data & 0x7F );
}


// Read a byte from the mega168's EEPROM.  This command can be used to check
//  the values of the settings that are loaded when the 168 is reset.
//  Settings occupy EEPROM bytes 0 - 23.
unsigned char readEEPROM( unsigned int address )
{
	if ( address >= 512 )		// address out of bounds
		return 0;

	waitWhileEEPROMBusy();		// wait for any current EEPROM writes to finish
	
	// insert address bits 7 and 8 into the command byte
	SPITransmit( CMD_READ_EEPROM | (( address & 0x0080 ) >> 6 )
								 | (( address & 0x0100 ) >> 8 ));
	SPITransmit( (unsigned char) ( address & 0x007F ));
	return SPIReceive( 0 );		// send a junk data byte here
}


// Check to see if the mega168's EEPROM is currently being written to, which
//  means it is not possible to read from it or start a new write.
unsigned char getEEPROMBusy()
{
	SPITransmit( CMD_GET_EEPROM_BUSY );
	return SPIReceive( 0 );		// send a junk data byte here
}




// ***********************************************************************
// 							Misc Commands
// ***********************************************************************

// Get the mega168's status byte.  Status byte bits are enumerated in SPI.h.
//  Checking the status will clear the attention line and the latched
//  status bits.
unsigned char getStatus()
{
	SPITransmit( CMD_GET_STATUS );
	return SPIReceive( 0 );		// junk data byte
}


// Get the version of the firmware running on the mega168.  The arguments
//  to this function will hold the return values once the function is through.
void getFirmwareVersion( unsigned char *vMajor, unsigned char *vMinor )
{
	SPITransmit( CMD_GET_FIRMWARE_VERSION );
	*vMajor = SPIReceive( 0 );	// can send any data byte here (not NULL cmd)
	*vMinor = SPIReceive( 0 );	// junk data byte (this one can be NULL cmd)
}



// ***********************************************************************
// 							Save Settings Commands
// ***********************************************************************

// These functions save settings to the mega168's EEPROM.  Saved settings
//  are loaded every time the mega168 initializes itself (e.g. after every
//  hardware reset).  Calling these functions will have no immediate effect
//  on the settings, only on the mega168's EEPROM; they will affect the
//  settings after the mega168 is next reset.


// After this function is called the mega168 must be manually reset for the
//  changes to take effect.  After the reset the settings will all be reset
//  to their default values.
void restoreDefaultSettings()
{
	writeEEPROM( ADDR_INIT_CHECK, 0xFF );
}


// PWM frequency is determined by the resolution (either 7-bit or 8-bit) and
//  prescaler used to convert the 20MHz IO clk into the PWM clock.  The formula
//  is frequency = 20MHz / prescaler / 2^bit-resolution
//  Arguments passed to this function should be the RESOLUTION_ and PRESCALER_
//  #defines in SPI.h.
void saveM1PWMFrequency( unsigned char resolution, unsigned char prescaler )
{
	if ( resolution != RESOLUTION_7BIT )
		resolution = RESOLUTION_8BIT;
	if ( prescaler > PRESCALER_1024 )
		prescaler = PRESCALER_1024;
	writeEEPROM( ADDR_M1_PWM_FREQUENCY, ( resolution << 2 ) | prescaler );
}

void saveM2PWMFrequency( unsigned char resolution, unsigned char prescaler )
{
	if ( resolution != RESOLUTION_7BIT )
		resolution = RESOLUTION_8BIT;
	if ( prescaler > PRESCALER_1024 )
		prescaler = PRESCALER_1024;
	writeEEPROM( ADDR_M2_PWM_FREQUENCY, ( resolution << 2 ) | prescaler );
}


// The number of current samples in the running motor current average.  The
//  saved value must be a power of two (and hence cannot be zero).
void saveM1CurrentSamples( unsigned char samples )
{
	unsigned char i;
	unsigned char bitmask = 0x80;

	// ensure samples is a power of two
	if ( samples == 0 )
		samples = 1;
	for ( i = 0; i < 8; i++ )
	{
		if ( samples & bitmask )
			break;
		bitmask >>= 1;
	}
	samples &= bitmask;
	writeEEPROM( ADDR_M1_CURRENT_SAMPLES, samples );
}

void saveM2CurrentSamples( unsigned char samples )
{
	unsigned char i;
	unsigned char bitmask = 0x80;

	// ensure samples is a power of two
	if ( samples == 0 )
		samples = 1;
	for ( i = 0; i < 8; i++ )
	{
		if ( samples & bitmask )
			break;
		bitmask >>= 1;
	}
	samples &= bitmask;
	writeEEPROM( ADDR_M2_CURRENT_SAMPLES, samples );
}


// If current limit is zero, there is no limit.  Otherwise, if the current
// average ever exceeds the limit, P determines what happens.  If P is zero,
// the motor shuts off.  If P is non-zero, the motor's PWM is decreased by
// P * (current - limit).  The PWM will never increase by an amount greater
// than P * | limit - current | while accelerating, either.  There are
// no restrictions on limit; P must be no greater than 127.
void saveM1CurrentLimit( unsigned char limit, unsigned char P )
{
	if ( P > 127 )
		P = 127;
	writeEEPROM( ADDR_M1_CURRENT_LIMIT, limit );
	writeEEPROM( ADDR_M1_CL_P_CONST, P );
}

void saveM2CurrentLimit( unsigned char limit, unsigned char P )
{
	if ( P > 127 )
		P = 127;
	writeEEPROM( ADDR_M2_CURRENT_LIMIT, limit );
	writeEEPROM( ADDR_M2_CL_P_CONST, P );
}


// Saves the accelerations used by the accelMotor functions. Acceleration
//  commands cause the motor's pwm to be incremented by accel/10 every
//  10ms.  The net effect is that the pwm increases by accel every 100ms.
//  An accel of zero produces "infinite" acceleration.
void saveM1Acceleration( unsigned char accel )
{
	writeEEPROM( ADDR_M1_ACCELERATION, accel );
}

void saveM2Acceleration( unsigned char accel)
{
	writeEEPROM( ADDR_M2_ACCELERATION, accel);
}


// Saves the duration the motor spends braking low at 100% duty cycle
//  when an accelMotor command is received that will causes the motor to
//  change direction.  brakeDuration is in units of 10ms, so a value of
//  1 will brake for 10ms and a value of 127 will brake for 1.27s.
void saveM1BrakeDuration( unsigned char brakeDuration )
{
	writeEEPROM( ADDR_M1_BRAKE_DURATION, brakeDuration );
}

void saveM2BrakeDuration( unsigned char brakeDuration )
{
	writeEEPROM( ADDR_M2_BRAKE_DURATION, brakeDuration );
}


// Saves the UART parameters.  Arguments to this function should be the
//  UART #defines in SPI.h.  speedMode is either UART_NORMAL_SPEED or
//  UART_DOUBLE_SPEED.  This command will disable a saved permanent progmode
//  if it's enabled.
void saveSerialSettings( unsigned char parity, unsigned char stopBits,
						 unsigned char speedMode, unsigned int UBRR )
{
	// ensure parameter values are all valid
	if (( parity != UART_EVEN_PARITY ) && ( parity != UART_ODD_PARITY ))
		parity = UART_NO_PARITY;
	if ( stopBits != UART_ONE_STOP_BIT )
		stopBits = UART_TWO_STOP_BITS;
	if ( speedMode != UART_NORMAL_SPEED )
		speedMode = UART_DOUBLE_SPEED;
	if ( UBRR > 2047 )					// UBRR must be an 11-bit value
		UBRR = 2047;

	writeEEPROM( ADDR_SERIAL_SETTINGS, ( parity << 4 ) | ( stopBits << 3 ) |
									   ( speedMode << 1 ));
	writeEEPROM( ADDR_SERIAL_UBRRH, (( UBRR & 0x0700 ) >> 8 ));
	writeEEPROM( ADDR_SERIAL_UBRRL, UBRR & 0x00FF );
}


// Reserves the UART solely for programming the mega644 so that you do not need
//  to explicitly put the X2 into programming mode to program the mega644.
void saveEnablePermanentProgMode()
{
	writeEEPROM( ADDR_SERIAL_SETTINGS, 0x40 );
}


// Determines how many bytes must be in the UART read buffer before the status
//  byte and attention line indicate that it is ready to be read.
void saveReadReadySize( unsigned char readReadySize )
{
	// readReadySize must be in the range of 1 - UART_READ_BUFF_SZ (32)
	if ( readReadySize == 0 )
		readReadySize = 1;
	if ( readReadySize > UART_READ_BUFF_SZ )
		readReadySize = UART_READ_BUFF_SZ;

	writeEEPROM( ADDR_SERIAL_READ_READY, readReadySize );
}


// Saves the volume of the buzzer by changing the duty cycle of the pwm driving
//  it.  buzzer duty cycle = 1 >> ( 16 - volume ).  volume must range from
//  0 - 15.
void saveVolume( unsigned char volume )
{
	if ( volume > 15 )
		volume = 15;
	writeEEPROM( ADDR_BUZZER_VOLUME, volume );
}


// noteGap determines the silent pause (in ms) that's inserted after every note
//  the buzzer plays.
void saveNoteGap( unsigned char noteGap )
{
	writeEEPROM( ADDR_NOTE_GAP, noteGap );
}


// Determine the sound that plays when the mega168 initializes itself after a
//  shutdown or reset.  if 0 <= melody <= 7, it will play that melody on
//  startup.  If melody == 8, it will make no sound on startup.  All other
//  melody values will cause it to chirp on startup.
void saveStartupMelody( unsigned char melody )
{
	writeEEPROM( ADDR_STARTUP_MELODY, melody );
}


// At the time of the X2's release, the current AVR Studio AVRISP version is
//  2.0A. The arguments to this function represent the version AVRISP version
//  number the mega168 will transmit to AVR Studio when you attempt to program
//  the mega644. If the two version don't match, you will have to first cancel
//  out of an "upgrade your firmware?" dialog box every time you try to program
//  your mega644 (you cannot upgrade your firmware, anyway, so this dialog is
//  nothing more than an annoyance). As such, you might have a better
//  programming experience if you set these version values to match the AVRISP
//  version used by the AVR Studio you're running (if you use AVR Studio).
void saveAVRISPVersion( unsigned char vMajor, unsigned char vMinor )
{
	// version is "vMajor.vMinor"
	writeEEPROM( ADDR_ISP_SW_MAJOR, vMajor );
	writeEEPROM( ADDR_ISP_SW_MINOR, vMinor );
}
