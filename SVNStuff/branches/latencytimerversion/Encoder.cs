/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v2.a - 04/08/11
*/

using System;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using System.IO.Ports;
using GHIElectronics.NETMF.FEZ;
using System;

namespace PiEAPI
{
    /// <summary>
    /// Represents an encoder channel on the PiE Robot Controller.
    /// </summary>
    /// 
    /// Interfaces with the ATtiny2313A encoder counter chip on the
    /// PiE External Board 2011 (rev 4), using the Pololu protocol.
    public class Encoder
    {
        private const byte CMD_READ = 1,
            CMD_RESET = 2,
            CMD_SET_DOUBLE_RESOLUTION = 3,
            CMD_SET_SINGLE_RESOLUTION = 4;

        private const byte POLOLU_ADDRESS = 10;

        private SerialPort serial;
        private byte[] outBuf = new byte[3];
        private byte channel;

        // Speed tracking members
        private long startTime = DateTime.Now.Ticks;
        private int startCounts = 0;

        // Static initialization
        // TODO share serial port with Micro Maestro
        // The ATtiny2313A is programmed for a 38400 baud UART.

        /// <summary>
        /// Initialize a quadrature encoder on the given channel.
        /// </summary>
        /// <param name="channel">encoder channel (0-3)</param>
        public Encoder(Robot robot, byte channel)
        {
            this.channel = channel;

            //create new port or use already existing port
            foreach (SerialPort pts in robot.ports)
            {
                if (pts.PortName == "COM2")
                {
                    serial = pts;
                }
            }


            if (serial == null)
            {
                serial = new SerialPort("COM2", 38400, Parity.None, 8, StopBits.One);
                robot.ports.Add(serial);
                serial.ReadTimeout = 2;
                serial.Open();
            }

            // Initialize start byte and address in output buffer
            outBuf[0] = 0xAA;
            outBuf[1] = POLOLU_ADDRESS;

            this.Reset();
        }

        /// <summary>
        /// Initialize a quadrature encoder on the given channel
        /// in the given resolution mode.
        /// 
        /// In double-resolution mode, both rising and falling edges on pin A
        /// are counted.
        /// In single-resolution mode, only rising edges on pin A are counted.
        /// </summary>
        /// <param name="channel">encoder channel (0-3)</param>
        /// <param name="doubleResolution">true for double resolution, 
        ///     false for single resolution</param>
        public Encoder(Robot robot, byte channel, bool doubleResolution) : 
            this(robot, channel)
        {
            // Set resolution mode (defaults to double resolution)
            if (!doubleResolution)
            {
                outBuf[2] = (byte)((CMD_SET_SINGLE_RESOLUTION << 2) | 
                    (channel & 0x3));
                serial.Write(outBuf, 0, outBuf.Length);
            }
        }

        /// <summary>
        /// Read the encoder count, an integer value representing 
        /// total rotated distance.
        /// </summary>
        /// <returns>encoder count as a 32-bit signed int</returns>
        public int Read()
        {
            byte[] inBuf = new byte[4];
            int count;

            outBuf[2] = (byte)((CMD_READ << 2) | (channel & 0x3));
            serial.Write(outBuf, 0, outBuf.Length);
            serial.Read(inBuf, 0, 4);

            // Convert (little-endian) byte array to Int32
            count = (int)Utility.ExtractValueFromArray(inBuf, 0, 4);
            return count;
        }

        /// <summary>
        /// Resets the encoder value to 0.
        /// </summary>
        public void Reset()
        {
            outBuf[2] = (byte)((CMD_RESET << 2) | (channel & 0x3));
            serial.Write(outBuf, 0, outBuf.Length);
        }

        /// <summary>
        /// Returns encoder speed in counts per second.
        /// Note that this method requires subsequent calls to be close in time
        /// for accurate readings.
        /// </summary>
        /// <returns> Speed in ticks per second. </returns>
        public int Speed()
        {
           
            // Record current quantities
            long currentTime = DateTime.Now.Ticks;
            int currentCounts = this.Read();

            // Calculate elapsed quantities
            int timeElapsed = (int)(currentTime - this.startTime);
            int countsElapsed = currentCounts - this.startCounts;
            
            // Reset speed measuring interval
            this.startTime = DateTime.Now.Ticks;
            this.startCounts = currentCounts;
            Debug.Print("Counts Elapsed: " + countsElapsed + "\tTime Elapsed: " + timeElapsed);
            return (countsElapsed * 10000000) / timeElapsed;
            
        }
    }
}
