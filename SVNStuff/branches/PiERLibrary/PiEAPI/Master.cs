using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO.Ports;
using System.Collections;
using GHIElectronics.NETMF.FEZ;
using A6281;

/*
 *This is the main class of the robot. It starts program threads and manages device communication. 
 *When in doubt, do not change this code!
 */
namespace PiEAPI
{
    public class Master
    {
        /// <summary>
        /// The student code to run
        /// </summary>
        private RobotCode student;
        /// <summary>
        /// The robot to run the code on
        /// </summary>
        private Robot robot;
        
        // Threads
        private Thread studentCodeThread;
        private Thread autonomousThread;
        private Thread rfPollThread;
        private Thread rfTeleThread;

        // Event Wait Handlers
        // public ManualResetEvent scWait { get; private set; }	//studentcode
        // public ManualResetEvent autoWait { get; private set; }	//autonomous
        
        // Constants
        private const int ROBOT_SLEEP_TIME = 0;
        private const int STUDENT_SLEEP_TIME = 75;
        private const int AUTONO_SLEEP_TIME = 0;
        private const int RFPOLL_SLEEP_TIME = 0;
        private const int RFTELE_SLEEP_TIME = 100;

        /// <summary>
        /// Constructor to be called by Student Code.
        /// Creates the radio's and student code's threads.
        /// </summary>
        public Master(RobotCode s)
        {
            // Makes this robot run the student's code
            student = s;
            robot = s.getRobot();
            // Create and set radio threads
            rfPollThread = new Thread(RunRFPoll);
            rfPollThread.Priority = ThreadPriority.AboveNormal;
            rfTeleThread = new Thread(RunRFTele);
            rfTeleThread.Priority = ThreadPriority.AboveNormal;

            // Create and set student code threads
            studentCodeThread = new Thread(RunStudent);
            studentCodeThread.Priority = ThreadPriority.AboveNormal;
            autonomousThread = new Thread(RunAutono);
            autonomousThread.Priority = ThreadPriority.BelowNormal;

            // Initialize LEDs
            robot.yellowLED = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.IO59, false);
            robot.redLED = new OutputPort((Cpu.Pin)FEZ_Pin.Digital.IO60, false);

            // Disable 
            Debug.EnableGCMessages(false);

            // Run the robot once to initialize
            robot.Run();
        }

        /// <summary>
        /// Begins running the threads, and then runs the robot thread indefinitely.
        /// </summary>
        public void RunCode()
        {
            // Set robot thread priority to AboveNormal
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;

            // Start running all the threads
            studentCodeThread.Start();
            autonomousThread.Start();
            rfPollThread.Start();
            rfTeleThread.Start();

            // Run robot supervisor code
            while (true)
            {
                robot.Run();
                Thread.Sleep(ROBOT_SLEEP_TIME);
            }
        }

        /// <summary>
        /// Returns the robot controlled by this student code
        /// </summary>
        public Robot getRobot()
        {
            return robot;
        }

        /// <summary>
        /// Continually run student teleoperated code w/ yielding
        /// </summary>
        private void RunStudent()
        {
            while (true)
            {
            if (robot.isAutonomous == false)
                {
                    student.UserControlledCode();
                    Thread.Sleep(STUDENT_SLEEP_TIME);
                }
            }
        }

        /// <summary>
        /// Continually run student autonomous code w/ yielding
        /// </summary>
        private void RunAutono()
        {
            while (true)
            {
                if (robot.isAutonomous == true)
                {
                    student.AutonomousCode();
                    Thread.Sleep(AUTONO_SLEEP_TIME);
                }
            }
        }

        /// <summary>
        /// Continually run poll code in the radio class w/ yielding
        /// </summary>
        private void RunRFPoll()
        {
            while (true)
            {
                robot.radio.Poll();
                Thread.Sleep(RFPOLL_SLEEP_TIME);
            }
        }

        /// <summary>
        /// Continually run telemetry code in the radio class w/ yielding
        /// </summary>
        private void RunRFTele()
        {
            while (true)
            {
                robot.radio.Telemetry();
                Thread.Sleep(RFTELE_SLEEP_TIME);
            }
        }
    }
}