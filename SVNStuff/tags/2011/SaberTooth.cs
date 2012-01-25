using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;



enum commands {
    motor1_forward = 0,
    motor1_backward = 1,
    minVolt = 2,
    maxVolt = 3,
    motor2_forward = 4,
    motor2_backward = 5,
    motor1_drive = 6,
    motor2_drive = 7,
};


namespace PiEProject1
{
    class SaberTooth : ActuatorController
    {   
        // const int PACKET_LENGTH = 4;        
        public SerialPort port;
        public Thread thread;
        public Robot robot;
        public bool move;
        public byte motor1_ID;
        public byte motor2_ID;
        public bool disabled;
        public byte address = 128;

        public SaberTooth(Robot robo, string portName)
        {
            this.disabled = false;
            Thread.Sleep(2000);
            this.robot = robo;
            this.port = new SerialPort(portName, 9600, Parity.None, 64, StopBits.One);
            Debug.Print("StartPoll");
            thread = new Thread(new ThreadStart(this.Poll));
            thread.Start();
        }

        public void Poll() {
            while (true)
            {
                Debug.Print("Poll");
                Random random = new Random();
                // get input
                int speed = random.Next(127);
                SetMotor((byte)commands.motor1_forward, speed);
                SetMotor((byte)commands.motor2_forward, speed);
                Thread.Sleep(500);
            }
        }


        public void SetMotor(byte motorCommand, int speed)
        {
            Debug.Print("SetMotor" + motorCommand + " to speed: " + speed);
            byte[] packet = new byte[4];
            packet[0] = address;
            packet[1] = motorCommand;
            packet[2] = (byte)speed;
            packet[3] = (byte)((address + motorCommand + (byte)speed) & 0x7F);

            port.Write(packet, 0, 4);
        }

        public void updateMotor()
        {
            this.SetMotor((byte)commands.motor1_drive, robot.getActuatorValue(motor1_ID));
            this.SetMotor((byte)commands.motor2_drive, robot.getActuatorValue(motor2_ID));
        }

        public void disable()
        {
            this.disabled = true;
            this.SetMotor((byte)commands.motor1_drive, 64);
            this.SetMotor((byte)commands.motor2_drive, 64);
        }

        public void enable()
        {
            this.disabled = false;
        }

        public int[] getMotorValue()
        {
            int[] motorValues = new int[2];
            motorValues[0] = this.robot.getActuatorValue(motor1_ID);
            motorValues[1] = this.robot.getActuatorValue(motor2_ID);

            return motorValues;
        }

        public void kill()
        {
            this.disable();
            // disable pin
        }

        public void setMotorID(int motorNumber, byte motorID)
        {
            switch (motorNumber)
            {
                case 1:
                    motor1_ID = motorID;
                    break;
                case 2:
                    motor2_ID = motorID;
                    break;
            }
        }

    }
}