using System;
using System.Threading;
using Microsoft.SPOT;

using System.Collections;

namespace PiEAPI
{
    public class Robot
    {
        string teamID { get; set; }
        private static string portName = "";
        private ArrayList actuators = new ArrayList();
        private ArrayList sensors = new ArrayList();
        private Radio radio;

        public Robot(string teamID)
        {
            this.teamID = teamID;
            radio = new Radio(this, portName);
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
                act.Kill();
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
                act.Enable();
            }
            foreach (Sensor sensor in sensors)
            {
                sensor.EnableSensor();
            }
        }

    }
}
