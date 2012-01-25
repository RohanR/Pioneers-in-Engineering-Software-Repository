import System.Text;

enum PacketType{RAW, XBEE, UDP}
var inputManager : InputManager;
var serialManager : SerialManager;
var fieldManager : FieldManager;

var packetType : PacketType;
var packetLength : int;

var packet : byte[];
var telemetry : byte[];
var telemAnalogBytes : int;
var telemDigitalBytes : int;

var telemetryFill : int;

var links : Array;

function ParsePacketSetup(lines : String[], i){
	serialManager = gameObject.GetComponent(SerialManager);
	fieldManager = gameObject.GetComponent(FieldManager);
	
	links = new Array();
	finished = false;
	
	while(!finished){
		if(i < lines.length){
			ln = lines[i].Split("="[0]);
			//~ if(ln.length > 0){
				//~ try{
					//~ Debug.Log(parseInt(ln[0]));
				//~ }
				//~ catch(err){
					//~ Debug.Log("Not integer");
				//~ }
			//~ }
			
			if(ln[0].length > 0){
				switch(ln[0]){
					case "type":
						switch(ln[1]){
							case "raw":
								packetType = PacketType.RAW;
							break;
							
							case "xbee":
								packetType = PacketType.XBEE;
							break;
							
							case "udp":
								packetType = PacketType.UDP;
							break;
						}
					break;
					
					case "length":
						packetLength = parseInt(ln[1]) + 5;
						packet = new byte[packetLength];
					break;
					
					case "DH":
						serialManager.DHStr = ln[1];
					break;
					
					case "DL":
						serialManager.DLStr = ln[1];
					break;
					
					default:
						//~ ParseLink(ln);
						if(ln[0].Contains(":")){
							//bit field link
							field = ln[0].Split(":"[0]);
							byteNum = parseInt(field[0]) + 5;
							bit = parseInt(field[1]);
							if(ln[1][0] == "%"){
								//static value
								Debug.Log("Static not supported for bitfield");
							}
							else{
								//control link
								links.Add(new ValueLink(inputManager.outputs, ln[1], bit, byteNum));
							}
						}
						else{
							//full byte link
							byteNum = parseInt(ln[0]) + 5;
							if(ln[1][0] == "%"){
								//static value
								ln[1] = ln[1].Trim("%"[0]);
								packet[byteNum] = parseInt(ln[1]);
								
							}
							else{
								//control link
								links.Add(new ValueLink(inputManager.outputs, ln[1], -1, byteNum));
							}
						}
					break;
				}
			}
			
			if(lines[i].length == 0) finished = true;
			i++;
			
		}
		else{
			finished = true;
		}
	}
	
	fieldManager.Setup();
}

/*
[Packet]
type=xbee
DH=13A200
DL=404B4BFE
length=9
0=Left Stick.x_output
1=Left Stick.y_output
2=Right Stick.x_output
3=Right Stick.y_output
5=Gauge 2.output
6=Gauge 3.output
7:0=Button 4.output
7:1=Toggle 2.output
7:2=Toggle 3.output
7:7=Button 4.output
*/

function FormatPacketSetup() : String{
	var sb : StringBuilder = new StringBuilder();
	sb.AppendLine("[Packet]");
	sb.AppendLine("type=xbee");
	sb.AppendLine("DH=" + serialManager.DHStr);
	sb.AppendLine("DL=" + serialManager.DLStr);
	sb.AppendLine("length=" + (packetLength - 5));
	
	for(i = 0; i < links.length-4; i++){
		sb.AppendLine(links[i].FormatLink(-5));
	}
	
	sb.AppendLine("");
	sb.AppendLine("");
	
	return sb.ToString();
}

