import System.Text;

class DUAL_AXIS extends ControlClass{}

//input names from cfg
var x_input : String;
var y_input : String;

//output bytes for packet
var x_output : byte;
var y_output : byte;

//visual stuff
var xRange : float;
var yRange : float;
var reticle : GameObject;
var label : TextMesh;
var xLabel : TextMesh;
var yLabel : TextMesh;

var xInvert : boolean;
var yInvert : boolean;
var xLinked : boolean;
var yLinked : boolean;
var mouseControl : boolean;

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
	x_input = lines[ln+2].Split("="[0])[1];
	y_input = lines[ln+3].Split("="[0])[1];
	if(parseInt(lines[ln+4].Split("="[0])[1]) == 1) xInvert = true;
	if(parseInt(lines[ln+5].Split("="[0])[1]) == 1) yInvert = true;
	
	if(xInvert) xRange *= -1;
	if(yInvert) yRange *= -1;
	label.text = name;
	
	InputManager.outputs.Add(name + ".x_output", x_output);
	InputManager.outputs.Add(name + ".y_output", y_output);
	
	if(x_input.Contains(".")) xLinked = true;
	if(y_input.Contains(".")) yLinked = true;
	
	if(x_input == "mouse" || y_input == "mouse"){
		mouseControl = true;
		x_output = 127;
		y_output = 127;
		UpdateOutput();
	}
}

function FormatConfig() : String{
	var sb : StringBuilder = new StringBuilder();
	sb.AppendLine("[Control]");
	sb.AppendLine("type=" + prefabName);
	sb.AppendLine("name=" + name);
	sb.AppendLine("position=" + (transform.position.x * 100) + ","  + (transform.position.y * 100));
	sb.AppendLine("x_input=" + x_input);
	sb.AppendLine("y_input=" + y_input);
	sb.AppendLine("x_invert=" + (xInvert ? 1 : 0));
	sb.AppendLine("y_invert=" + (yInvert ? 1 : 0));
	sb.AppendLine("");
	sb.AppendLine("");
	
	return sb.ToString();
}
/*
[Control]
type=DUAL_AXIS
name=Right Stick
position=320,0
x_input=joystick 1 axis 2
y_input=joystick 1 axis 3
x_invert=0
y_invert=1

*/

function Update(){

	if(!mouseControl){
		if(!xLinked){
			x = Input.GetAxis(x_input);
		}
		else{
			x = InputManager.ByteToAxis(InputManager.outputs[x_input]);
		}
		if(!yLinked){
			y = Input.GetAxis(y_input);
		}
		else{
			y = InputManager.ByteToAxis(InputManager.outputs[y_input]);
		}
		reticle.transform.localPosition.x = x * xRange;
		reticle.transform.localPosition.y = y * yRange;
		x_output = InputManager.AxisToByte(x);
		y_output = 255 - InputManager.AxisToByte(y);	

		UpdateOutput();
		//~ xLabel.text = ZeroPad(x_output);
		//~ yLabel.text = ZeroPad(y_output);
		
		//~ InputManager.outputs[name + ".x_output"] = x_output;
		//~ InputManager.outputs[name + ".y_output"] = y_output;
	}
}

function MouseOn(hit : RaycastHit){
	tx = hit.textureCoord.x;
	x = (tx * 2) - 1.0;
	reticle.transform.localPosition.x = x * xRange;
	x_output = InputManager.AxisToByte(x);
	
	
	
	ty = hit.textureCoord.y;
	y = (ty * 2) - 1.0;
	reticle.transform.localPosition.y = y * yRange;
	y_output = InputManager.AxisToByte(y);
	
	UpdateOutput();
}

function UpdateOutput(){
	xLabel.text = ZeroPad(x_output);
	yLabel.text = ZeroPad(y_output);
	InputManager.outputs[name + ".x_output"] = x_output;
	InputManager.outputs[name + ".y_output"] = y_output;
}

function ZeroPad(i : int){
	str = "";
	if(i < 100) str = str + "0";
	if(i < 10) str = str + "0";
	str = str + i;
	return str;
}