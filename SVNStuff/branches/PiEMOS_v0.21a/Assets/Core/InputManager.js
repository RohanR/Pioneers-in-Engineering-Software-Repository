import System.IO;
import System.Text;
import System.Collections.Generic;

static var outputs : Hashtable;
var interfaceFilename : String = "interface.cfg";
var controls : List.<ControlClass>;
enum InputState{PLAYING, EDITING}
static var state : InputState;
var movingControl : Transform;

function OnEnable(){
	outputs = new Hashtable();
	ParseInterface(interfaceFilename);
}

enum InterfaceParseState{NONE, CONTROL, PACKET, TELEMETRY}
function ParseInterface(path : String){
	lines = File.ReadAllLines(Application.dataPath + "/" + path);
	controls = new List.<ControlClass>();
	
	var state : InterfaceParseState = InterfaceParseState.NONE;
	for(i = 0; i < lines.length; i++){
		if(lines[i].length > 0){
			switch(state){
				case InterfaceParseState.NONE:
					if(lines[i] == "[Control]"){
						//~ Debug.Log("CONTROL");
						state = InterfaceParseState.CONTROL;
					}
					
					if(lines[i] == "[Packet]"){
						state = InterfaceParseState.PACKET;
					}
					
					if(lines[i] == "[Telemetry]"){
						state = InterfaceParseState.TELEMETRY;
					}
				break;
				
				case InterfaceParseState.CONTROL:
					line = lines[i].Split("="[0]);
					prefab = Resources.Load("Controls/Prefabs/" + line[1]);
					obj = Instantiate(prefab);
					obj.transform.parent = transform;
					comp = obj.GetComponent(ControlClass);
					controls.Add(comp);
					comp.ParseConfig(lines, i+1);
					state = InterfaceParseState.NONE;
				break;
				
				case InterfaceParseState.PACKET:
					gameObject.GetComponent(PacketManager).ParsePacketSetup(lines, i);
					state = InterfaceParseState.NONE;
				break;
				
				case InterfaceParseState.TELEMETRY:
					gameObject.GetComponent(PacketManager).ParseTelemetrySetup(lines, i);
					state = InterfaceParseState.NONE;
				break;
			}
		}
	}
}

function FormatControls() : String{
	var sb : StringBuilder = new StringBuilder();
	for(i = 0; i < controls.Count; i++){
		sb.Append(controls[i].FormatConfig());
	}
	
	return sb.ToString();

}

function Update (){
	var ray : Ray;
	var hit : RaycastHit;
	if (state == InputState.PLAYING) {
		if(Input.GetKey("mouse 0")){

			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, hit, 1000)){
				hit.transform.SendMessageUpwards("MouseOn", hit, SendMessageOptions.DontRequireReceiver);
			}
		}
		if(Input.GetKeyUp("mouse 0")){
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, hit, 1000)){
				hit.transform.SendMessageUpwards("MouseUp", hit, SendMessageOptions.DontRequireReceiver);
			}
		}
	} else if (state == InputState.EDITING) {
		if (movingControl != null) {
			// FIGURE OUT HOW TO MOVE COMPONENTS TO THE RIGHT POSITION
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.Log(ray.direction);
			movingControl.position = ray.origin;
		}
		if(Input.GetKey("mouse 0")) {
			ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if(Physics.Raycast(ray, hit, 1000)){
				movingControl = hit.transform;			}
		}
		if(Input.GetKeyUp("mouse 0")) {
			movingControl = null;
		}
	}
}



static function AxisToByte(f : float){
	var b : byte = ((f + 1) / 2.0)	* 255;
	return b;
}

static function ByteToAxis(b : byte){
	var f : float = ((b / 255.0) * 2.0) - 1.0;
	return f;
}