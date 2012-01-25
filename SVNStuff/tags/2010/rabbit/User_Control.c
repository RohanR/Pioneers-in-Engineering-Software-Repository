/*** BeginHeader setTREX0 setTREX1 setServo*/
void setTREX0(unsigned char spd, unsigned char spd_prev );
void setTREX1(unsigned char spd, unsigned char spd_prev );
void setServo(char servo, int micros);
/*** EndHeader */


#define  C_BAUDRATE	57600
#define  D_BAUDRATE	57600
#define 	E_BAUDRATE  57600

#define CINBUFSIZE 255
#define COUTBUFSIZE 255
#define DINBUFSIZE 255
#define DOUTBUFSIZE 255


void serialInit() {
//These are required for serial initialization

	//Serial C is used for communication with X2
	serCopen(C_BAUDRATE);
   serCwrFlush();
   serCrdFlush();

   //Serial D is used for TREXs
	serDopen(D_BAUDRATE);
   serDwrFlush();
   serDrdFlush();

   //Serial E is used for Servo control
   serEopen(E_BAUDRATE);
   serEwrFlush();
   serErdFlush();

   // byte for baudrate auto-detection
   serEputc(0xAA);
}


void updateTREX() {

   unsigned char spd;
   unsigned char spd_prev;
   unsigned char i,j;

	if(serDwrUsed() == 0){
   /*
		motors.trexCycle++;
      if(motors.trexCycle>1) motors.trexCycle = 0;
		switch(motors.trexCycle){
	    case 0 : setTREX0(motors.trex[0]);
	    case 1 : setTREX1(motors.trex[1]);
	   }
   */
		// TReX number (currently support 2)
      // TESTING: changed to 1 TReX
   	for (i = 0; i < 1; i += 1) {
	      // motor number (0,1 = main motors, 2 = unidirectional motor)
			spd = motors.trex[i][0];
		   spd_prev = motors.trex_prev[i][0];
		   setTREX0(spd,spd_prev);
		   motors.trex_prev[i][0] = motors.trex[i][0];

			spd = motors.trex[i][1];
	      spd_prev = motors.trex_prev[i][1];
	      setTREX1(spd,spd_prev);
	      motors.trex_prev[i][1] = motors.trex[i][1];
      }
		/*
   	//if(motors.trex[0] != motors.trex_prev[0]){
	      spd = motors.trex[0];
	      spd_prev = motors.trex_prev[0];
	      setTREX0(spd,spd_prev);
	      motors.trex_prev[0] = motors.trex[0];
	   //}

	   //if(motors.trex[1] != motors.trex_prev[1]){
	      spd = motors.trex[1];
	      spd_prev = motors.trex_prev[1];
	      setTREX1(spd,spd_prev);
	      motors.trex_prev[1] = motors.trex[1];
	   //}
		*/
   }

}

// This uses the Pololu protocol:
//  0x80 [device #] [command byte lower 7bits] [data bytes ...]
// The default TReX device # is 7; we number further TReXes
//  beginning at 8.
void sendTREXCommand(unsigned char deviceNo, unsigned char cmd,
							unsigned char data) {
   /*
	serDputc(0x80);			// Pololu protocol start command
   serDputc(deviceNo & 0x7F);
   serDputc(cmd & 0x7F);	// lower 7 bits of command byte
	serDputc(data & 0x7F);
   */
   serDputc(cmd);
   serDputc(data & 0x7F);
}

//send a packet indicating direction
//then a packet indicating speed
void setTREX0(unsigned char spd, unsigned char spd_prev){
	if(spd < 127){
   	//sendTREXCommand(7, 0xC1, 127-spd);
		serDputc(0xC1);
		serDputc((127-spd));
	}
	else if(spd >127){
   	//sendTREXCommand(7, 0xC2, 127-(255-spd));
		serDputc(0xC2);
		serDputc(127 - (255 - spd));
   }
   else{
	   // Brake
		if(spd_prev < 127){
      	//sendTREXCommand(7, 0xC1, 0);
			serDputc(0xC1);
      } else{
      	//sendTREXCommand(7, 0xC2, 0);
			serDputc(0xC2);
		}
      serDputc(0);
   }
   return;
}

void setTREX1(unsigned char spd, unsigned char spd_prev){
	if(spd < 127){
   	//sendTREXCommand(7, 0xC9, 127-spd);
		serDputc(0xC9);
      serDputc((127-spd));
   }
   else if(spd > 127){
   	//sendTREXCommand(7, 0xCA, 127-(255-spd));
		serDputc(0xCA);
      serDputc(127 - (255 - spd));
   }
   else{
		// Brake
	   if(spd_prev < 127){
      	//sendTREXCommand(7, 0xC9, 0);
	      serDputc(0xC9);
	   } else{
      	//sendTREXCommand(7, 0xCA, 0);
	      serDputc(0xCA);
	   }
      serDputc(0);
   }
	return;
}

void setTREX1_0(unsigned char spd, unsigned char spd_prev) {
	if(spd < 127){
   	sendTREXCommand(8, 0xC1, 127-spd);
	} else if(spd >127){
   	sendTREXCommand(8, 0xC2, 127-(255-spd));
   } else{
	   // Brake
		if(spd_prev < 127){
      	sendTREXCommand(8, 0xC1, 0);
      } else{
      	sendTREXCommand(8, 0xC2, 0);
		}
   }
   return;
}

void setTREX1_1(unsigned char spd, unsigned char spd_prev) {
	if(spd < 127){
   	sendTREXCommand(8, 0xC9, 127-spd);
   } else if(spd > 127){
   	sendTREXCommand(8, 0xCA, 127-(255-spd));
   } else{
		// Brake
	   if(spd_prev < 127){
      	sendTREXCommand(8, 0xC9, 0);
	   } else{
      	sendTREXCommand(8, 0xCA, 0);
	   }
   }
	return;
}


void updateServos() {
  int i;

  for (i=0; i < 6; i++) {
  		//if (servos.positions[i] != servos.positions_prev[i]) {
      	setServo((char)i, servos.positions[i]*7 + 600);
         servos.positions_prev[i] = servos.positions[i];
		//}
	}

}


void setServo(char servo, int micros) {
	micros *= 4;	// send quarter-microseconds
   //printf("servo: %d   micros: %d \n", servo, micros);

	serEputc(0x84);
   serEputc(servo);
	serEputc(micros & 0x7F);	// least significant 7 bits
   serEputc((micros >> 7) & 0x7F);
}