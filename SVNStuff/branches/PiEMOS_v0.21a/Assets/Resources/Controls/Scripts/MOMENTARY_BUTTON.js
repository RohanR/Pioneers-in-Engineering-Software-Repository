class MOMENTARY_BUTTON extends ControlClass{}

//input names from cfg
var input : String;

//output bytes for packet
var output : boolean;

//visual stuff
var rend : Renderer;
var label : TextMesh;
var openTexture : Texture2D;
var closedTexture : Texture2D;
var beep : AudioClip;
var mouseOn : boolean;
var linkedControl : boolean;

function ParseConfig(lines : String[], ln : int){
	//name
	name = lines[ln+0].Split("="[0])[1];
	
	//position
	line = lines[ln+1].Split("="[0])[1];
	str = line.Split(","[0]);
	x = parseFloat(str[0]) * 0.01;
	y = parseFloat(str[1]) * 0.01;
	transform.position = Vector3(x,y,0);
	
	//inputs
	input = lines[ln+2].Split("="[0])[1];
	if(input.Contains(".")){
		linkedControl = true;
	}
	label.text = name;
	
	InputManager.outputs.Add(name + ".output", output);
}

/*
[Control]
type=MOMENTARY_BUTTON
name=Button 1
position=-400,-219
input=telemetry.digital[0]
*/

function FormatConfig() : String{
	var sb : StringBuilder = new StringBuilder();
	sb.AppendLine("[Control]");
	sb.AppendLine("type=" + prefabName);
	sb.AppendLine("name=" + name);
	sb.AppendLine("position=" + (transform.position.x * 100) + ","  + (transform.position.y * 100));
	sb.AppendLine("input=" + input);
	sb.AppendLine("");
	sb.AppendLine("");
	
	return sb.ToString();
}

function MouseOn(hit : RaycastHit){
	if(!linkedControl){
		if(!output) audio.PlayOneShot(beep);
		mouseOn = true;
		output = true;
		rend.material.mainTexture = closedTexture;
	}
}

function Update(){
	
	if(!linkedControl){
		if(Input.GetKeyDown(input)){
			output = true;
			rend.material.mainTexture = closedTexture;
		}
		else if(Input.GetKeyUp(input)){
			output = false;
			rend.material.mainTexture = openTexture;
		}
		
		if(Input.GetKeyDown(input)){
			audio.PlayOneShot(beep);
		}
	}
	else{
		oldOutput = output;
		output = InputManager.outputs[input];
		if(output){
			if(!oldOutput) audio.PlayOneShot(beep);
			rend.material.mainTexture = closedTexture;
		}
		else{
			rend.material.mainTexture = openTexture;
		}
	}
	
	if(mouseOn){
		if(!Input.GetKey("mouse 0")){
			output = false;
			rend.material.mainTexture = openTexture;
			mouseOn = false;
		}
	}
	
	UpdateOutput();
	
}

function UpdateOutput(){
	InputManager.outputs[name + ".output"] = output;
}