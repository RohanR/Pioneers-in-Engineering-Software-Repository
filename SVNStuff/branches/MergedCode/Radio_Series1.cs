/*
 * University of California, Berkeley
 * Pioneers in Engineering, Robotics Organizer.
 * PiER Framework v0.2b - 04/14/11
 * 
 * Changelog:
 * v0.2b
 *  - Formatting changes
 *  - Added update timestamping
*/

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;

/*
 * XBee series 1 driver code. Do not change. 
 */
namespace PiEAPI
{
    //interface packet properties
    //must be reflected in PiEMOS configuration
    enum INTERFACEPACKET
    {
        IDENT = 0xFE,
        ANALOG_BYTES = 7,
        DIGITAL_BYTES = 1
    }

    //telemetry packet properties
    //must be reflected in PiEMOS configuration
    enum TELEMETRYPACKET
    {
        IDENT = 0xFD,
        ANALOG_BYTES = 7,
        DIGITAL_BYTES = 1
    }

    public class Radio_Series1
    {

        public enum RadioState { IDLE, RECEIVING_PACKET }

        public SerialPort port;
        public Thread radioThread;
        public Thread telemetryThread;
        public RadioState state;
        public byte[] buffer;
        public InterfacePacket data;
        public TelemetryPacket telemetry;
        public long lastUpdate;

        public Robot robot;
        public int[] UIAnalogVals { get; private set; }
        public bool[] UIDigitalVals { get; private set; }

        public byte fieldTime;
        public bool canMove;
        public bool isAutonomous;
        public bool isBlue;

        public Boolean packetReceived;
        public XBeeInterfaceReceiver receiver;
        public UInt64 interfaceAddress;
        public byte[] interfaceAddressBytes;
        public UInt64 fieldAddress;

        public byte frame;

        // Read lock
        public readonly object radioLock = new object();

        public Radio_Series1(Robot robo, string portName)
        {
            robot = robo;
            UIAnalogVals = new int[(int)INTERFACEPACKET.ANALOG_BYTES];
            UIDigitalVals = new bool[(int)INTERFACEPACKET.DIGITAL_BYTES * 8];

            state = RadioState.IDLE;
            port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            port.ReadTimeout = 2;
            port.Open();

            data = new InterfacePacket((byte)INTERFACEPACKET.IDENT, (int)INTERFACEPACKET.ANALOG_BYTES, (int)INTERFACEPACKET.DIGITAL_BYTES);
            telemetry = new TelemetryPacket((byte)TELEMETRYPACKET.IDENT, (int)TELEMETRYPACKET.ANALOG_BYTES, (int)TELEMETRYPACKET.DIGITAL_BYTES);
            buffer = new byte[512];
            receiver = new XBeeInterfaceReceiver(this);

            lastUpdate = DateTime.Now.Ticks;

            frame = 0;
        }

        public void InterfacePrint(string str)
        {
            char[] chars = str.ToCharArray();
            InterfacePrint(chars);
        }

        public void InterfacePrint(char[] chars)
        {
            if (interfaceAddressBytes != null)
            {
                byte[] pBuffer = new byte[chars.Length + 2];
                pBuffer[0] = 0xFC;
                for (int i = 1; i < pBuffer.Length - 1; i++)
                {
                    pBuffer[i] = (byte)chars[i - 1];
                }
                byte[] req = TransmitRequest(interfaceAddressBytes, frame, pBuffer);
                frame++;
                if (frame > 126) frame = 0;
                port.Write(req, 0, req.Length);
            }

            return;
        }

        public void Telemetry()
        {
            //Debug.Print("Sending telemetry");
            byte[] req = TransmitRequest(interfaceAddressBytes, frame, telemetry.GetBuffer());
            frame++;
            if (frame > 126) frame = 0;
            port.Write(req, 0, req.Length);

            //Thread.Sleep(100);
        }

