using System;
using System.Threading;
using Microsoft.SPOT;
using System.IO.Ports;

using System.Collections;

namespace PiEAPI
{
    public class Robot
    {
        public ArrayList actuators;
        public ArrayList sensors;
        public Radio radio;
        public StudentCode student;
        public SerialPort motorPort;

        public string teamID { get; private set; }
        public int[] scores { get; private set; }
        public int winningScore { get; private set; }
        public bool canMove { get; private set; }
        public bool isAutonomous { get; private set; }
        public bool RobotSet { get; private set; }
        public int[] UIAnalogVals { get; private set; }
        public bool[] UIDigitalVals { get; private set; }
        public int[] FieldAnalogVals { get; private set; }
        public bool[] FieldDigitalVals { get; private set; }

        // Threads
        private Thread robotThread;
        private Thread radioThread;
        private Thread studentCodeThread;
        private Thread autonomousThread;

        // Event Wait Handlers
        public ManualResetEvent scWait { get; private set; }	//studentcode
        public ManualResetEvent autoWait { get; private set; }	//autonomous

        // Lock
        public readonly object radioLock = new object();

        public Robot(string teamID, String robotComPort)
        {
            motorPort = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            motorPort.ReadTimeout = 2;
            motorPort.Open();
           // Debug.Print("set actuators");
            actuators = new ArrayList();
            sensors = new ArrayList();
            canMove = true;
            isAutonomous = false; // Not sure about this
            RobotSet = false;

            autoWait = new ManualResetEvent(false);
            scWait = new ManualResetEvent(false);

            this.teamID = teamID;
            radio = new Radio(this, robotComPort);
            student = new StudentCode(this);

            lock (radioLock)
            {
                UIAnalogVals = radio.UIAnalogVals;
                UIDigitalVals = radio.UIDigitalVals;
                FieldAnalogVals = radio.FieldAnalogVals;
                FieldDigitalVals = radio.FieldDigitalVals;
                Debug.Print(" digital length: " + UIDigitalVals.Length);
            }

            // Setup threads to maintain and control actuators based on radio input
            robotThread = new Thread(this.Run);
            //radioThread = new Thread(radio.Poll);
            studentCodeThread = new Thread(student.UserControlledCode);
            autonomousThread = new Thread(student.AutonomousCode);

            // Name the threads for debugging purposes
            /*robotThread.Name = "robotThread";
            radioThread.Name = "radioThread";
            studentCodeThread.Name = "studentCodeThread";
            autonomousThread.Name = "autonomousThread";*/


            // Start all the threads
            robotThread.Start();
            //radioThread.Start();
            studentCodeThread.Start();
            autonomousThread.Start();
        }

        public void Auton(bool b)
        {
            isAutonomous = b;
        }

        private void Run()
        {
            while (true)
            {
                // acquire the radioLock in order to safely update the values
                lock (radioLock)
                {
                    UIAnalogVals = radio.UIAnalogVals;
                    UIDigitalVals = radio.UIDigitalVals;
                    FieldAnalogVals = radio.FieldAnalogVals;
                    FieldDigitalVals = radio.FieldDigitalVals;
                }
                    
                // kill or revive actuators based on the canMove bool
                if (canMove == true)
                {
                    foreach (ActuatorController act in actuators)
                    {
                        act.ReviveActuators(); // if actuator is already revived, this will not do anything
                    }

                    // update each actuator's state using its actuator controller's "update actuators" method.
                    RobotSet = true;
                    foreach (ActuatorController act in actuators)
                    {
                        act.UpdateActuators();
                    }
                    RobotSet = false;
                }
                else
                {
                    foreach (ActuatorController act in actuators)
                    {
                        act.KillActuators(); // if actuator is already killed, this will not do anything
                    }
                }

                if (isAutonomous)
                {
                    scWait.Reset();		// Tell StudentCode thread to wait
                    autoWait.Set();		// Tell autonomous thread to go
                }
                else
                {
                    autoWait.Reset();	// Tell autonomous thread to wait
                    scWait.Set();		// Tell StudentCode thread to go
                }

            }
        }

        // kills actuators, can be called by StudentCode or Robot thread
        public void KillActuators()
        {
            foreach (ActuatorController act in actuators)
            {
                act.KillActuators();
            }
        }

