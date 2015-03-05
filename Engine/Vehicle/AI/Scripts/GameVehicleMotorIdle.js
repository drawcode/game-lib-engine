
function FixedUpdate () {
	IdleSound();
}


function IdleSound(){
	var currentPitch : float =0.00;
		
	currentPitch = Input.GetAxis("Vertical") + 0.8;
	GetComponent(AudioSource).pitch = currentPitch;
}