        public void Poll()
        {
            int availableBytes;
            robot.yellowLED.Write(false);
            if (port.BytesToRead > 0)
            {
                robot.yellowLED.Write(true);
                availableBytes = port.BytesToRead;
                port.Read(buffer, 0, availableBytes);

                // If anything at all was received by radio
                if (availableBytes > 0)
                {
                    // And if full packet was received
                    if (receiver.Fill(buffer, availableBytes))
                    {
                        // Blink RF Activity LED
                        robot.yellowLED.Write(true);

                        // If packet belongs to us, copy it over
                        if (receiver.packet.data[0] == data.ident)
                        {
                            data.FillData(receiver.packet.data);

                            // Mark timestamp
                            lastUpdate = DateTime.Now.Ticks;

                            //echo radio data
                            //byte[] req = TransmitRequest(receiver.packet.src64Bytes, frame, receiver.packet.data);

                            //update fields
                            lock (radioLock)
                            {
                                fieldTime = data.fieldTime;
                                canMove = data.canMove;
                                isAutonomous = data.isAutonomous;
                                isBlue = data.isBlue;
                                for (int i = 0; i < UIAnalogVals.Length; i++)
                                {
                                    int ana = (int)data.analog[i];
                                    UIAnalogVals[i] = ana;
                                    telemetry.analog[i] = (byte)ana;
                                }
                                for (int i = 0; i < UIDigitalVals.Length; i++)
                                {
                                    bool digi = (bool)data.digital[i];
                                    UIDigitalVals[i] = digi;
                                    telemetry.digital[i] = digi;
                                }
                            }
                        }
                        receiver.fullPacket = false;
                    }

                    packetReceived = true;
                }

            }

        }

        public String BytesToString(byte[] bytes, int length)
        {
            String str = "";
            int i;
            for (i = 0; i < length; i++)
            {
                str += bytes[i].ToString() + " ";
                bytes[i] = 0;
            }

            return str;
        }

        public byte[] TransmitRequest(byte[] addr, byte frameId, byte[] data)
        {
            if (addr == null)
            {
                addr = new byte[8];
                //Debug.Print("Missing address bytes");
            }
            int i;
            byte[] packet = new byte[data.Length + 15];
            packet[0] = 0x7E;
            packet[1] = 0x00;
            packet[2] = (byte)(data.Length + 11);
            packet[3] = 0x00;
            packet[4] = frameId;
            for (i = 5; i < 13; i++)
            {
                packet[i] = addr[i - 5];
            }

            packet[13] = 0x00;

            for (i = 14; i < 14 + data.Length; i++)
            {
                packet[i] = data[i - 14];
            }

            byte checksum = 0;
            for (i = 3; i < packet.Length - 1; i++)
            {
                checksum += packet[i];
            }

            checksum = (byte)(0xFF - checksum);
            packet[packet.Length - 1] = checksum;
            return packet;
        }
    }


    public class TelemetryPacket
    {
        public byte ident;
        public byte[] analog;
        public bool[] digital;
        public ushort digitalByteCount;
        public byte[] buffer;

        public TelemetryPacket(byte identByte, int analogBytes, ushort digitalBytes)
        {
            digitalByteCount = digitalBytes;
            digital = new bool[digitalBytes * 8];
            analog = new byte[analogBytes];
            ident = identByte;

            buffer = new byte[analogBytes + digitalBytes + 1];
            Debug.Print("telem len:   " + buffer.Length);
        }

        public byte[] GetBuffer()
        {
            buffer[0] = ident;
            int i;
            for (i = 1; i < analog.Length + 1; i++)
            {
                buffer[i] = analog[i - 1];
            }

            int idx = analog.Length + 1;
            for (i = 0; i < digitalByteCount; i++)
            {
                buffer[idx] = 0;
                if (digital[0 + (8 * i)]) buffer[idx] += 0x01;
                if (digital[1 + (8 * i)]) buffer[idx] += 0x02;
                if (digital[2 + (8 * i)]) buffer[idx] += 0x04;
                if (digital[3 + (8 * i)]) buffer[idx] += 0x08;
                if (digital[4 + (8 * i)]) buffer[idx] += 0x10;
                if (digital[5 + (8 * i)]) buffer[idx] += 0x20;
                if (digital[6 + (8 * i)]) buffer[idx] += 0x40;
                if (digital[7 + (8 * i)]) buffer[idx] += 0x80;
                idx++;
            }
            return buffer;
        }
    }


