using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

namespace PiEProject1
{
    public class MicroMaestro : ActuatorController
    {
        public byte servo_1_ID;
        public byte servo_2_ID;
        public byte servo_3_ID;
        public byte servo_4_ID;
        public byte servo_5_ID;
        public byte servo_6_ID;

        public SerialPort port;
        public Thread thread;
        public byte[] servoBuffer;
        public Robot robot;
        public bool moveOne;
        public bool disabled;

        public MicroMaestro (Robot robo, string portName)
        {
            this.disabled = false;
            robot = robo;
            moveOne = true;
            port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            port.ReadTimeout = 2;
            port.Open();

            servoBuffer = new byte[16];

            thread = new Thread(new ThreadStart(this.Poll));
            thread.Start();
        }

        public void Poll()
        {
            while (true)
            {
                // poll feedback from motor controller
                updateServos();
                Thread.Sleep(1000); // milliseconds
            }
        }

        //public void SetServo1()
        //{
        //    //Debug.Print("set servo");
        //    Random random = new Random();
        //    int spd = random.Next(12864) + 256; // random PWM from range 256-11320 [us/4]
        //    servoBuffer[0] = 0x84; // target pwm
        //    if (moveOne)
        //    {
        //        servoBuffer[1] = 0x02; // which servo location on the controller
        //        moveOne = !moveOne;
        //    }
        //    else
        //    {
        //        servoBuffer[1] = 0x01;
        //        moveOne = !moveOne;
        //    }
        //    servoBuffer[2] = (byte)(spd & 0x7F); // lower 7 bytes of the target pwm
        //    servoBuffer[3] = (byte)(spd >> 7 & 0x7F); // bytes 7-13 of the target pwm
        //    port.Write(servoBuffer, 0, 4);
        //    return;
        //}

        public void updateServos()
        {
            if (this.disabled == true) return;
            for (int servos = 0; servos < 8; servos++)
            {
                //Debug.Print("set servo");
                int spd = robot.getActuatorValue(getServoID(servos));
                servoBuffer[0] = 0x84; // target pwm
                if (moveOne)
                {
                    servoBuffer[1] = (byte)servos; // which servo location on the controller
                    moveOne = !moveOne;
                }
                else
                {
                    servoBuffer[1] = 0x01;
                    moveOne = !moveOne;
                }
                servoBuffer[2] = (byte)(spd & 0x7F); // lower 7 bytes of the target pwm
                servoBuffer[3] = (byte)(spd >> 7 & 0x7F); // bytes 7-13 of the target pwm
                port.Write(servoBuffer, 0, 4);
            }
            return;
        }

        public void setServoID(int servoNumber, byte servoID)
        {
            switch (servoNumber)
            {
                case 1:
                    servo_1_ID = servoID;
                    break;
                case 2:
                    servo_2_ID = servoID;
                    break;
                case 3:
                    servo_3_ID = servoID;
                    break;
                case 4:
                    servo_4_ID = servoID;
                    break;
                case 5:
                    servo_5_ID = servoID;
                    break;
                case 6:
                    servo_6_ID = servoID;
                    break;
            }
        }

        public byte getServoID(int servoNumber)
        {
            switch (servoNumber)
            {
                case 1:
                    return servo_1_ID;
                case 2:
                    return servo_2_ID;
                case 3:
                    return servo_3_ID;
                case 4:
                    return servo_4_ID;
                case 5:
                    return servo_5_ID;
                case 6:
                    return servo_6_ID;
                default:
                    return 0;
            }
        }

        public void kill()
        {
            this.disabled = true;
        }

        public void disable()
        {
            this.disabled = true;
        }

        public void enable()
        {
            this.disabled = false;
        }


     

    }
}