        // revives actuators, can be called by StudentCode or Robot thread
        public void ReviveActuators()
        {
            foreach (ActuatorController act in actuators)
            {
                act.ReviveActuators();
            }
        }

    }
}
/*using System;
using System.Threading;
using Microsoft.SPOT;

using System.Collections;

namespace PiEAPI
{
    public class Robot
    {
        private string id;
        private int[] scrs; //scores 
        private int winningScr; //winning score
        private bool canMv; //canMove
        private bool isAuton; //isAutonomous
        private int[] uiAnalogVals;
        private bool[] uiDigitalVals;
        private int[] fieldAnalogVals;
        private bool[] fieldDigitalVals;

        public ArrayList actuators;
        public ArrayList sensors;
        public Radio radio;

        public readonly object radioLock = new object(); // Lock

        public string teamID 
        {
            get
            {
                return id;
            }
            private set
            {
                id = value;
            }
        }

        public int[] scores
        {
            get
            {
                return scrs;
            }
            private set
            {
                scrs = value;
            }
        }
        public int winningScore
        {
            get
            {
                return winningScr;
            }
            private set
            {
                winningScr = value;
            }
        }
        public bool canMove
        {
            get
            {
                return canMv;
            }
            private set
            {
                canMv = value;
            }
        }
        public bool isAutonomous
        {
            get
            {
                return isAuton;
            }
            private set
            {
                isAuton = value;
            }
        }
        public int[] UIAnalogVals
        {
            get
            {
                return uiAnalogVals;
            }
            private set
            {
                uiAnalogVals = value;
            }
        }
        public bool[] UIDigitalVals
        {
            get
            {
                return uiDigitalVals;
            }
            private set
            {
                uiDigitalVals = value;
            }
        }
        public int[] FieldAnalogVals
        {
            get
            {
                return fieldAnalogVals;
            }
            private set
            {
                fieldAnalogVals = value;
            }
        }
        public bool[] FieldDigitalVals
        {
            get
            {
                return fieldDigitalVals;
            }
            private set
            {
                fieldDigitalVals = value;
            }
        }

        public Robot(string teamID, String robotComPort)
        {
            this.teamID = teamID;
            radio = new Radio(this, robotComPort);
			actuators = new ArrayList();
			sensors = new ArrayList();
            canMove = true;

			lock (radioLock)
            {
				UIAnalogVals = radio.UIAnalogVals;
				UIDigitalVals = radio.UIDigitalVals;
				FieldAnalogVals = radio.FieldAnalogVals;
				FieldDigitalVals = radio.FieldDigitalVals;
            }

            //Starts the thread to maintain and control actuators based on radio input
            Thread robotThread = new Thread(this.Run);
            robotThread.Start();
        }

        private void Run()
        {
            while (true)
            {
                // acquire the radioLock in order to safely update the values
                lock (radioLock)
                {
                    UIAnalogVals = radio.UIAnalogVals;
                    UIDigitalVals = radio.UIDigitalVals;
                    FieldAnalogVals = radio.FieldAnalogVals;
                    FieldDigitalVals = radio.FieldDigitalVals;
                }

                // kill or revive actuators based on the canMove bool
                if (canMove == true)
                {
                    foreach (ActuatorController act in actuators)
                    {
                        act.ReviveActuators(); // if actuator is already revived, this will not do anything
                    }

                    // update each actuator's state using its actuator controller's "update actuators" method.
                    if (isAutonomous == false)
                    {
                        foreach (ActuatorController act in actuators)
                        {
                            act.UpdateActuators();
                        }
                    }
                }
                else
                {
                    foreach (ActuatorController act in actuators)
                    {
                        act.KillActuators(); // if actuator is already killed, this will not do anything
                    }
                }

                Thread.Sleep(100);
            }
        }

        // kills actuators, can be called by StudentCode or Robot thread
        public void KillActuators()
        {
            foreach (ActuatorController act in actuators)
            {
                act.KillActuators();
            }
        }

        // revives actuators, can be called by StudentCode or Robot thread
        public void ReviveActuators()
        {
            foreach (ActuatorController act in actuators)
            {
                act.ReviveActuators();
            }
        }

    }
}*/