    public class InterfacePacket
    {

        public byte ident;
        public byte[] analog;
        public bool[] digital;
        public ushort digitalByteCount;
        public byte fieldTime;
        public bool canMove;
        public bool isAutonomous;
        public bool isBlue;

        public InterfacePacket(byte identByte, int analogBytes, ushort digitalBytes)
        {
            digitalByteCount = digitalBytes;
            digital = new bool[digitalBytes * 8];
            analog = new byte[analogBytes];
            ident = identByte;

            Debug.Print(digital.Length.ToString());
        }

        public void FillData(byte[] bytes)
        {

            fieldTime = bytes[1];
            if ((bytes[2] & 0x01) > 0) canMove = true;
            else
                canMove = false;
            if ((bytes[2] & 0x02) > 0) isAutonomous = true;
            else isAutonomous = false;

            if ((bytes[2] & 0x04) > 0) isBlue = true;
            else isBlue = false;

            //Debug.Print("len: " + bytes.Length);
            //parse bytes into analog and digital arrays
            int i;
            for (i = 0; i < analog.Length; i++)
            {
                analog[i] = bytes[i + 5];
            }

            int bit = 0;

            //Debug.Print("" + (analog.Length + digitalByteCount));

            for (i++; i < analog.Length + digitalByteCount + 1; i++)
            {
                //Debug.Print("DIGITAL BYTE:  " + (i - analog.Length -1));
                //Debug.Print("byte: " + bytes[i+4]);
                if ((bytes[i + 4] & 0x01) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i + 4] & 0x02) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i + 4] & 0x04) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i + 4] & 0x08) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i + 4] & 0x10) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i + 4] & 0x20) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i + 4] & 0x40) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i + 4] & 0x80) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;
            }

            /*   //Print out digital banks
            string str = "";
            for (i = 0; i < digital.Length; i++)
            {
                str += digital[i].ToString() + " ";
            }
            Debug.Print("DIGITAL BANK:   " + str);
            */
        }


    }

    public class XBeeInterfaceReceiver
    {
        enum RXState { IDLE, LEN, COMMAND, SRC64, SRC16, RSSI, OPTIONS, DATA, CHECKSUM, UNKNOWN, ERROR }
        enum XBeeCommands { RX64 = 0x80, RX16 = 0x81, TX = 0x89 }
        private RXState state;
        public byte[] buffer;
        public int bytesRead;
        public XBeeRXPacket packet;
        public Boolean fullPacket;
        private Radio_Series1 parent;

        public XBeeInterfaceReceiver(Radio_Series1 p)
        {
            state = (int)RXState.IDLE;
            buffer = new byte[128];
            packet = new XBeeRXPacket();
            parent = p;
        }

        public Boolean Fill(byte[] bytes, int len)
        {
            byte b;
            int i;
            for (i = 0; i < len; i++)
            {
                b = bytes[i];
                switch (state)
                {
                    case RXState.IDLE:
                        if (b == 0x7E)
                        {
                            bytesRead = 0;
                            state = RXState.LEN;
                        }
                        break;

                    case RXState.LEN:
                        if (bytesRead == 0)
                        {
                            packet.length = (uint)b << 8;
                        }
                        else if (bytesRead == 1)
                        {
                            packet.length |= b;
                        }
                        bytesRead++;
                        if (bytesRead == 2)
                        {
                            bytesRead = 0;
                            state = RXState.COMMAND;
                        }
                        break;

                    case RXState.COMMAND:
                        if (b == (byte)XBeeCommands.RX64)
                        {
                            packet.command = b;
                            packet.data = new byte[packet.length - 11];
                            packet.src64 = 0;
                            state = RXState.SRC64;
                        }
                        else if (b == (byte)XBeeCommands.TX)
                        {
                            bytesRead = 0;
                            state = RXState.UNKNOWN;
                        }
                        else
                        {
                            bytesRead = 0;
                            Debug.Print("UNKNOWN PACKET!! " + b);
                            state = RXState.UNKNOWN;
                        }
                        break;

                    case RXState.SRC64:
                        //Read 64bit Sender's Address
                        packet.src64Bytes[bytesRead] = b;
                        packet.src64 += (UInt64)b << (8 * bytesRead);

                        bytesRead++;
                        if (bytesRead == 8)
                        {
                            bytesRead = 0;
                            state = RXState.RSSI;

                            //get first interface packet and store 64bit address
                            if (parent.interfaceAddress == 0)
                            {
                                parent.interfaceAddress = packet.src64;
                                parent.interfaceAddressBytes = packet.src64Bytes;
                                //parent.telemetryThread.Start();
                            }
                            else
                            {
                                //ignore interface packets from the wrong address
                                if (parent.interfaceAddress != packet.src64)
                                {
                                    //state = (int)RXState.UNKNOWN;
                                }
                            }

                        }
                        break;

                    case RXState.SRC16:
                        //Read 16bit Sender's Address
                        packet.src16 = (ushort)((packet.src16 << 8) | b);
                        bytesRead++;

                        if (bytesRead == 2)
                        {
                            bytesRead = 0;
                            state = RXState.RSSI;
                        }
                        break;

                    case RXState.RSSI:
                        packet.rssi = b;
                        state = RXState.OPTIONS;
                        break;

                    case RXState.OPTIONS:
                        packet.options = b;
                        state = RXState.DATA;
                        break;

                    case RXState.DATA:
                        //The beef
                        packet.data[bytesRead] = b;
                        bytesRead++;
                        if (bytesRead == packet.data.Length)
                        {
                            state = RXState.CHECKSUM;
                        }
                        break;

                    case RXState.CHECKSUM:
                        fullPacket = true;
                        state = RXState.IDLE;
                        break;

                    case RXState.UNKNOWN:
                        //ignore all bytes in packet before trying to parse again (all XBee API functions generate a confirmation packet sent as a packet)
                        //We dont care about most of them.
                        //We can get information about the current XBee module (like requesting its serial number or coordinator or network status)
                        bytesRead++;
                        if (bytesRead == packet.length)
                        {
                            state = RXState.IDLE;
                            bytesRead = 0;
                        }
                        break;
                }
            }

            if (fullPacket) return true;
            return false;
        }
    }

    public class XBeeRXPacket
    {
        public uint length;
        public byte command;
        public byte[] src64Bytes;
        public UInt64 src64;
        public UInt16 src16;
        public byte rssi;
        public byte options;
        public byte[] data;
        public byte checksum;

        public XBeeRXPacket()
        {
            src64Bytes = new byte[8];
            return;
        }
        public XBeeRXPacket(uint aLength, byte aCommand, UInt64 aSrc64, byte aRssi, byte aOptions, byte[] aData, byte aChecksum)
        {
            length = aLength;
            command = aCommand;
            src64 = aSrc64;
            //src16 = aSrc16;
            rssi = aRssi;
            options = aOptions;
            data = aData;
            checksum = aChecksum;
            src64Bytes = new byte[8];
            return;
        }

        public string FormatString()
        {
            string str = "XB";
            str += "  LEN " + length;
            str += "  COM " + command;
            str += "  S64 " + src64;
            str += "  S16 " + src16;
            str += "  OPT " + options;
            str += "  DAT " + BytesToString(data);
            str += "  CHK " + checksum;
            return str;
        }

        public string BytesToString(byte[] bytes)
        {
            string str = "";
            int i = 0;
            for (i = 0; i < bytes.Length; i++)
            {
                str += bytes[i].ToString() + " ";
            }
            return str;
        }
    }
}