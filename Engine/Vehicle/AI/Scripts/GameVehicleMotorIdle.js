
function FixedUpdate () {
	IdleSound();
}


function IdleSound(){
	var currentPitch : float =0.00;
		
	currentPitch = Input.GetAxis("Vertical") + 0.8;
	audio.pitch = currentPitch;
}