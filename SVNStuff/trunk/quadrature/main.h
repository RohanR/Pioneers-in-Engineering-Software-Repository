#ifndef _main_h
#define _main_h

// Pololu protocol settings
// Device number is any int from 0-127
#define MY_DEVICE_NUM	10

// Structures
typedef struct {
	unsigned count_both_edges_enc2:1;
	unsigned count_both_edges_enc3:1;
	
} EncoderConfig;


// Enumeration for UART state machine
enum {
	STATE_IDLE,
	STATE_START,
	STATE_CMD
};


// Function prototypes
void encoder_init(void);
void cmd_received(unsigned char data);
void byte_received(unsigned char data);


// Patch for missing avr/io.h includes

// GIMSK - General Interrupt Mask Register
#define PCIE0 5
#define PCIE2 4
#define PCIE1 3


// Miscellaneous helper functions
#define NULL 0

#define FALSE 0
#define TRUE !(FALSE)

#define sbi(value, bit) ((value) |= (1UL << (bit)))
#define cbi(value, bit) ((value) &= ~(1UL << (bit)))
#define bitRead(value, bit) (((value) >> (bit)) & 0x01)

#endif //_main_h
