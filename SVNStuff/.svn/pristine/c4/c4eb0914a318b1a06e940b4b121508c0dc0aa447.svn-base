using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO.Ports;
using System.Collections;
using GHIElectronics.NETMF.FEZ;
using A6281;


namespace PiERFramework
{
    public class Robot
    {
        public ArrayList actuators;
        public ArrayList sensors;
        public ArrayList ports;

        public Radio_Series1 radio;
        public StudentCode student;

        // <summary>current match time in seconds property with private set and public get.</summary>
        public int fieldTime { get; private set; }
        // <summary>teamID property with private set and public get.</summary>
        public string teamID { get; private set; }
        // <summary>scores property that returns all teams scores.</summary>
        public int[] scores { get; private set; }
        // <summary>winningScore property that returns the score needed to win.</summary>
        public int winningScore { get; private set; }
        // <summary>canMove property that returns true if the robot can move.</summary>
        public bool canMove { get; private set; }
        // <summary>isAutonomous property that returns true if robot is in autonomous mode.</summary>
        public bool isAutonomous { get; private set; }
        // <summary>isAutonomous property that returns true if robot is blue team.</summary>
        public bool isBlue { get; private set; }
        // <summary>UIAnalogVals property that returns the interface's analog values.</summary>
        public int[] UIAnalogVals { get; private set; }
        // <summary>UIDigitalVals property that returns the interface's digital values.</summary>
        public bool[] UIDigitalVals { get; private set; }

        // Time Tracker
        private long previousTime;
        private long period = 5000000;

        public A6281.Single shiftBrite;

        // LEDs
        public OutputPort yellowLED;
        public OutputPort redLED;

        // Threads
        private Thread studentCodeThread;
        private Thread autonomousThread;
        private Thread rfPollThread;
        private Thread rfTeleThread;

        // Event Wait Handlers
        // public ManualResetEvent scWait { get; private set; }	//studentcode
        // public ManualResetEvent autoWait { get; private set; }	//autonomous

        // Lock
        public readonly object radioLock = new object();

        // Constants
        private const int ROBOT_SLEEP_TIME = 0;
        private const int STUDENT_SLEEP_TIME = 0;
        private const int AUTONO_SLEEP_TIME = 0;
        private const int RFPOLL_SLEEP_TIME = 0;
        private const int RFTELE_SLEEP_TIME = 100;

        // Debug flag
        public const bool DEBUG_FLAG = true;

        // <summary>
        // Main method which initializes the robot, and creates
        // the radio's and student code's threads. Begins running
        // the threads, and then runs the robot thread indefinitely.
        // </summary>
        public static void Main()
        {

            // Initialize robot
            Robot robot = new Robot("1", "COM4");
            robot.Auton(false);
            robot.student = new StudentCode(robot);

            // Create and set radio threads
            robot.rfPollThread = new Thread(robot.RunRFPoll);
            robot.rfPollThread.Priority = ThreadPriority.AboveNormal;
            robot.rfTeleThread = new Thread(robot.RunRFTele);
            robot.rfTeleThread.Priority = ThreadPriority.AboveNormal;

            // Create and set student code threads
            robot.studentCodeThread = new Thread(robot.RunStudent);
            robot.studentCodeThread.Priority = ThreadPriority.BelowNormal;
            robot.autonomousThread = new Thread(robot.RunAutono);
            robot.autonomousThread.Priority = ThreadPriority.BelowNormal;

            // Initialize LEDs
            robot.yellowLED = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.IO59, false);
            robot.redLED = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.IO60, false);

            // Disable 
            Debug.EnableGCMessages(false);

            // Run the robot once to initialize
            robot.Run();

            // Start running all the threads
            robot.studentCodeThread.Start();
            robot.autonomousThread.Start();
            robot.rfPollThread.Start();
            robot.rfTeleThread.Start();

