
/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v0.2b - 04/14/11
 * 
 * Changelog:
 * v0.2b
 *  - Move teleoperated/autonomous checking from StudentCode.cs to robot threads
 *  - Added heartbeat timeout
*/

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO.Ports;
using System.Collections;
using GHIElectronics.NETMF.FEZ;
using A6281;

namespace PiEAPI
{
    public class Robot
    {
        /// <summary>
        /// List of actuator controllers on this robot
        /// </summary>
        public ArrayList actuators;
        /// <summary>
        // List of sensors on this robot       
        /// </summary>
        public ArrayList sensors;
        /// <summary>
        /// List of open communication ports on this robot
        /// </summary>      
        public ArrayList ports;
        /// <summary>
        /// This robot's radio
        /// </summary>
        public Radio_Series1 radio;
        /// <summary>
        /// This robot's ShiftBrite
        /// </summary>
        public A6281.Single shiftBrite;

        /// <summary>
        /// current match time in seconds property with private set and public get.
        /// </summary>
        public int fieldTime { get; private set; }
        /// <summary>
        /// teamID property with private set and public get.
        /// </summary>
        public string teamID { get; private set; }
        /// <summary>
        /// scores property that returns all teams scores.
        /// </summary>
        public int[] scores { get; private set; }
        /// <summary>
        /// winningScore property that returns the score needed to win.
        /// </summary>
        public int winningScore { get; private set; }
        /// <summary>
        /// canMove property that returns true if the robot can move.
        /// </summary>
        public bool canMove { get; private set; }
        /// <summary>
        /// isAutonomous property that returns true if robot is in autonomous mode.
        /// </summary>
        public bool isAutonomous { get; private set; }
        /// <summary>
        /// isAutonomous property that returns true if robot is blue team.
        /// </summary>
        public bool isBlue { get; private set; }
        /// <summary>
        /// UIAnalogVals property that returns the interface's analog values.
        /// </summary> 
        public int[] UIAnalogVals { get; private set; }
        /// <summary>
        /// UIDigitalVals property that returns the interface's digital values.
        /// </summary>
        public bool[] UIDigitalVals { get; private set; }

        // ShiftBrite Timer
        private long shiftBriteTimer;
        private static long shiftBritePeriod = 5000000;

        // Heartbeat Timer
        private long heartbeatTimer;
        private static long heartbeatPeriod = 2 * 10000000;
        /// <summary>
        /// I2C Device used by all I2C components on robot
        /// </summary>
        public I2CDevice i2c;
        /// <summary>
        /// Port representation of the yellow LED pin.
        /// </summary>
        public OutputPort yellowLED;
        /// <summary>
        /// Port representation of the red LED pin.
        /// </summary>
        public OutputPort redLED;

        /// <summary>
        /// Robot Constructor: initilizes variables
        /// </summary>
        /// <param name="teamID">Team ID</param>
        /// <param name="radioComPort">Radio Com Port</param>
        public Robot(string teamID, String radioComPort)
        {
            actuators = new ArrayList();
            sensors = new ArrayList();
            ports = new ArrayList();
            canMove = true;
            isAutonomous = false;
            this.teamID = teamID;
            radio = new Radio_Series1(this, radioComPort);

            I2CDevice.Configuration conA = new I2CDevice.Configuration(0x0C, 100);
            i2c = new I2CDevice(conA);

            shiftBrite = new A6281.Single(Cpu.Pin.GPIO_NONE, (Cpu.Pin)FEZ_Pin.Digital.Di9, (Cpu.Pin)FEZ_Pin.Digital.Di10, SPI.SPI_module.SPI1);
            shiftBrite.On = true;
            shiftBriteTimer = 0;
            shiftBrite.SetColorImmediate(700, 700, 700);

            heartbeatTimer = DateTime.Now.Ticks;

            // Set the team color for the shiftBrite
            // Still to be implemented in 0.1c


            // Make a deep copy of the UI values so they don't change mid-update
            UIAnalogVals = (int[])radio.UIAnalogVals.Clone();
            UIDigitalVals = (bool[])radio.UIDigitalVals.Clone();
        }

        // Set isAutonomous
        public void Auton(bool b)
        {
            isAutonomous = b;
        }

        /// <summary>
        /// Robot Thread constantly calls this to update the actuator
        /// values or kill them if necessary.
        /// </summary>        
        public void Run()
        {
            bool prevCanMove = canMove;

            lock (radio.radioLock)
            {
                // Make a deep copy of the radio values so they don't change mid-update
                UIAnalogVals = (int[])radio.UIAnalogVals.Clone();
                UIDigitalVals = (bool[])radio.UIDigitalVals.Clone();
                fieldTime = radio.fieldTime;
                canMove = radio.canMove;
                isAutonomous = radio.isAutonomous;
                isBlue = radio.isBlue;
                heartbeatTimer = radio.lastUpdate;
            }

            // Check heartbeat time
            if (DateTime.Now.Ticks - heartbeatTimer > heartbeatPeriod)
            {
                canMove = false;
            }

            //Update Team Color
            if (isBlue)
            {
                shiftBrite.On = true;
                shiftBrite.SetColorImmediate(650, 350, 0);
            }
            else
            {
                shiftBrite.On = true;
                shiftBrite.SetColorImmediate(0, 0, 500);
            }

            //specifically revive actuators if field wanted to revive actuators.
            if (!prevCanMove && canMove)
            {
                foreach (ActuatorController act in actuators)
                {
                    act.ReviveActuators(); // update each actuator's state using its actuator controller's "update actuators" method.
                }
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
                if (DateTime.Now.Ticks >= shiftBriteTimer + shiftBritePeriod)
                {
                    shiftBrite.On = !shiftBrite.On;
                    shiftBriteTimer = DateTime.Now.Ticks;
                    shiftBrite.On = true;
                }
            }
        }

        /// <summary>
        /// Kills actuators; can be called by StudentCode or Robot thread
        /// </summary>
        public void KillActuators()
        {
            foreach (ActuatorController act in actuators)
            {
                act.KillActuators();
            }
        }

        /// <summary>
        /// Revives actuators; can be called by StudentCode or Robot thread
        /// </summary>
        public void ReviveActuators()
        {
            foreach (ActuatorController act in actuators)
            {
                act.ReviveActuators();
            }
        }

    }
}