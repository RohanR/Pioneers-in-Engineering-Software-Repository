class TOGGLE_BUTTON extends ControlClass{}

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
	
	label.text = name;
	
	InputManager.outputs.Add(name + ".output", output);
}

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

function MouseUp(hit : RaycastHit){
	audio.PlayOneShot(beep);
	output = !output;
	UpdateTexture();
}

function UpdateTexture(){
	if(output){
		rend.material.mainTexture = closedTexture;
	}
	else{
		rend.material.mainTexture = openTexture;
	}
}

function Update(){
	if(input != "mouse"){
		if(Input.GetKeyUp(input)){
			audio.PlayOneShot(beep);
			output = !output;
			if(output){
				rend.material.mainTexture = closedTexture;
			}
			else{
				rend.material.mainTexture = openTexture;
			}
		}
	}
	
	UpdateOutput();
}

function UpdateOutput(){
	InputManager.outputs[name + ".output"] = output;
	UpdateTexture();
}