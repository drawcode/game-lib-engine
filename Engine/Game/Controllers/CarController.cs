using System;
using System.Collections.Generic;
using Engine;
using Engine.Game.Controllers;
using UnityEngine;

//namespace Engine.Game.Controllers {
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
public class CarController : MonoBehaviour {

    //maximal corner and braking acceleration capabilities
    public float maxCornerAccel = 10.0f;

    public float maxBrakeAccel = 10.0f;

    //center of gravity height - effects tilting in corners
    public float cogY = 0.0f;

    //engine powerband
    public int minRPM = 700;

    public int maxRPM = 6000;

    //maximum Engine Torque
    public int maxTorque = 400;

    //automatic transmission shift points
    public int shiftDownRPM = 2500;

    public int shiftUpRPM = 5500;

    //gear ratios
    public List<float> gearRatios = new List<float>();//[-2.66, 2.66, 1.78, 1.30, 1.00];

    public float finalDriveRatio = 3.4f;

    //a basic handling modifier:
    //1.0 understeer
    //0.0 oversteer
    public float handlingTendency = 0.7f;

    //graphical wheel objects
    public Transform wheelFR;

    public Transform wheelFL;
    public Transform wheelBR;
    public Transform wheelBL;

    //suspension setup
    public float suspensionDistance = 0.3f;

    public int springs = 1000;
    public int dampers = 200;
    public float wheelRadius = 0.45f;

    //particle effect for ground dust
    public Transform groundDustEffect;

    private bool queryUserInput = true;
    private float engineRPM = 0.0f;
    private float steerVelo = 0.0f;
    private float brake = 0.0f;
    private float handbrake = 0.0f;
    private float steer = 0.0f;
    private float motor = 0.0f;

    //private float skidTime = 0.0f;
    private bool onGround = false;

    private float cornerSlip = 0.0f;
    private float driveSlip = 0.0f;
    private float wheelRPM = 0.0f;
    private int gear = 1;

    //private Skidmarks skidmarks;
    private WheelData[] wheels;

    private float wheelY = 0.0f;
    private float rev = 0.0f;

    //Functions to be used by external scripts
    //controlling the car if required
    //===================================================================

    //return a status string for the vehicle
    public void GetStatus(GUIText gui) {
        gui.text = "v=" + (rigidbody.velocity.magnitude * 3.6f).ToString("f1") + " km/h\ngear= " + gear + "\nrpm= " + engineRPM.ToString("f0");
    }

    //return an information string for the vehicle
    public void GetControlString(GUIText gui) {
        gui.text = "Use arrow keys to control the jeep,\nspace for handbrake.";
    }

    //Enable or disable user controls
    public void SetEnableUserInput(bool enableInput) {
        queryUserInput = enableInput;
    }

    //Car physics
    //===================================================================

    //some whee calculation data
    public class WheelData {
        public float rotation = 0.0f;
        public WheelCollider coll;
        public Transform graphic;
        public float maxSteerAngle = 0.0f;
        public int lastSkidMark = -1;
        public bool powered = false;
        public bool handbraked = false;
        public Quaternion originalRotation;
    };

    private void Start() {
        gearRatios.Add(3.66f);
        gearRatios.Add(2.66f);
        gearRatios.Add(1.78f);
        gearRatios.Add(1.30f);
        gearRatios.Add(1.00f);

        //setup wheels
        wheels = new WheelData[4];
        for (int i = 0; i < 4; i++)
            wheels[i] = new WheelData();

        wheels[0].graphic = wheelFL;
        wheels[1].graphic = wheelFR;
        wheels[2].graphic = wheelBL;
        wheels[3].graphic = wheelBR;

        wheels[0].maxSteerAngle = 30.0f;
        wheels[1].maxSteerAngle = 30.0f;
        wheels[2].powered = true;
        wheels[3].powered = true;
        wheels[2].handbraked = true;
        wheels[3].handbraked = true;

        foreach (WheelData w in wheels) {
            if (w.graphic == null)
                Debug.Log("You need to assign all four wheels for the car script!");
            if (!w.graphic.transform.IsChildOf(transform))
                Debug.Log("Wheels need to be children of the Object with the car script");

            w.originalRotation = w.graphic.localRotation;

            //create collider
            GameObject colliderObject = new GameObject("WheelCollider");
            colliderObject.transform.parent = transform;
            colliderObject.transform.position = w.graphic.position;
            w.coll = colliderObject.AddComponent<WheelCollider>();
            w.coll.suspensionDistance = suspensionDistance;
            JointSpring jSpring = w.coll.suspensionSpring;
            jSpring.spring = springs;
            jSpring.damper = springs;
            w.coll.suspensionSpring = jSpring;

            //no grip, as we simulate handling ourselves
            WheelFrictionCurve wFrictionCurveForward = new WheelFrictionCurve();
            wFrictionCurveForward.stiffness = 0;
            WheelFrictionCurve wFrictionCurveSideways = new WheelFrictionCurve();
            wFrictionCurveSideways.stiffness = 0;
            w.coll.forwardFriction = wFrictionCurveForward;
            w.coll.sidewaysFriction = wFrictionCurveSideways;
            w.coll.radius = wheelRadius;
        }

        //get wheel height (height forces are applied on)
        wheelY = wheels[0].graphic.localPosition.y;

        //setup center of gravity
        Vector3 centerMassAdjust = rigidbody.centerOfMass;
        centerMassAdjust.y = cogY;
        rigidbody.centerOfMass = centerMassAdjust;

        //find skidmark object
        //skidmarks = FindObjectOfType(typeof(Skidmarks));

        //shift to first
        gear = 1;
    }

