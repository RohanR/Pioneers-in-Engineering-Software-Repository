var normalColor : Color;
var autonomousColor : Color;
var disabledColor : Color;

var cam : Camera;
var objects : Renderer[];

function OnGUI(){
	if(GUILayout.Button("normal")){
		SetColor(normalColor);
	}
	
	if(GUILayout.Button("autonomous")){
		SetColor(autonomousColor);
	}
	if(GUILayout.Button("disabled")){
		SetColor(disabledColor);
	}
}

function SetColor(c : Color){
	cam.backgroundColor = c;
	for(var r : Renderer in objects){
		r.material.color = c;
	}
}