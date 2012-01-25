//enum _MatchState{IDLE, PRACTICE, 

var packetManager : PacketManager;

var fieldTime : float;
private var fieldStartTime : float;
var matchDuration : float;
var disabled : boolean;
var autonomous : boolean;
var team : boolean;

var disableToggle : TOGGLE_BUTTON;
var autonomousToggle : TOGGLE_BUTTON;
var teamToggle : TOGGLE_BUTTON;

private var matchStarted : boolean;
//private var practiceMode : boolean;

function Start(){


}

function Update(){
	if(matchStarted){
		fieldTime = Mathf.Clamp((Time.time - fieldStartTime), 0.0,  255.0);
		if(fieldTime > matchDuration){
			disableToggle.output = false;
			autonomousToggle.output = false;
			matchStarted = false;
		}
	}
	
	var timeInt : int = fieldTime;
	var timeByte : byte = timeInt;
	
	InputManager.outputs["field.time.output"] = timeByte;
	//~ InputManager.outputs["field.disabled"] = disabled;
	//~ InputManager.outputs["field.autonomous"] = autonomous;
	//~ InputManager.outputs["field.team"] = team;
	
}

function Setup(){
	//packet header hardcoded
	packetManager.packet[0] = 254;
	var b : byte = 0;
	InputManager.outputs.Add("field.time.output", b);
	InputManager.outputs.Add("field.disabled.output", false);
	InputManager.outputs.Add("field.autonomous.output", false);
	InputManager.outputs.Add("field.team.output", false);
	
	packetManager.links.Add(new ValueLink(InputManager.outputs, "field.time.output", -1, 1));
	packetManager.links.Add(new ValueLink(InputManager.outputs, "field.disabled.output", 0, 2));
	packetManager.links.Add(new ValueLink(InputManager.outputs, "field.autonomous.output", 1, 2));
	packetManager.links.Add(new ValueLink(InputManager.outputs, "field.team.output", 2, 2));
	
	
	//load toggles
	prefab = Resources.Load("Controls/Prefabs/" + "TOGGLE_BUTTON");
	obj = Instantiate(prefab);
	obj.name = "field.disabled";
	obj.transform.parent = transform;
	disableToggle = obj.GetComponent(TOGGLE_BUTTON);
	comp = obj.GetComponent(ControlClass);
	comp.input = "escape";
	comp.label.text = "Enabled";
	obj.transform.localPosition = Vector3(2.662123, -2.1, 0);
	
	prefab = Resources.Load("Controls/Prefabs/" + "TOGGLE_BUTTON");
	obj = Instantiate(prefab);
	obj.name = "field.autonomous";
	obj.transform.parent = transform;
	autonomousToggle = obj.GetComponent(TOGGLE_BUTTON);
	comp = obj.GetComponent(ControlClass);
	comp.input = "tab";
	comp.label.text = "Autonomous";
	obj.transform.localPosition = Vector3(2.662123, -2.7, 0);
	
	
	prefab = Resources.Load("Controls/Prefabs/" + "TEAM_TOGGLE_BUTTON");
	obj = Instantiate(prefab);
	obj.name = "field.team";
	obj.transform.parent = transform;
	teamToggle = obj.GetComponent(TOGGLE_BUTTON);
	comp = obj.GetComponent(ControlClass);
	comp.input = "mouse";
	comp.label.text = "Team Color";
	obj.transform.localPosition = Vector3(2.662123, -3.3, 0);
	


	//1		field time
	//2	bitfield
	//2:0	disabled
	//2:1	autonomous
	//2:2	team
	//2:3	???
	
	//3	team1 score
	//4	team2 score
}

function OnGUI(){
	GUILayout.BeginArea(Rect(Screen.width-256,0,256,256));
		GUILayout.Label("Time: " + (matchDuration - fieldTime).ToString());
		//~ if(disabled)
			//~ GUILayout.Label("Disabled");
		//~ else
			//~ GUILayout.Label("Enabled");
		
		//~ if(autonomous)
			//~ GUILayout.Label("Autonomous");
		//~ else
			//~ GUILayout.Label("Tele-operated");
			
		//~ if(team)
			//~ GUILayout.Label("Gold Team");
		//~ else
			//~ GUILayout.Label("Blue Team");
		
		if(!matchStarted){
			if(GUILayout.Button("Start Match")){
				matchStarted = true;
				disableToggle.output = true;
				fieldStartTime = Time.time;
			}
		}
		else{
			if(GUILayout.Button("Stop Match")){
				matchStarted = false;
			}
		}
	GUILayout.EndArea();
}