private var UDPHost : String = "127.0.0.1";
private var listenerPort : int = 8000;
private var broadcastPort : int = 57131;
private var oscHandler : Osc;
private var mm : middleMan;


private 
private var eventName : String = "";
private var eventData : String = "";
private var posX : int = 0;
private var posZ : int = 0;
private var area : int = 0;
private var messages;
private var temp;
private var whirl;
//public var output_txt : UnityEngine.UI.Text;

public function Start ()
{	
	messages = new Array();
	temp = new Array();

	var udp : UDPPacketIO = GetComponent("UDPPacketIO");
	mm = GameObject.FindGameObjectWithTag("Global").GetComponent("middleMan");
	udp.init(UDPHost, broadcastPort, listenerPort);
	oscHandler = GetComponent("Osc");
	oscHandler.init(udp);
			
	//oscHandler.SetAddressHandler("/eventTest", updateText);
	oscHandler.SetAddressHandler("/positionData", positionData);
	whirl = GameObject.Find("Whirl");
}
Debug.Log("Running");

function Update () {
/*
	var cube = GameObject.Find("trackingTarget");
	var x:int = posX;
	var z:int = posZ;
	var str:int = area;
    //cube.transform.localScale = Vector3(boxWidth,5,boxHeight);	
	cube.transform.position = new Vector3(x,35,z);
	cube.GetComponent("trackingCast").SetRange(str);    
*/
	if(messages.length > 0)
	{
		temp = messages.pop();
		var cube = GameObject.Find("trackingTarget");

		var x:int = temp.mPosX;
		var z:int = temp.mPosZ;
		var str:int = temp.mArea;
		var id:int = temp.mId;

    	//cube.transform.localScale = Vector3(boxWidth,5,boxHeight);
		cube.transform.position = new Vector3(x,-3,z);
		whirl.transform.position =	cube.transform.position;

		//print("ID: "+id);
		cube.GetComponent("trackingCast").SetRange(str);
		 
	}
	else
	{
		if(Random.Range(0,100) == 1)
		{
			whirl.transform.position = new Vector3(-100,0,-100);
		}
	}
}	
public function positionData(oscMessage : OscMessage) : void
{	
	mm.SetNData(true);
	//print("sendimg true");
	Osc.OscMessageToString(oscMessage);
    posX = oscMessage.Values[0];
    posZ = oscMessage.Values[1];
    area = oscMessage.Values[2];
    id = oscMessage.Values[3];

    var mes = new m(posX, posZ, area, id);
    messages.push(mes);
    //print(posX +" "+ posZ);
} 

class m{
 var mPosX : int;
 var mPosZ : int;
 var mArea : int;
 var mId : int;

 function m(mPosX : int, mPosZ : int, mArea : int, mId : int){
 	this.mPosX = mPosX;
 	this.mPosZ = mPosZ;
 	this.mId = mId;
 	this.mArea = mArea;
 }
}

/*public function updateText(oscMessage : OscMessage) : void
{	
	eventName = Osc.OscMessageToString(oscMessage);
	eventData = oscMessage.Values[0];
} */

