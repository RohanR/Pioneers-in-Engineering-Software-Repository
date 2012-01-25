#ifndef _devices_h
#define _devices_h

#include <stdint.h>

typedef struct {
	uint8_t motor[3];
} TRexValues;

typedef struct {
	uint8_t motor[2];
	uint8_t servo[2];
} X2Values;

typedef struct {
	uint8_t servo[6];
} ServoValues;


/* Bitfields of an unsigned char */
typedef struct {
  unsigned int bit0:1;
  unsigned int bit1:1;
  unsigned int bit2:1;
  unsigned int bit3:1;
  unsigned int bit4:1;
  unsigned int bit5:1;
  unsigned int bit6:1;
  unsigned int bit7:1;
} Bitfield;

/* Datafield that is the width of an unsigned char */
typedef union {
  Bitfield bits;
  uint8_t allbits;
} Bytefield;


typedef struct {
	uint8_t analog[12];
	Bytefield digital[8];
} ControlValues;

typedef struct {
	Bytefield digitalIn[2];
} RabbitValues;


extern TRexValues trex;
extern X2Values x2;
extern ServoValues sc;
extern RabbitValues rabbit;
extern ControlValues controls;

#endif // _devices_h