    //update wheel status
    public void UpdateWheels() {

        //calculate handbrake slip for traction gfx
        float handbrakeSlip = handbrake * rigidbody.velocity.magnitude * 0.1f;
        if (handbrakeSlip > 1)
            handbrakeSlip = 1;

        float totalSlip = 0.0f;
        onGround = false;
        foreach (WheelData w in wheels) {

            //rotate wheel
            w.rotation += wheelRPM / 60.0f * -rev * 360.0f * Time.fixedDeltaTime;
            w.rotation = Mathf.Repeat(w.rotation, 360.0f);
            w.graphic.localRotation = Quaternion.Euler(w.rotation, w.maxSteerAngle * steer, 0.0f) * w.originalRotation;

            //check if wheel is on ground
            if (w.coll.isGrounded)
                onGround = true;

            float slip = cornerSlip + (w.powered ? driveSlip : 0.0f) + (w.handbraked ? handbrakeSlip : 0.0f);
            totalSlip += slip;

            WheelHit hit;
            WheelCollider c;
            c = w.coll;
            if (c.GetGroundHit(out hit)) {

                //if the wheel touches the ground, adjust graphical wheel position to reflect springs
                Vector3 tmpLocalPostion = w.graphic.localPosition;
                tmpLocalPostion.y -= Vector3.Dot(w.graphic.position - hit.point, transform.up) - w.coll.radius;
                w.graphic.localPosition = tmpLocalPostion;

                //create dust on ground if appropiate
                if (slip > 0.5 && hit.collider.tag == "Dusty") {
                    groundDustEffect.position = hit.point;
                    groundDustEffect.particleEmitter.worldVelocity = rigidbody.velocity * 0.5f;
                    groundDustEffect.particleEmitter.minEmission = (slip - 0.5f) * 3f;
                    groundDustEffect.particleEmitter.maxEmission = (slip - 0.5f) * 3f;
                    groundDustEffect.particleEmitter.Emit();
                }

                //and skid marks
                //if(slip>0.75 && skidmarks != null)
                //	w.lastSkidMark=skidmarks.AddSkidMark(hit.point,hit.normal,(slip-0.75)*2,w.lastSkidMark);
                //else
                //	w.lastSkidMark=-1;
            }
            else
                w.lastSkidMark = -1;
        }
        totalSlip /= wheels.Length;
    }

    //Automatically shift gears
    public void AutomaticTransmission() {
        if (gear > 0) {
            if (engineRPM > shiftUpRPM && gear < gearRatios.Count - 1)
                gear++;
            if (engineRPM < shiftDownRPM && gear > 1)
                gear--;
        }
    }

    //Calculate engine acceleration force for current RPM and trottle
    public float CalcEngine() {

        //no engine when braking
        if (brake + handbrake > 0.1f)
            motor = 0.0f;

        //if car is airborne, just rev engine
        if (!onGround) {
            engineRPM += (motor - 0.3f) * 25000.0f * Time.deltaTime;
            engineRPM = Mathf.Clamp(engineRPM, minRPM, maxRPM);
            return 0.0f;
        }
        else {
            AutomaticTransmission();
            engineRPM = wheelRPM * gearRatios[gear] * finalDriveRatio;
            if (engineRPM < minRPM)
                engineRPM = minRPM;
            if (engineRPM < maxRPM) {

                //fake a basic torque curve
                float x = (2 * (engineRPM / maxRPM) - 1);
                float torqueCurve = 0.5f * (-x * x + 2);
                float torqueToForceRatio = gearRatios[gear] * finalDriveRatio / wheelRadius;
                return motor * maxTorque * torqueCurve * torqueToForceRatio;
            }
            else

                //rpm delimiter
                return 0.0f;
        }
    }