            // Run robot supervisor code
            while (true)
            {
                //Debug.Print("S: RunRobot");
                robot.Run();
                //Debug.Print("E: RunRobot");
                Thread.Sleep(ROBOT_SLEEP_TIME);
            }
        }

        // <summary>
        // Continually run student teleoperated code w/ yielding
        // </summary>
        private void RunStudent()
        {
            while (true)
            {
                //Debug.Print("S: RunStudent");
                student.UserControlledCode();
                //Debug.Print("E: RunStudent");
                Thread.Sleep(STUDENT_SLEEP_TIME);
            }
        }

        // <summary>
        // Continually run student autonomous code w/ yielding
        // </summary>
        private void RunAutono()
        {
            while (true)
            {
                //Debug.Print("S: RunAutono");
                student.AutonomousCode();
                //Debug.Print("E: RunAutono");
                Thread.Sleep(AUTONO_SLEEP_TIME);
            }
        }

        // <summary>
        // Continually run poll code in the radio class w/ yielding
        // </summary>
        private void RunRFPoll()
        {
            while (true)
            {
                //Debug.Print("S: RunRFPoll");
                radio.Poll();
                //Debug.Print("E: RunRFPoll");
                Thread.Sleep(RFPOLL_SLEEP_TIME);
            }
        }

        // <summary>
        // Continually run telemetry code in the radio class w/ yielding
        // </summary>
        private void RunRFTele()
        {
            while (true)
            {
                //Debug.Print("S: RunRFTele");
                radio.Telemetry();
                //Debug.Print("E: RunRFTele");
                Thread.Sleep(RFTELE_SLEEP_TIME);
            }
        }

        // <summary>
        // Robot Constructor: initilizes variables
        // </summary>
        // <param name="teamID">Team ID</param>
        // <param name="radioComPort">Radio Com Port</param>
        public Robot(string teamID, String radioComPort)
        {
            actuators = new ArrayList();
            sensors = new ArrayList();
            ports = new ArrayList();
            canMove = true;
            isAutonomous = false; // Not sure about this

            //autoWait = new ManualResetEvent(false);
            //scWait = new ManualResetEvent(false);

            this.teamID = teamID;
            radio = new Radio_Series1(this, radioComPort);

            shiftBrite = new A6281.Single(Cpu.Pin.GPIO_NONE, (Cpu.Pin)FEZ_Pin.Digital.Di9, (Cpu.Pin)FEZ_Pin.Digital.Di10, SPI.SPI_module.SPI1);
            previousTime = 0;

            //set the team color for the shiftBrite
            if (!isBlue)
            {
                shiftBrite.SetColorImmediate(650, 350, 0); // sets gold
            }
            else
            {
                shiftBrite.SetColorImmediate(0, 0, 500); // sets blue
            }

            // Make a deep copy of the UI values so they don't change mid-update
            UIAnalogVals = (int[])radio.UIAnalogVals.Clone();
            UIDigitalVals = (bool[])radio.UIDigitalVals.Clone();
        }

        // Set isAutonomous
        public void Auton(bool b)
        {
            isAutonomous = b;
        }

        // <summary>
        // Robot Thread constantly calls this to update the actuator
        // values or kill them if necessary.
        // </summary>
        private void Run()
        {
            // acquire the radioLock in order to safely update the values
            lock (radioLock)
            {
                bool prevCanMove = canMove;
                // Make a deep copy of the UI values so they don't change mid-update
                UIAnalogVals = (int[])radio.UIAnalogVals.Clone();
                UIDigitalVals = (bool[])radio.UIDigitalVals.Clone();
                fieldTime = radio.fieldTime;
                canMove = radio.canMove;
                isAutonomous = radio.isAutonomous;
                isBlue = radio.isBlue;

                //specifically revive actuators if field wanted to revive actuators.
                if(!prevCanMove && canMove)
                {
                    foreach (ActuatorController act in actuators)
                    {
                        act.ReviveActuators(); // update each actuator's state using its actuator controller's "update actuators" method.
                    }
                }

                // Debug printout of UI values
                /*
                if (DEBUG_FLAG)
                {
                    string radVals = "";
                    foreach (int val in UIAnalogVals)
                    {
                        radVals += val + " ";
                    }
                    //Debug.Print(" Robot: " + radVals);
                }
                 */
            }

            // kill or revive actuators based on the canMove bool
            if (canMove == true)
            {
                foreach (ActuatorController act in actuators)
                {
                    act.UpdateActuators(); // update each actuator's state using its actuator controller's "update actuators" method.
                }
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
                //scWait.Reset();		// Tell StudentCode thread to wait
                //autoWait.Set();		// Tell autonomous thread to go

                if (DateTime.Now.Ticks >= previousTime + period)
                {
                    shiftBrite.On = !shiftBrite.On;
                    previousTime = DateTime.Now.Ticks;
                    //Debug.Print("Current time" + previousTime);
                    shiftBrite.On = true;
                }
            }
            /*
                else
                {
                    autoWait.Reset();	// Tell autonomous thread to wait
                    scWait.Set();		// Tell StudentCode thread to go
                }
             */
        }

        // <summary>
        // Kills actuators; can be called by StudentCode or Robot thread
        // </summary>
        public void KillActuators()
        {
            foreach (ActuatorController act in actuators)
            {
                act.KillActuators();
            }
        }

        // <summary>
        // Revives actuators; can be called by StudentCode or Robot thread
        // </summary>
        public void ReviveActuators()
        {
            foreach (ActuatorController act in actuators)
            {
                act.ReviveActuators();
            }
        }

    }
}
