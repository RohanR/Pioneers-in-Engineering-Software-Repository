using System;
using System.IO.Ports;
using System.Threading;
using Microsoft.SPOT;

using System.Collections;

namespace PiEAPI
{
    public class Robot
    {
        string teamID { get; set; }
        private static string portName = "COM4";
        private ArrayList actuators = new ArrayList();
        private ArrayList sensors = new ArrayList();

        public SerialPort motorPort;
        public Radio radio;

        SimpleMotorController leftMotor, rightMotor;

        public Robot(string teamID)
        {
            this.teamID = teamID;
            radio = new Radio(this, portName);

            // Initialize single motor COM port
            motorPort = new SerialPort("COM1", 9600);
            motorPort.ReadTimeout = 2;
            motorPort.Open();
            // Send baudrate autodetect byte (0xAA)
            motorPort.Write(new byte[] { (byte)0xAA }, 0, 1);

            leftMotor = new SimpleMotorController(this, 13);
            rightMotor = new SimpleMotorController(this, 14);
        }

        public int[] UpdateUIAnalogVals(byte[] analogVals)
        {
            actuators.Clear();
            foreach (int aVal in analogVals)
            {
                actuators.Add(aVal);
            }
            return (int[])(actuators.ToArray(typeof(int)));
        }

        public bool[] UpdateUIDigitalVals(bool[] digitalVals)
        {
            sensors.Clear();
            foreach (bool dVal in digitalVals)
            {
                sensors.Add(dVal);
            }
            return (bool[])(sensors.ToArray(typeof(bool)));
        }

        public void AddActuator(ActuatorController actuator)
        {
            actuators.Add(actuator);
        }

        public void AddSensor(Sensor sensor)
        {
            sensors.Add(sensor);
        }

        public void KillDevices()
        {
            foreach (ActuatorController act in actuators)
            {
                //act.Kill();
            }
            foreach (Sensor sensor in sensors)
            {
                sensor.KillSensor();
            }
        }

        public void ReviveDevices()
        {
            foreach (ActuatorController act in actuators)
            {
                //act.Enable();
            }
            foreach (Sensor sensor in sensors)
            {
                //sensor.EnableSensor();
            }
        }

        public void InterfacePacketReceived(byte[] analog, bool[] digital)
        {
            // Tank drive
            leftMotor.motorSpeed = (radio.data.analog[1] - 127) * 100 / 128;
            rightMotor.motorSpeed = (radio.data.analog[3] - 127) * 100 / 128;
        }

    }
}
