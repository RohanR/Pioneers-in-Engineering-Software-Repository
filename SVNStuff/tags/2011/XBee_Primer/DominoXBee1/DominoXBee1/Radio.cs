

using System;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using GHIElectronics.NETMF.FEZ;
using System.Text;
using System.IO.Ports;


namespace DominoXBee1
{



    enum PACKET
    {
        ANALOG_BYTES = 7,
        DIGITAL_BYTES = 1
    }

    class Radio
    {
        

        public enum RadioState { IDLE, RECEIVING_PACKET }

        public SerialPort port;
        public Thread radioThread;
        public RadioState state;
        public byte[] buffer;
        public InterfacePacket data;
        public Boolean interfacePacketReceived;
        public Boolean packetReceived;
        public XBeeReceiver receiver;

        public byte frame;

        public Radio(ushort address)
        {
            state = RadioState.IDLE;
            port = new SerialPort("COM1", 9600, Parity.None, 8, StopBits.One);
            port.ReadTimeout = 2;

            port.Open();

            data = new InterfacePacket(0xFE, (int)PACKET.ANALOG_BYTES, (int)PACKET.DIGITAL_BYTES);

            buffer = new byte[128];
            
            receiver = new XBeeReceiver();
            frame = 0;
            radioThread = new Thread(new ThreadStart(this.Poll));
            radioThread.Start();
        }

        public void Poll()
        {

            int availableBytes;
            while (true)
            {
                if (port.BytesToRead > 0)
                {
                    //Debug.Print("�GOT RX?");
                    availableBytes = port.BytesToRead;
                    port.Read(buffer, 0, availableBytes);
                    
                    if (receiver.Fill(buffer, availableBytes))
                    {
                        //Debug.Print("RECEIVED FULL PACKET");
                        //Debug.Print(receiver.packet.FormatString());
                        //received full xbee packet
                        if (receiver.packet.data[0] == data.ident)
                        {
                            //Debug.Print("PACKET IS INTERFACE PACKET");
                            data.FillData(receiver.packet.data);

                            //echo radio data
                            if (!interfacePacketReceived)
                            {
                                byte[] req = TransmitRequest(receiver.packet.src64Bytes, frame, receiver.packet.data);
                                port.Write(req, 0, req.Length);
                            }

                            interfacePacketReceived = true;

                        }
                        receiver.fullPacket = false;
                    }

                    packetReceived = true;
                }

                Thread.Sleep(10);
            }
            
        }

        public String BytesToString(byte[] bytes, int length)
        {
            String str = "";
            int i;
            for(i = 0; i < length; i++){
                str += bytes[i].ToString() + " ";
                bytes[i] = 0;
            }

            return str;
        }


        public byte[] TransmitRequest(byte[] addr, byte frameId, byte[] data)
        {
            int i;
            byte[] packet = new byte[data.Length + 18];
            packet[0] = 0x7E;
            packet[1] = 0x00;
            packet[2] = (byte)(data.Length + 14);
            packet[3] = 0x10;
            packet[4] = frameId;
            for (i = 5; i < 13; i++)
            {
                packet[i] = addr[i - 5];
            }
            packet[13] = 0xFF;
            packet[14] = 0xFE;
            packet[15] = 0x00;
            packet[16] = 0x00;
            for (i = 17; i < 17 + data.Length; i++)
            {
                packet[i] = data[i - 17];
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


    class InterfacePacket
    {


        public byte ident;
        public byte[] analog;
        public bool[] digital;
        public ushort digitalByteCount;

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
            //parse bytes into analog and digital arrays
            int i;
            for (i = 0; i < analog.Length; i++)
            {
                analog[i] = bytes[i + 1];
            }

            int bit = 0;
            for (i++; i < analog.Length + digitalByteCount + 1; i++)
            {
                //Debug.Print("DIGITAL BYTE:  " + (i - analog.Length -1));

                if ((bytes[i] & 0x01) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i] & 0x02) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i] & 0x04) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i] & 0x08) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i] & 0x10) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i] & 0x20) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i] & 0x40) > 0) digital[bit] = true;
                else digital[bit] = false;
                bit++;

                if ((bytes[i] & 0x80) > 0) digital[bit] = true;
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

    class XBeeReceiver
    {
        enum RXState { IDLE, LEN, COMMAND, SRC64, SRC16, OPTIONS, DATA, CHECKSUM, UNKNOWN, ERROR }
        enum XBeeCommands { RX = 0x90, TX = 0x8B }
        public int state;
        public byte[] buffer;
        public int bytesRead;
        public XBeeRXPacket packet;
        public Boolean fullPacket;

        public XBeeReceiver()
        {
            state = (int)RXState.IDLE;
            buffer = new byte[128];
        }

        public Boolean Fill(byte[] bytes, int len){
            byte b;
            int i;
            for (i = 0; i < len; i++)
            {
                b = bytes[i];

                switch (state)
                {
                    case (int)RXState.IDLE:
                        if (b == 0x7E)
                        {
                            packet = new XBeeRXPacket();
                            bytesRead = 0;
                            state++;
                        }
                        break;
                    case (int)RXState.LEN:
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
                            state++;
                        }
                        break;
                    case (int)RXState.COMMAND:
                        if (b == (byte)XBeeCommands.RX)
                        {
                            packet.command = b;
                            packet.data = new byte[packet.length - 12];
                            state++;
                        }
                        else
                        {
                            bytesRead = 0;
                            state = (int)RXState.UNKNOWN;
                        }
                        break;
                    case (int)RXState.SRC64:
                        //Read 64bit Sender's Address
                        packet.src64Bytes[bytesRead] = b;
                        bytesRead++;
                        if (bytesRead == 8)
                        {
                            bytesRead = 0;
                            state++;
                        }
                        break;

                    case (int)RXState.SRC16:
                        //Read 16bit Sender's Address
                        bytesRead++;
                        if (bytesRead == 2)
                        {
                            bytesRead = 0;
                            state++;
                        }
                        break;

                    case (int)RXState.OPTIONS:
                        packet.options = b;
                        state++;
                        break;

                    case (int)RXState.DATA:
                        //The beef
          				packet.data[bytesRead] = b;
				        bytesRead++;
				        if(bytesRead == packet.data.Length){
					        state++;
				        }
                        break;

                    case (int)RXState.CHECKSUM:
                        fullPacket = true;
                        state = (int)RXState.IDLE;
                        break;

                    case (int)RXState.UNKNOWN:
                        //ignore all bytes in packet before trying to parse again (all XBee API functions generate a confirmation packet sent as a packet)
                        //We dont care about most of them.
                        //We can get information about the current XBee module (like requesting its serial number or coordinator or network status)
                        bytesRead++;
                        if (bytesRead == packet.length)
                        {
                            state = (int)RXState.IDLE;
                            bytesRead = 0;
                        }
                        break;
                }
            }
            if (fullPacket) return true;
            return false;
        }
    }

    class XBeeRXPacket
    {
        public uint length;
        public byte command;
        public byte[] src64Bytes;
        public UInt64 src64;
        public UInt16 src16;
        public byte options;
        public byte[] data;
        public byte checksum;

        public XBeeRXPacket()
        {
            src64Bytes = new byte[8];
            return;
        }
        public XBeeRXPacket(uint aLength, byte aCommand, UInt64 aSrc64, UInt16 aSrc16, byte aOptions, byte[] aData, byte aChecksum)
        {
            length = aLength;
            command = aCommand;
            src64 = aSrc64;
            src16 = aSrc16;
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