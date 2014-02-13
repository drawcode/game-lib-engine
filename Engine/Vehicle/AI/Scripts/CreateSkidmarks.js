var skidmark : Transform;
private var wheelCol : WheelCollider;

private var newPos : Vector3;
private var wheelDistance : Vector3;
function Awake () {
	
	wheelCol = GetComponent(WheelCollider);
}
function LateUpdate () {
	var hit : WheelHit;
	if(wheelCol.GetGroundHit(hit)){
		if ((Mathf.Abs(hit.forwardSlip) > 5) || (Mathf.Abs(hit.sidewaysSlip)> 5)) {
			newPos = hit.point;
			newPos.y +=0.1;
			
			Instantiate( skidmark,newPos,transform.rotation);
		}		
	}
}