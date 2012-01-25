using UnityEngine;
using System.Collections;
using System.IO.Ports;
using System;
//using System.BitConverter;


public class XBee{
	public enum PacketIdentifier{ NONE = 0, PACKET_RX = 0x80, PACKET_TX = 0x8B}
	public enum RXState{IDLE, LEN_UPPER, LEN_LOWER, COMMAND, SRC64, RSSI, OPTIONS, DATA, CHECKSUM, UNKNOWN, ERROR}
	
	public static UInt16 bytesRead;
	public static int state;
	public static RadioPacket packet;
	public static PacketIdentifier packetType;
	
	public static byte[] TransmitRequest(XBee64bitAddress addr, byte frameId, byte[] data){
		int i;
		byte[] packet = new byte[data.Length + 15];
		packet[0] = 0x7E;
		packet[1] = 0x00;
		packet[2] = (byte)(data.Length + 11);
		packet[3] = 0x00;
		packet[4] = frameId;
		for(i = 5; i < 13; i++){
			packet[i] = addr.bytes[i-5];
		}
		//~ packet[13] = 0xFF;
		//~ packet[14] = 0xFE;
		packet[13] = 0x00;
		//~ packet[16] = 0x00;
		for(i = 14; i < 14+data.Length; i++){
				packet[i] = data[i-14];
		}
		
		byte checksum = 0;
		for(i = 3; i < packet.Length-1; i++){
			checksum += packet[i];
		}
		
		checksum = (byte)(0xFF - checksum);
		packet[packet.Length-1] = checksum;
		return packet;
	}
	
	public static bool ParsePacket(byte b){
		bool fullPacket = false;
		
		switch(state){
			case (int)RXState.IDLE:
				if(b == 0x7E){
					packet = new RadioPacket();
					state++;
				}
			break;
			
			case (int)RXState.LEN_UPPER:
				packet.length = (ushort)(b << 8);
				state++;
			break;
			
			case (int)RXState.LEN_LOWER:
				packet.length |= b;
				//~ Debug.Log(packet.length);
				state++;
			break;
			
			case (int)RXState.COMMAND:
				packet.command = b;
				if(b == (byte)PacketIdentifier.PACKET_RX){
					//~ Debug.Log("Identified as Packet RX");
					packet.data = new byte[packet.length-11];
					//~ Debug.Log("SETTING DATA SIZE: " + (packet.length-11));
					bytesRead = 0;
					state++;
				}
				else{
					bytesRead = 0;
					//~ Debug.Log("UNKNOWN PACKET TYPE");
					state = (int)RXState.UNKNOWN;
				}

			break;
				
			case (int)RXState.SRC64:
				packet.srcAddr.bytes[bytesRead] = b;
				bytesRead++;
				if(bytesRead == 8){
					bytesRead = 0;
					state++;
				}
			break;
			
			//~ case (int)RXState.SRC16:
				//~ bytesRead++;
				//~ if(bytesRead == 2){
					//~ bytesRead = 0;
					//~ state++;
				//~ }
			//~ break;
				
			case (int)RXState.RSSI:
				packet.rssi = b;
				state++;
			break;
				
			case (int)RXState.OPTIONS:
				packet.options = b;
				
				state++;
			break;
			
			case (int)RXState.DATA:
				packet.data[bytesRead] = b;
				bytesRead++;
				if(bytesRead == packet.data.Length){
					state++;
				}
			break;
				
			case (int)RXState.CHECKSUM:
				fullPacket = true;
//				Debug.Log(packet.data[0]);
				//Debug.Log("OPTIONS :: " + packet.options);
				state = (int)RXState.IDLE;
			break;
			
			case (int)RXState.UNKNOWN:
				bytesRead++;
				if(bytesRead == packet.length){
					state = (int)RXState.IDLE;
					bytesRead = 0;
				}
			break;
		}
		if(fullPacket) return true;
		return false;
	}
	
	
	
	public static byte[] ReverseBytes(byte[] bytes){
		byte[] reversed = new byte[bytes.Length];
		int i;
		for(i = 0; i < bytes.Length; i++){
			reversed[bytes.Length-1-i] = bytes[i];
		}
		return reversed;
	}
	
	public static byte[] HexStringToBytes (string str){
		return System.BitConverter.GetBytes(System.UInt64.Parse(str, System.Globalization.NumberStyles.AllowHexSpecifier));
	}
	

}

public class RadioPacket{
	public UInt16 length;
	public byte command;
	public XBee64bitAddress srcAddr;
	public byte rssi;
	public byte options;
	public byte[] data;
	
	public RadioPacket(){
		srcAddr = new XBee64bitAddress();
	}
}

public class XBee64bitAddress{
	public byte[] bytes;

	public XBee64bitAddress(){
		bytes = new byte[8];
	}
	
	public XBee64bitAddress(string DHStr, string DLStr){
		bytes = XBee.ReverseBytes(XBee.HexStringToBytes(DHStr + DLStr));
	}
}

//~ public class XBee16bitAddress{
	
		//~ public XBee16bitAddress(){
		//~ }
		//~ bytes = XBee.ReverseBytes(XBee.HexStringToBytes(MY));
//~ }