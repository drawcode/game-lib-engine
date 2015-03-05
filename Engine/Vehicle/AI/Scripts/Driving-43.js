//Driving Script is a free script from www.der-softwareentwickler-blog.de
//It's a result of a video tutorial serial.
var flWheelCollider : WheelCollider;
var frWheelCollider : WheelCollider;
var rlWheelCollider  : WheelCollider;
var rrWheelCollider: WheelCollider;
var maxTorque= 150.0;
var maxBrakeTorque= 500.0;
var maxSteerAngle = 30.0;
var maxSpeed : float = 200.0;
var maxBackwardSpeed : float = 40.0;
var currentSpeed : float =0.0;
private var isBraking : boolean=false;

var flWheel : Transform;
var frWheel : Transform;
var rlWheel : Transform;
var rrWheel : Transform;

var gearSpeed : int [];
private var currentGear: int=0;

var FullBrakeTorque: float = 5000.00;
var brakeSound : AudioClip;

var groundEffectsOn : boolean =true;

//Skidmarks
var skidmark : Transform;
private var lastSkidmarkPosR : Vector3;
private var lastSkidmarkPosL : Vector3;

private var oldForwardFriction : float =0.00;
private var oldSidewaysFriction : float =0.00;
private var brakingForwardFriction : float =0.03;
private var brakingSidewaysFriction : float =0.03;
private var stopForwardFriction : float =1;
private var stopSidewaysFriction : float =1;
private var brakeAudioSource : AudioSource;
private var isPlayingSound : boolean=false;

//Dust
private var DustL : Transform;
private var DustR : Transform;
private var DustFL : Transform;
private var DustFR : Transform;
var tagNameTest : String;

var rigid : Rigidbody = null;


function Awake () {	
	//guiSpeed.material.color = Color.green;
	
	rigid = GetComponent(Rigidbody);
	
	rigid.centerOfMass.y = 0;	
	rigid.centerOfMass.z = 0;	
	//rigidbody.centerOfMass.z = 0;		
	oldForwardFriction = frWheelCollider.forwardFriction.stiffness;
	oldSidewaysFriction = frWheelCollider.sidewaysFriction.stiffness;
	brakeAudioSource = gameObject.AddComponent(AudioSource);
	brakeAudioSource.clip = brakeSound;
	brakeAudioSource.loop = true;
	brakeAudioSource.volume = 0.7;	
	brakeAudioSource.playOnAwake = false;
	brakeAudioSource.rolloffMode = AudioRolloffMode.Linear;
		
	DustL = transform.Find("DustL");
	DustR = transform.Find("DustR");
	DustFL = transform.Find("DustFL");
	DustFR = transform.Find("DustFR");

}

function FixedUpdate () {
		
	currentSpeed = (Mathf.PI * 2 * flWheelCollider.radius) * flWheelCollider.rpm *60 /1000;
	currentSpeed = Mathf.Round(currentSpeed);
	
	FullBraking ();
	
	
	if (((currentSpeed> 0) && (Input.GetAxis("Vertical") <0 )) || ((currentSpeed< 0) && (Input.GetAxis("Vertical") > 0 ))){
	  	isBraking = true;
	}
	else {
		isBraking = false;
		
		flWheelCollider.brakeTorque =0;
		frWheelCollider.brakeTorque =0;
		
		
	}
	
	if (isBraking ==false) {
		if ((currentSpeed < maxSpeed) && (currentSpeed > (maxBackwardSpeed*-1))){
			
			flWheelCollider.motorTorque =  maxTorque * Input.GetAxis("Vertical");
			frWheelCollider.motorTorque = maxTorque * Input.GetAxis("Vertical");	
			
			
		}
		else {
			flWheelCollider.motorTorque =  0;
			frWheelCollider.motorTorque =  0;
		}
	}
	else {
		flWheelCollider.brakeTorque = maxBrakeTorque;
		frWheelCollider.brakeTorque = maxBrakeTorque;
		flWheelCollider.motorTorque =  0;
		frWheelCollider.motorTorque =  0;
	}
	
	flWheelCollider.steerAngle = maxSteerAngle  * Input.GetAxis("Horizontal");
	frWheelCollider.steerAngle = maxSteerAngle  *  Input.GetAxis("Horizontal");
		
	SetCurrentGear();
	GearSound();	
	
}

function FullBraking (){
	if(Input.GetKey("space")){		
		rlWheelCollider.brakeTorque =FullBrakeTorque;
		rrWheelCollider.brakeTorque =FullBrakeTorque;
		
		if ((Mathf.Abs(rigid.velocity.z)>1) || (Mathf.Abs(rigid.velocity.x)>1)){
		
			SetFriction(brakingForwardFriction,brakingSidewaysFriction);	
			SetBrakeEffects(true);	
		}
		else{
			SetFriction(stopForwardFriction,stopSidewaysFriction);	
			SetBrakeEffects(false);		
		}
		
	}
	else{		
		rlWheelCollider.brakeTorque =0;
		rrWheelCollider.brakeTorque =0;
		SetFriction(oldForwardFriction,oldSidewaysFriction);	
		SetBrakeEffects(false);		
	}	
}
	