function ParseTelemetrySetup(lines : String[], i){
	serialManager = gameObject.GetComponent(SerialManager);
	inputManager = gameObject.GetComponent(InputManager);
	finished = false;
	
	while(!finished){
		ln = lines[i].Split("="[0]);
		if(ln[0] == "analog"){
			an = parseInt(ln[1]);
			telemAnalogBytes = an;
			for(a = 0; a < an; a++){
				inputManager.outputs.Add("telemetry.analog[" + a + "]", 0);
			}
		}
		
		if(ln[0] == "digital"){
			di = parseInt(ln[1]) * 8;
			telemDigitalBytes = parseInt(ln[1]);
			for(d = 0; d < di; d++){
				inputManager.outputs.Add("telemetry.digital[" + d + "]", false);
			}
		}
		
		if(lines[i].length == 0){
			telemetry = new byte[telemAnalogBytes + telemDigitalBytes + 1];
			finished = true;
		}
		i++;
	}
}

function FormatTelemetrySetup() : String{
	var sb : StringBuilder = new StringBuilder();
	sb.AppendLine("[Telemetry]");
	sb.AppendLine("analog=" + telemAnalogBytes);
	sb.AppendLine("digital=" + telemDigitalBytes);
	sb.AppendLine("");
	sb.AppendLine("");
	sb.AppendLine("");
	
	return sb.ToString();
}

var debugPacket : boolean;
var telemetryInit : boolean;
var pinged : boolean = false; //whether or not the computer is waiting for a ping from the robot
var latency : float = 0;

function FillTelemetry(bArr : byte[]){
	for(i = 0; i < bArr.length; i++){
		if(!telemetryInit){
			if(bArr[i] == 253){ //253 is normal telemetrypacket ident

				telemetry[0] = 253;
				telemetryInit = true;
				telemetryFill = 1;
			}
			else if(bArr[i] == 251){ //251 is pongpacket ident (received return after pinging robot)
				telemetry[0] = 251;
				telemetryInit = true;
				telemetryFill = 1;
				if(pinged == true){ //if the computer is waiting for a response when it receives the response
					latency = Time.time - timeSent; //record the latency
					pinged = false; //the computer is no longer waiting for a response
				}
			}
		}
		else{
			telemetry[telemetryFill] = bArr[i];
			telemetryFill++;	
			if(telemetryFill == telemetry.length){
				telemetryInit = false;
				
				//update telemetry
				
				for(i = 0; i < telemAnalogBytes; i++){
					inputManager.outputs["telemetry.analog[" + i + "]"] = telemetry[i+1];
				}
				
				b = 0;
				for(i = i+0; i < telemetry.length-1; i++){
					inputManager.outputs["telemetry.digital[" + b + "]"] = (telemetry[i+1] & 0x01) > 0;
					b++;
					inputManager.outputs["telemetry.digital[" + b + "]"] = (telemetry[i+1] & 0x02) > 0;
					b++;
					inputManager.outputs["telemetry.digital[" + b + "]"] = (telemetry[i+1] & 0x04) > 0;
					b++;
					inputManager.outputs["telemetry.digital[" + b + "]"] = (telemetry[i+1] & 0x08) > 0;
					b++;
					inputManager.outputs["telemetry.digital[" + b + "]"] = (telemetry[i+1] & 0x10) > 0;
					b++;
					inputManager.outputs["telemetry.digital[" + b + "]"] = (telemetry[i+1] & 0x20) > 0;
					b++;
					inputManager.outputs["telemetry.digital[" + b + "]"] = (telemetry[i+1] & 0x40) > 0;
					b++;
					inputManager.outputs["telemetry.digital[" + b + "]"] = (telemetry[i+1] & 0x80) > 0;
					b++;
				}
			}
		}
		//~ if(echoFill == packetLength) echoFill = 0;
	}
	

	

}

var printInit : boolean;

