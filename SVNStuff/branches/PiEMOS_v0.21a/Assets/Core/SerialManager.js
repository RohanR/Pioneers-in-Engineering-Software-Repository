import System.IO.Ports;
import System.IO;
import System.BitConverter;
enum SerialState {NONE, PORTSELECT, CONNECTING, CONNECTED, ACTIVE, PAUSED, ERROR}

var state : SerialState;
var port : SerialPort;
var ports : String[];
var baudrate : int;
var parity : Parity;
var dataBits : int;
var stopBits : StopBits;

var transmitRate : float;
var keepAliveRate : float;
private var nextTransmissionTime : float;
private var nextKeepAliveTime : float;

var packetManager : PacketManager;
var DHStr : String;
var DLStr : String;
var address : XBee64bitAddress;
var transmitFrame : byte;

var serialStream : Stream;

var oldData : byte[];

function Start(){
	//~ port = new SerialPort();
	ports = SerialPort.GetPortNames();	
	state = SerialState.PORTSELECT;
	packetManager = gameObject.GetComponent(PacketManager);
	
	config = File.ReadAllText(Application.dataPath + "/serial.pref");
	
	entries = config.Split(","[0]);
	baudrate = parseInt(entries [0]);
	parity = parseInt(entries [1]);
	dataBits = parseInt(entries [2]);
	stopBits = parseInt(entries [3]);
	//~ address = XBeeAddress(DHStr, DLStr);
	
	
	//~ serialStream = new Stream();
	//~ Debug.Log(BytesToString(test.bytes, test.bytes.length));
	//~ Debug.Log(BytesToString(XBee.ReverseBytes(XBee.HexStringToBytes(DHStr)), 4));
}



function OnGUI(){

	GUILayout.BeginVertical(GUILayout.Width(128));
	switch(state){
		case SerialState.PORTSELECT:
			GUILayout.Label("Select Port");
			for(str in ports){
				if(GUILayout.Button(str)){
					ConnectToPort(str);
				}
			}
		break;
		
		case SerialState.CONNECTING:
			GUILayout.Label("Port Settings");
			GUILayout.Label("Baud Rate: " + baudrate);
			
			if(GUILayout.Button("Open")){
				address = XBee64bitAddress(DHStr, DLStr);
				port.Open();
				port.DiscardOutBuffer();
				port.DiscardInBuffer();
			}
			if(port.IsOpen){
				Poller();
				state = SerialState.CONNECTED;
			}
		break;
		
		case SerialState.CONNECTED:
			if(Time.time >= nextTransmissionTime){
				if(Different(packetManager.packet, oldData) || Time.time > nextKeepAliveTime){
					oldData = CopyByteArray(packetManager.packet);
					req = XBee.TransmitRequest(address, transmitFrame, packetManager.packet);
					port.Write(req, 0, req.length);
					transmitFrame++;
					//transmitFrame = 0;
					if(transmitFrame > 125) transmitFrame = 0;
					nextTransmissionTime = Time.time + transmitRate;
					nextKeepAliveTime = Time.time + keepAliveRate;
				}
			}
			//~ if(GUILayout.Button("Send Packet")){
					//~ req = XBee.TransmitRequest(address, transmitFrame, packetManager.packet);
					//~ port.Write(req, 0, req.length);
			//~ }
			//~ if(GUILayout.Button("Begin Polling")){
				//~ Poller();
			//~ }
		break;
	}
	GUILayout.EndVertical();
}

function CopyByteArray(arr : byte[]){
	var result : byte[] = new byte[arr.length];
	
	for(i = 0; i < arr.length; i++){
		result[i] = arr[i];
	}
	
	return result;
}

function Different(arr1 : byte[], arr2 : byte[]){
	if(arr1.length != arr2.length) return true;
	for(i = 0; i < arr1.length; i++){
		if(arr1[i] != arr2[i]) return true;
	}
	return false;
}

function ConnectToPort(str : String){
	if(str.Length > 4)	str = "\\\\.\\" + str;
	
	port = new SerialPort(str, baudrate,parity,dataBits,stopBits);
	port.ReadTimeout = 2;
	port.WriteTimeout = 2;

	state = SerialState.CONNECTING;

}



//~ var packetType : XBeePacketIdentifier;
//~ var readArray : byte[] = new byte[256];
//~ var packetLen : System.UInt16;
//~ var packetPosition : System.UInt16;

function Poller(){
	while(true){
		//~ Debug.Log("Polling");
		bArr = Poll();
		
		if(bArr.length > 0){
			//~ Debug.Log("Parsing");
			//~ Debug.Log(BytesToString(bArr, bArr.length));
			for(i = 0; i < bArr.length; i++){
				
				rx = XBee.ParsePacket(bArr[i]);
				if(rx){
					//~ Debug.Log("Got packet");
					//~ Debug.Log(BytesToString(XBee.packet.data, XBee.packet.data.Length));
					packetManager.FillTelemetry(XBee.packet.data);
					packetManager.FillPrint(XBee.packet.data);
					//~ Debug.Log("Complete packet returned");
				}
			}
		}
		//~ Debug.Log(BytesToString(bArr, bArr.length));
		yield WaitForSeconds(0.001);
	}
}

function Poll(){
	var b : Stream = port.BaseStream;
	str = "";
	
	var arr : Array = new Array();
	while(true){
		try{
			aByte = b.ReadByte();
			str += aByte.ToString() + " ";
			arr.Push(aByte);
		}
		catch(err){
			if(XBee.packet != null){
				//~ Debug.Log(port.ReadByte());
				//~ Debug.Log("NB " + BytesToString(XBee.packet.data, XBee.packet.data.length));
			}
			break;
		}
	}
	
	//~ Debug.Log(str);
	
	var byteArray : byte[] = new byte[arr.length];
	for(i = 0; i < byteArray.length; i++){
		byteArray[i] = arr[i];
	}
	return byteArray;
}


static function StringToBytes(str : String){
	var bytes : byte[] = new byte[str.length];
	for(i = 0; i < bytes.length; i++){
		bytes[i] = GetBytes(str.Chars[i])[0];
		Debug.Log(bytes[i]);
	}
	return bytes;
}

static function BytesToString(bytes : byte[], leng : int){
	str = "";
	for(i = 0; i < leng; i++){
		str += bytes[i].ToString() + " ";
		//~ bytes[i] = 0;
	}
	return str;
}

function OnDisable(){
	if(port){
		port.Close();
	}
}