function SetFriction(MyForwardFriction :float,MySidewaysFriction:float){

	frWheelCollider.forwardFriction.stiffness = MyForwardFriction;
	flWheelCollider.forwardFriction.stiffness = MyForwardFriction;
	rrWheelCollider.forwardFriction.stiffness = MyForwardFriction;
	rlWheelCollider.forwardFriction.stiffness = MyForwardFriction;
	
	frWheelCollider.sidewaysFriction.stiffness = MySidewaysFriction;
	flWheelCollider.sidewaysFriction.stiffness = MySidewaysFriction;
	rrWheelCollider.sidewaysFriction.stiffness = MySidewaysFriction;
	rlWheelCollider.sidewaysFriction.stiffness = MySidewaysFriction;
		
}

function SetBrakeEffects(PlayEffects : boolean){
	var isGrounding : boolean = false;
	var skidmarkPos : Vector3;
	var rotationToLastSkidmark : Quaternion;
	var relativePos : Vector3;
	if(PlayEffects == true){
		var hit : WheelHit;
		
		if (rlWheelCollider.GetGroundHit(hit)){
			//GameObject.Find("DustL").GetComponent(ParticleEmitter).emit=true;
			DustL.GetComponent(ParticleEmitter).emit=true;
			isGrounding = true;
			skidmarkPos  = hit.point;
			skidmarkPos.y += 0.01;
			skidmarkPos.x += 0.2;
			if (lastSkidmarkPosL != Vector3.zero){
				relativePos = lastSkidmarkPosL - skidmarkPos;
				rotationToLastSkidmark = Quaternion.LookRotation(relativePos);
				Instantiate(skidmark,skidmarkPos,rotationToLastSkidmark);
			}
			lastSkidmarkPosL = skidmarkPos;
		}
		else{
			//GameObject.Find("DustL").GetComponent(ParticleEmitter).emit=false;		
			DustL.GetComponent(ParticleEmitter).emit=false;	
			lastSkidmarkPosL = Vector3.zero;			
		}
				
		if (rrWheelCollider.GetGroundHit(hit)){
			//GameObject.Find("DustR").GetComponent(ParticleEmitter).emit=true;
			DustR.GetComponent(ParticleEmitter).emit=true;
			isGrounding = true;
			skidmarkPos  = hit.point;
			skidmarkPos.y += 0.01;
			skidmarkPos.x -= 0.2;
			if (lastSkidmarkPosR != Vector3.zero){
				relativePos = lastSkidmarkPosR - skidmarkPos;
				rotationToLastSkidmark = Quaternion.LookRotation(relativePos);
				Instantiate(skidmark,skidmarkPos,rotationToLastSkidmark);
			}
			lastSkidmarkPosR = skidmarkPos;
		}
		else{
			//GameObject.Find("DustR").GetComponent(ParticleEmitter).emit=false;		
			DustR.GetComponent(ParticleEmitter).emit=false;	
			lastSkidmarkPosR = Vector3.zero;				
		}
				
		if((isPlayingSound==false)&&(isGrounding == true)){
			isPlayingSound = true;
			brakeAudioSource.Play();					
		}
		if(isGrounding == false){
			isPlayingSound = false;
			brakeAudioSource.Stop();					
		}
		
	}
	else{
		isPlayingSound = false;
		brakeAudioSource.Stop();
		//GameObject.Find("DustL").GetComponent(ParticleEmitter).emit=false;
		//GameObject.Find("DustR").GetComponent(ParticleEmitter).emit=false;
		DustL.GetComponent(ParticleEmitter).emit=false;	
		DustR.GetComponent(ParticleEmitter).emit=false;	
		lastSkidmarkPosL = Vector3.zero;	
		lastSkidmarkPosR = Vector3.zero;	
	}
}

function Update () {

	RotateWheels();
	SteelWheels();
	
}

var OffsetX  : float =0;
	var OffsetY : float =0;


function RotateWheels(){
	flWheel.Rotate(flWheelCollider.rpm / 60 * 360 * Time.deltaTime ,0,0);	
	frWheel.Rotate(frWheelCollider.rpm / 60 * 360 * Time.deltaTime ,0,0);	
	rlWheel.Rotate(rlWheelCollider.rpm / 60 * 360 * Time.deltaTime ,0,0);	
	rrWheel.Rotate(rrWheelCollider.rpm / 60 * 360 * Time.deltaTime ,0,0);	
}

function SteelWheels() {
	flWheel.localEulerAngles.y = flWheelCollider.steerAngle - flWheel.localEulerAngles.z ;
	frWheel.localEulerAngles.y = frWheelCollider.steerAngle - frWheel.localEulerAngles.z ;
}

function SetCurrentGear(){
	var gearNumber :int;
	gearNumber = gearSpeed.length;
	
	for (var i=0; i< gearNumber;i++){
		if(gearSpeed[i]>currentSpeed){
			currentGear = i;
			break;
		}
	}
}

function GearSound(){
	var tempMinSpeed : float=0.00;
	var tempMaxSpeed : float =0.00;
	var currentPitch : float =0.00;
	
	switch (currentGear) {
		case 0:
			tempMinSpeed =0.00;
			tempMaxSpeed = gearSpeed[currentGear];
			break;
			
		default:
			tempMinSpeed = gearSpeed[currentGear -1];
			tempMaxSpeed = gearSpeed[currentGear];
	}
	
	currentPitch =((Mathf.Abs(currentSpeed) - tempMinSpeed)/(tempMaxSpeed-tempMinSpeed)) + 0.8;
	GetComponent(AudioSource).pitch = currentPitch;
}
