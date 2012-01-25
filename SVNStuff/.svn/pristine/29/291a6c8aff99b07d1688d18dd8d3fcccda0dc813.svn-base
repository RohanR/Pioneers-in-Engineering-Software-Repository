class GAUGE extends ControlClass{}

//input names from cfg
var input : String;

//output bytes for packet
var output : byte;

//visual stuff
var rend : Renderer;
var label : TextMesh;
var vLabel : TextMesh;
var invert : boolean;
var linkedControl : boolean;
var mouseControl : boolean;
var lowPulse : Color;
var highPulse : Color;

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
	if(parseInt(lines[ln+3].Split("="[0])[1]) == 1) invert = true;
	label.text = name;
	
	InputManager.outputs.Add(name + ".output", output);
	
	if(input.Contains(".")){
		linkedControl = true;
	}
	
	if(input == "mouse"){
		mouseControl = true;
	}
	UpdateOutput(1.0);
}

/*
[Control]
type=HORIZONTAL_GAUGE
name=Gauge 1
position=0,264
input=telemetry.analog[1]
invert=0
*/

function FormatConfig() : String{
	var sb : StringBuilder = new StringBuilder();
	sb.AppendLine("[Control]");
	sb.AppendLine("type=" + prefabName);
	sb.AppendLine("name=" + name);
	sb.AppendLine("position=" + (transform.position.x * 100) + ","  + (transform.position.y * 100));
	sb.AppendLine("input=" + input);
	sb.AppendLine("invert=" + (invert ? 1 : 0));
	sb.AppendLine("");
	sb.AppendLine("");
	
	return sb.ToString();
}

function Update(){

	if(InputManager.outputs != null){
	if(!mouseControl){
		if(!linkedControl){
			v = Input.GetAxis(input);
			x = 1.0 - ((v + 1) / 2.0);
			output = InputManager.AxisToByte(v);
		}
		else{
			output = InputManager.outputs[input];
			x = 1.0 - (InputManager.ByteToAxis(output) + 1) / 2.0;
		}
		
		if(invert){
			x = 1.0 - x;
			output = 255 - output;
		}
		
		UpdateOutput(x);
	}
	else{
		
	}
	}
	//~ rend.material.color = Color.Lerp(lowPulse, highPulse, (Mathf.Sin(Time.time) + 1.0) / 2.0);
	
	
}

function MouseOn(hit : RaycastHit){
	//~ Debug.Log(hit.textureCoord);
	if(mouseControl){
		x = 1.0 - hit.textureCoord.x;
		output = InputManager.AxisToByte((hit.textureCoord.x * 2) - 1.0);
		x = 1.0 - ((InputManager.ByteToAxis(output) + 1.0) / 2.0);
		//~ x = (0.9375 * x) + 0.015625;
		UpdateOutput(x);
	}
}

function UpdateOutput(x : float){
	if(!mouseControl) rend.material.mainTextureOffset.x = Mathf.Lerp(rend.material.mainTextureOffset.x, x, 0.25);
	else rend.material.mainTextureOffset.x = x;
	InputManager.outputs[name + ".output"] = output;
	vLabel.text = ZeroPad(output);
}

function ZeroPad(i : int){
	str = "";
	if(i < 100) str = str + "0";
	if(i < 10) str = str + "0";
	str = str + i;
	return str;
}