function FillPrint(bArr : byte[]){
	for(i = 0; i < bArr.length; i++){
		if(!printInit){
			if(bArr[i] == 252){
				printInit = true;
				printString = "";
			}
		}
		else{
			
			if(bArr[i] == 0){
				printInit = false;
				
			}
			else{
				var chr : char = parseInt(bArr[i]);
				printString = printString + chr;
				printRenderer.material.color.a = 1.0;
				printTextMesh.text = printString;
			}
		}
	}
}

var printString : String;
var printRenderer : Renderer;
var printTextMesh : TextMesh;
var timeSent : float;


function OnGUI(){
		
	if(debugPacket){
		for(i = 0; i < packet.length; i++){
			GUILayout.BeginHorizontal();
				GUILayout.Label(packet[i].ToString(), GUILayout.Width(64));
				if(i < telemetry.length){
					GUILayout.Label(telemetry[i].ToString(), GUILayout.Width(64));
					if(i > telemAnalogBytes){
						offset = ((i-telemAnalogBytes-1)*8);
						for(b = 0 + offset; b < 8  + offset; b++){
							GUILayout.Label(InputManager.outputs["telemetry.digital[" + b + "]"].ToString());
						}
					}
				}
			GUILayout.EndHorizontal();
		}
		
		if(serialManager.state == SerialState.CONNECTED){
			if(GUILayout.Button("Ping " + latency*1000 + " ms")){ //makes a pinging button
				RobotPing();
			}
		}
	}
}

var pingpacket : byte[];
function RobotPing(){ //copies packet, changes ident and sends it
	if(serialManager.state == SerialState.CONNECTED){
		pingpacket = serialManager.CopyByteArray(packet);
		pingpacket[0] = 252;
		pingreq = XBee.TransmitRequest(serialManager.address, serialManager.transmitFrame, pingpacket);
		serialManager.port.Write(pingreq, 0, pingreq.length);
		serialManager.transmitFrame++;
		pinged = true;
		timeSent = Time.time;
	}
}
function Update(){
	printRenderer.material.color.a -= 0.5 * Time.deltaTime;
	if(Input.GetKeyUp("q")){
		debugPacket = !debugPacket;
	}
	
	for(var link : ValueLink in links){
		packet[link.byteNum] = 0;
	}
	
	for(var link : ValueLink in links){
		packet[link.byteNum] += link.UpdateLink();
	}
	if(pinged == true){ //if the computer is waiting for a ping
		if(Time.time - timeSent >= 5){ //and its been 5 seconds since it pinged the robot
			RobotPing(); //ping the robot again
		}
	}
}

class ValueLink{
	var outputs : Hashtable;
	var outputName : String;
	var bit : int = -1;
	var byteNum : int = -1;
	function ValueLink(table : Hashtable, aName : String, b : int, bn : int){
		outputs = table;
		outputName = aName;
		bit = b;
		byteNum = bn;
	}
	function UpdateLink(){
		if(bit == -1){
			return outputs[outputName];
		}
		else if(bit < 8 && bit > -1){
			//~ Debug.Log(outputs[outputName]);
			if(outputs[outputName] == true){
				//~ Debug.Log(outputName + "    " + Mathf.Pow(2, bit));
				return Mathf.Pow(2, bit);
			}
			return 0;
		}
	}
	
	function FormatLink(byteOffset : int) : String{
		//1=Left Stick.y_output
		//7:1=Toggle 2.output
		if(bit > -1){
			//bit specific form
			return (byteNum + byteOffset).ToString() + ":" + bit + "=" + outputName;
		}
		else{
			//full byte form
			return (byteNum + byteOffset).ToString() + "=" + outputName;
		}
		
	
	}
}

//~ class TelemetryLink{
	//~ var outputs : Hashtable;
	//~ var outputName : String;
	//~ var bit : int = -1;
	//~ var byteNum : int = -1;
	
	//~ function TelemetryLink(table : Hashtable, aName : String, b : int, bn : int){
		//~ outputs = table;
		//~ outputName = aName;
		//~ bit = b;
		//~ byteNum = bn;
	//~ }
//~ }