    //Car physics
    //The physics of this car are really a trial-and-error based extension of
    //basic "Asteriods" physics -- so you will get a pretty arcade-like feel.
    //This may or may not be what you want, for a more physical approach research
    //the wheel colliders
    public void HandlePhysics() {
        Vector3 velo = rigidbody.velocity;
        wheelRPM = velo.magnitude * 60.0f * 0.5f;

        rigidbody.angularVelocity = new Vector3(rigidbody.angularVelocity.x, 0.0f, rigidbody.angularVelocity.z);
        Vector3 dir = transform.TransformDirection(Vector3.forward);
        Vector3 flatDir = Vector3.Normalize(new Vector3(dir.x, 0, dir.z));
        Vector3 flatVelo = new Vector3(velo.x, 0, velo.z);
        rev = Mathf.Sign(Vector3.Dot(flatVelo, flatDir));

        //when moving backwards or standing and brake is pressed, switch to reverse
        if ((rev < 0 || flatVelo.sqrMagnitude < 0.5) && brake > 0.1)
            gear = 0;
        if (gear == 0) {

            //when in reverse, flip brake and gas
            float tmp = brake;
            brake = motor;
            motor = tmp;

            //when moving forward or standing and gas is pressed, switch to drive
            if ((rev > 0 || flatVelo.sqrMagnitude < 0.5) && brake > 0.1)
                gear = 1;
        }
        Vector3 engineForce = flatDir * CalcEngine();
        float totalbrake = brake + handbrake * 0.5f;
        if (totalbrake > 1.0f)
            totalbrake = 1.0f;
        Vector3 brakeForce = -flatVelo.normalized * totalbrake * rigidbody.mass * maxBrakeAccel;

        flatDir *= flatVelo.magnitude;
        flatDir = Quaternion.AngleAxis(steer * 30.0f, Vector3.up) * flatDir;
        flatDir *= rev;
        float diff = (flatVelo - flatDir).magnitude;
        float cornerAccel = maxCornerAccel;
        if (cornerAccel > diff)
            cornerAccel = diff;
        Vector3 cornerForce = -(flatVelo - flatDir).normalized * cornerAccel * rigidbody.mass;
        cornerSlip = Mathf.Pow(cornerAccel / maxCornerAccel, 3);

        rigidbody.AddForceAtPosition(brakeForce + engineForce + cornerForce, transform.position + transform.up * wheelY);

        float handbrakeFactor = 1 + handbrake * 4;
        if (rev < 0)
            handbrakeFactor = 1;
        float veloSteer = ((15 / (2 * velo.magnitude + 1)) + 1) * handbrakeFactor;
        float steerGrip = (1 - handlingTendency * cornerSlip);
        if (rev * steer * steerVelo < 0)
            steerGrip = 1;
        float maxRotSteer = 2 * Time.fixedDeltaTime * handbrakeFactor * steerGrip;
        float fVelo = velo.magnitude;
        float veloFactor = fVelo < 1.0f ? fVelo : Mathf.Pow(velo.magnitude, 0.3f);
        float steerVeloInput = rev * steer * veloFactor * 0.5f * Time.fixedDeltaTime * handbrakeFactor;
        if (velo.magnitude < 0.1f)
            steerVeloInput = 0f;
        if (steerVeloInput > steerVelo) {
            steerVelo += 0.02f * Time.fixedDeltaTime * veloSteer;
            if (steerVeloInput < steerVelo)
                steerVelo = steerVeloInput;
        }
        else {
            steerVelo -= 0.02f * Time.fixedDeltaTime * veloSteer;
            if (steerVeloInput > steerVelo)
                steerVelo = steerVeloInput;
        }
        steerVelo = Mathf.Clamp(steerVelo, -maxRotSteer, maxRotSteer);
        transform.Rotate(Vector3.up * steerVelo * 57.295788f);
    }

    private void FixedUpdate() {

        //query input axes if necessarry
        if (queryUserInput) {
            brake = Mathf.Clamp01(-Input.GetAxis("Vertical"));
            handbrake = Input.GetButton("Jump") ? 1.0f : 0.0f;
            steer = Input.GetAxis("Horizontal");
            motor = Mathf.Clamp01(Input.GetAxis("Vertical"));
        }
        else {
            motor = 0;
            steer = 0;
            brake = 0;
            handbrake = 0;
        }

        //if car is on ground calculate handling, otherwise just rev the engine
        if (onGround)
            HandlePhysics();
        else
            CalcEngine();

        //wheel GFX
        UpdateWheels();

        //engine sounds
        audio.pitch = 0.5f + 0.2f * motor + 0.8f * engineRPM / maxRPM;
        audio.volume = 0.5f + 0.8f * motor + 0.2f * engineRPM / maxRPM;
    }

    //Called by DamageReceiver if boat destroyed
    public void Detonate() {

        //destroy wheels
        foreach (WheelData w in wheels)
            w.coll.gameObject.active = false;

        //no more car physics
        enabled = false;
    }
}

//}