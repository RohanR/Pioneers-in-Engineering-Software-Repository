import System.Text;
import System.IO;
import System.Collections.Generic;

var serialManager : SerialManager;
var packetManager : PacketManager;
var inputManager : InputManager;
var fieldManager : FieldManager;

enum InterfaceState{STARTUP, EDITING, LOADED}
static var state : InterfaceState;

var fileList : List.<FileInfo>;

function OnEnable(){
	serialManager = GetComponent("SerialManager");
	packetManager = GetComponent("PacketManager");
	inputManager = GetComponent("InputManager");
	fieldManager = GetComponent("FieldManager");
	
	var di : DirectoryInfo = new DirectoryInfo(Application.dataPath);

	var temp : FileInfo[] = di.GetFiles("*.cfg");
	fileList = new List.<FileInfo>(temp);
	
	for(i = 0; i < fileList.Count; i++){
		if(fileList[i].Name == "default.cfg"){
			fileList.RemoveAt(i);
			i--;
		}
	}
}



function OnGUI(){
	if(state == InterfaceState.STARTUP){
		GUILayout.BeginArea(Rect(256,0,256,256));
			if(GUILayout.Button("Load Default Interface")){
				inputManager.interfaceFilename = "default.cfg";
				Startup();
			}
			
			for(var info : FileInfo in fileList){
				if(GUILayout.Button(info.Name)){
					inputManager.interfaceFilename = info.Name;
					Startup();
				}
			}
		GUILayout.EndArea();
	}
	else if(state == InterfaceState.LOADED){
		GUILayout.BeginArea(Rect(256,0,256,256));
			if (GUILayout.Button("Edit Interface")) {
				EditInterface();
			}
		GUILayout.EndArea();
	}
	else if(state == InterfaceState.EDITING) {
		GUILayout.BeginArea(Rect(256,0,256,256));
			if(GUILayout.Button("Save Interface")){
				SaveInterface();
			}
			if(GUILayout.Button("Reset Changes")){
				ResetInterface();
			}
		GUILayout.EndArea();
	}
}

function Startup(){
	packetManager.enabled = true;
	fieldManager.enabled = true;
	serialManager.enabled = true;
	inputManager.enabled = true;
	state = InterfaceState.LOADED;
}

function SaveInterface(){
	var sb : StringBuilder = new StringBuilder();
	
	sb.Append(inputManager.FormatControls());
	sb.Append(packetManager.FormatPacketSetup());
	sb.Append(packetManager.FormatTelemetrySetup());
	
	File.WriteAllText(Application.dataPath + "/" + inputManager.interfaceFilename, sb.ToString());
	inputManager.state = InputState.PLAYING;
	state = InterfaceState.LOADED;
}

function EditInterface() {
	inputManager.state = InputState.EDITING;
	state = InterfaceState.EDITING;
}

function ResetInterface() {
	inputManager.state = InputState.PLAYING;
	state = InterfaceState.LOADED;
	inputManager.outputs = new Hashtable();
	inputManager.ParseInterface(inputManager.interfaceFilename);
}