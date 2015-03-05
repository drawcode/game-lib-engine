using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameVehicleDriveData {
    public float inputAxisVertical = 0;
    public float inputAxisHorizontal = 0;
    public bool inputUse = false;
    public bool inputBrake = false;
    public bool inputGas = true;

}

public class GameVehicleDrive : GameObjectBehavior {

    public GameVehicleDriveData vehicleDriveData;
    
    public GameObjectMountVehicle mountPrimary;
    public GameObjectMountVehicle mountSecondary;

    public AudioClip collisionSound;   

    //Fahrzeugsteuerung
    public WheelCollider flWheelCollider;
    public WheelCollider frWheelCollider;
    public WheelCollider rlWheelCollider;
    public WheelCollider rrWheelCollider;
    public float maxTorque = 150.0f;
    public float maxBrakeTorque = 500.0f;
    public float maxSteerAngle = 30.0f;
    public float maxSpeedSteerAngle = 10.0f;
    public float maxSpeed = 200.0f;
    public float maxBackwardSpeed = 40.0f;
    public float currentSpeed = 0.0f;
    private bool isBraking = false;
    //Reifenvisualisierung
    public Transform flWheel;
    public Transform frWheel;
    public Transform rlWheel;
    public Transform rrWheel;
    public int turboForce = 500000;
    public int explosionForce = 500000;
    //wenn die Raeder sich falsch herum drehen, dann diesen Parameter aktivieren.
    public bool inverseWheelTurning = false;
    private int  wheelTurningParameter = 1;
    //Gaenge

    public List<int> gearSpeed;
    private int currentGear = 0;
    //Vollbremsung
    public float FullBrakeTorque = 5000.00f;
    public AudioClip brakeSound ;
    public bool groundEffectsOn = true;

    //Autosound
    public AudioClip motorSound;
    private AudioSource motorAudioSource;

    //Autosound
    public AudioClip motorSoundLow;
    private AudioSource motorAudioSourceLow;

    //Autosound
    public AudioClip motorSoundMid;
    private AudioSource motorAudioSourceMid;

    //Autosound
    public AudioClip turboSound;
    private AudioSource turboAudioSource;

    //geschwindigkeit

    private WheelFrictionCurve frForwardFriction = new WheelFrictionCurve();
    private WheelFrictionCurve flForwardFriction = new WheelFrictionCurve();
    private WheelFrictionCurve rrForwardFriction = new WheelFrictionCurve();
    private WheelFrictionCurve rlForwardFriction = new WheelFrictionCurve();
    private WheelFrictionCurve frSidewaysFriction = new WheelFrictionCurve();
    private WheelFrictionCurve flSidewaysFriction = new WheelFrictionCurve();
    private WheelFrictionCurve rrSidewaysFriction = new WheelFrictionCurve();
    private WheelFrictionCurve rlSidewaysFriction = new WheelFrictionCurve();
    private float oldForwardFriction = 0.00f;
    private float oldSidewaysFriction = 0.00f;
    private float brakingForwardFriction = 0.03f;
    private float brakingSidewaysFriction = 0.03f;
    private float brakingSidewaysFrictionBackward = 0.01f;
    private float stopForwardFriction = 1;
    private float stopSidewaysFriction = 1;
    private AudioSource brakeAudioSource ;
    private bool isPlayingSound = false;

    //Dust
    public Transform DustL;
    public Transform DustR;
    public float centerOfMassY = 0;

    //Skidmarks
    public Transform skidmark;
    private Vector3 lastSkidmarkPosR;
    private Vector3 lastSkidmarkPosL;

    void Awake() {
  
        vehicleDriveData = new GameVehicleDriveData();

        if (inverseWheelTurning) {
            wheelTurningParameter = -1;
        }
        else {
            wheelTurningParameter = 1;
        }
        
        rigidbody.centerOfMass = new Vector3(0, centerOfMassY, 0);        
        
        oldForwardFriction = frWheelCollider.forwardFriction.stiffness;
        oldSidewaysFriction = frWheelCollider.sidewaysFriction.stiffness;

        InitSound();    
        
    }

    void Start() {        

        frForwardFriction = frWheelCollider.forwardFriction;
        flForwardFriction = flWheelCollider.forwardFriction;
        rrForwardFriction = rrWheelCollider.forwardFriction;
        rlForwardFriction = rlWheelCollider.forwardFriction;

        frSidewaysFriction = frWheelCollider.sidewaysFriction;
        flSidewaysFriction = flWheelCollider.sidewaysFriction;
        rrSidewaysFriction = rrWheelCollider.sidewaysFriction;
        rlSidewaysFriction = rlWheelCollider.sidewaysFriction;        
    }

    void InitSound() {
        
        brakeAudioSource = gameObject.AddComponent<AudioSource>();
        brakeAudioSource.clip = brakeSound;
        brakeAudioSource.loop = true;
        brakeAudioSource.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        //brakeAudioSource.rolloffMode = AudioRolloffMode.Linear;
        brakeAudioSource.playOnAwake = false;
        

        turboAudioSource = gameObject.AddComponent<AudioSource>();
        turboAudioSource.clip = turboSound;
        turboAudioSource.loop = false;
        turboAudioSource.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        //turboAudioSource.rolloffMode = AudioRolloffMode.Linear;
        turboAudioSource.playOnAwake = false;

        motorAudioSource = gameObject.AddComponent<AudioSource>();
        motorAudioSource.clip = motorSound;
        motorAudioSource.loop = true;
        motorAudioSource.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        motorAudioSource.playOnAwake = false;
        motorAudioSource.pitch = 0.1f;
        //motorAudioSource.rolloffMode = AudioRolloffMode.Linear;
        motorAudioSource.Play();

        motorAudioSourceLow = gameObject.AddComponent<AudioSource>();
        motorAudioSourceLow.clip = motorSoundLow;
        motorAudioSourceLow.loop = true;
        motorAudioSourceLow.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        motorAudioSourceLow.playOnAwake = false;
        motorAudioSourceLow.pitch = 0.1f;
        //motorAudioSourceLow.rolloffMode = AudioRolloffMode.Linear;
        motorAudioSourceLow.Play();

        motorAudioSourceMid = gameObject.AddComponent<AudioSource>();
        motorAudioSourceMid.clip = motorSoundMid;
        motorAudioSourceMid.loop = true;
        motorAudioSourceMid.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        motorAudioSourceMid.playOnAwake = false;
        motorAudioSourceMid.pitch = 0.1f;
        //motorAudioSourceMid.rolloffMode = AudioRolloffMode.Linear;
        motorAudioSourceMid.Play();

    }

    void FixedUpdate() {        
        
        if(!GameConfigs.isGameRunning) {

            return;
        }

        currentSpeed = (Mathf.PI * 2 * flWheelCollider.radius) * flWheelCollider.rpm * 60 / 1000;
        currentSpeed = Mathf.Round(currentSpeed);

        if (((currentSpeed > 0) && (vehicleDriveData.inputAxisVertical < 0)) 
            || ((currentSpeed < 0) && (vehicleDriveData.inputAxisVertical > 0))) {
            isBraking = true;
        }
        else {
            isBraking = false;
                
            flWheelCollider.brakeTorque = 0;
            frWheelCollider.brakeTorque = 0;            
        }
            
        if (isBraking == false) {
            if ((currentSpeed < maxSpeed) && (currentSpeed > (maxBackwardSpeed * -1))) {
                    
                flWheelCollider.motorTorque = maxTorque * vehicleDriveData.inputAxisVertical;
                frWheelCollider.motorTorque = maxTorque * vehicleDriveData.inputAxisVertical;                    
                    
            }
            else {
                flWheelCollider.motorTorque = 0;
                frWheelCollider.motorTorque = 0;
            }
        }
        else {
            flWheelCollider.brakeTorque = maxBrakeTorque;
            frWheelCollider.brakeTorque = maxBrakeTorque;
            flWheelCollider.motorTorque = 0;
            frWheelCollider.motorTorque = 0;
        }
            
        //je hoeher die Geschwindigkeit,  desto geringer der maximale Einschlagwinkel.
        float speedProcent = currentSpeed / maxSpeed;
        speedProcent = Mathf.Clamp(speedProcent, 0, 1);
        float speedControlledMaxSteerAngle;
        speedControlledMaxSteerAngle = maxSteerAngle - ((maxSteerAngle - maxSpeedSteerAngle) * speedProcent);
        flWheelCollider.steerAngle = speedControlledMaxSteerAngle * vehicleDriveData.inputAxisHorizontal;
        frWheelCollider.steerAngle = speedControlledMaxSteerAngle * vehicleDriveData.inputAxisHorizontal;

        FullBraking();
            
        SetCurrentGear();
        HandleGearSound();   
        HandleAboveGround();
    
    }

    public void HandleAboveGround() {

        Vector3 currentPosition = transform.position;

        // Never let the vehicle go below ground due to collision or physics
        if(currentPosition.y < .05f) {
            currentPosition.y = .2f;
            transform.position = currentPosition;
        }
    }

    void FullBraking() {
        if (vehicleDriveData.inputBrake) {    
        
            rlWheelCollider.brakeTorque = FullBrakeTorque;
            rrWheelCollider.brakeTorque = FullBrakeTorque;

            if ((Mathf.Abs(rigidbody.velocity.z) > 1) || (Mathf.Abs(rigidbody.velocity.x) > 1)) {
                
                SetFriction(brakingForwardFriction, brakingSidewaysFriction, brakingSidewaysFrictionBackward);
                SetBrakeEffects(true);  
            }
            else {
                SetFriction(stopForwardFriction, stopSidewaysFriction); 
                SetBrakeEffects(false);     
            }
            
        }
        else {      
            rlWheelCollider.brakeTorque = 0;
            rrWheelCollider.brakeTorque = 0;
            SetFriction(oldForwardFriction, oldSidewaysFriction);   
            SetBrakeEffects(false);     
        }   
    }
        
    void SetFriction(float MyForwardFriction, float MySidewaysFriction) {      

        frForwardFriction.stiffness = MyForwardFriction;
        frWheelCollider.forwardFriction = frForwardFriction;

        flForwardFriction.stiffness = MyForwardFriction;
        flWheelCollider.forwardFriction = flForwardFriction;

        rrForwardFriction.stiffness = MyForwardFriction;
        rrWheelCollider.forwardFriction = rrForwardFriction;

        rlForwardFriction.stiffness = MyForwardFriction;
        rlWheelCollider.forwardFriction = rlForwardFriction;

        frSidewaysFriction.stiffness = MySidewaysFriction;
        frWheelCollider.sidewaysFriction = frSidewaysFriction;

        flSidewaysFriction.stiffness = MySidewaysFriction;
        flWheelCollider.sidewaysFriction = flSidewaysFriction;

        rrSidewaysFriction.stiffness = MySidewaysFriction;
        rrWheelCollider.sidewaysFriction = rrSidewaysFriction;

        rlSidewaysFriction.stiffness = MySidewaysFriction;
        rlWheelCollider.sidewaysFriction = rlSidewaysFriction;
            
    }

    void SetFriction(float MyForwardFriction, float MySidewaysFriction, float MySidewaysFrictionBackward) {
       
        frForwardFriction.stiffness = MyForwardFriction;
        frWheelCollider.forwardFriction = frForwardFriction;

        flForwardFriction.stiffness = MyForwardFriction;
        flWheelCollider.forwardFriction = flForwardFriction;

        rrForwardFriction.stiffness = MyForwardFriction;
        rrWheelCollider.forwardFriction = rrForwardFriction;

        rlForwardFriction.stiffness = MyForwardFriction;
        rlWheelCollider.forwardFriction = rlForwardFriction;

        frSidewaysFriction.stiffness = MySidewaysFriction;
        frWheelCollider.sidewaysFriction = frSidewaysFriction;

        flSidewaysFriction.stiffness = MySidewaysFriction;
        flWheelCollider.sidewaysFriction = flSidewaysFriction;

        rrSidewaysFriction.stiffness = MySidewaysFrictionBackward;
        rrWheelCollider.sidewaysFriction = rrSidewaysFriction;

        rlSidewaysFriction.stiffness = MySidewaysFrictionBackward;
        rlWheelCollider.sidewaysFriction = rlSidewaysFriction;
            
    }

    void SetBrakeEffects(bool PlayEffects) {

        bool isGrounding = false;
        Vector3 skidmarkPos;
        Vector3 relativePos;
        Quaternion rotationToLastSkidmark;
        
        ParticleEmitter particleEmitterL = DustL.GetComponent<ParticleEmitter>();
        ParticleEmitter particleEmitterR = DustR.GetComponent<ParticleEmitter>(); 

        if (PlayEffects == true) {

            WheelHit hit;
            
            if (rlWheelCollider.GetGroundHit(out hit)) {
             
                particleEmitterL.emit = true;
                isGrounding = true;
                //Skidmarks
                skidmarkPos = hit.point;
                skidmarkPos.y += 0.1f;
        
                
                if (lastSkidmarkPosL != Vector3.zero) {
                    relativePos = lastSkidmarkPosL - skidmarkPos;
                    rotationToLastSkidmark = Quaternion.LookRotation(relativePos);
                    Instantiate(skidmark, skidmarkPos, rotationToLastSkidmark);
                }
                lastSkidmarkPosL = skidmarkPos;
            }
            else {
                    
                particleEmitterL.emit = false; 
                lastSkidmarkPosL = Vector3.zero;            
            }
                    
            if (rrWheelCollider.GetGroundHit(out hit)) {
               
                particleEmitterR.emit = true;
                isGrounding = true;
                //Skidmarks
                skidmarkPos = hit.point;
                skidmarkPos.y += 0.1f;
                
                if (lastSkidmarkPosR != Vector3.zero) {
                    relativePos = lastSkidmarkPosR - skidmarkPos;
                    rotationToLastSkidmark = Quaternion.LookRotation(relativePos);
                    Instantiate(skidmark, skidmarkPos, rotationToLastSkidmark);
                }
                lastSkidmarkPosR = skidmarkPos;
            }
            else {
                        
                particleEmitterR.emit = false; 
                lastSkidmarkPosR = Vector3.zero;            
            }
                    
            if ((isPlayingSound == false) && (isGrounding == true)) {
                isPlayingSound = true;
                brakeAudioSource.Play();                    
            }
            if (isGrounding == false) {
                isPlayingSound = false;
                brakeAudioSource.Stop();                    
            }
            
        }
        else {
            isPlayingSound = false;
            brakeAudioSource.Stop();

            particleEmitterL.emit = false; 
            particleEmitterR.emit = false; 
            lastSkidmarkPosL = Vector3.zero;    
            lastSkidmarkPosR = Vector3.zero;    
        }        
    }
  
    void Update() {

        if(GameConfigs.isUIRunning) {
            return;
        }
        
        if(!GameConfigs.isGameRunning) {            
            return;
        }

        RotateWheels();
        SteelWheels();       
    }

    void RotateWheels() {
        flWheel.Rotate(flWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter, 0, 0);  
        frWheel.Rotate(frWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter, 0, 0);  
        rlWheel.Rotate(rlWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter, 0, 0);  
        rrWheel.Rotate(rrWheelCollider.rpm / 60 * 360 * Time.deltaTime * wheelTurningParameter, 0, 0);  
    }

    void SteelWheels() {

        flWheel.localEulerAngles = 
            new Vector3(flWheel.localEulerAngles.x, 
                        flWheelCollider.steerAngle - flWheel.localEulerAngles.z, 
                        flWheel.localEulerAngles.z);

        frWheel.localEulerAngles = new Vector3(
            frWheel.localEulerAngles.x, 
            frWheelCollider.steerAngle - frWheel.localEulerAngles.z, 
            frWheel.localEulerAngles.z);
    }

    void SetCurrentGear() {
        int gearNumber;

        gearNumber = gearSpeed.Count;
        
        for (var i=0; i< gearNumber; i++) {
            if (gearSpeed[i] > currentSpeed) {
                currentGear = i;
                break;
            }
        }
    }

    public void HandleGearSound() {

        if(!GameConfigs.isGameRunning) {
            
            motorAudioSource.volume = 0;
            motorAudioSourceLow.volume = 0;
            motorAudioSourceMid.volume = 0;

            return;
        }

        float tempMinSpeed = 0.00f;
        float tempMaxSpeed = 0.00f;
        float currentPitch = 0.00f;
        
        switch (currentGear) {
        case 0:
            tempMinSpeed = 0.00f;
            tempMaxSpeed = gearSpeed[currentGear];
            break;
                
        default:
            tempMinSpeed = gearSpeed[currentGear - 1];
            tempMaxSpeed = gearSpeed[currentGear];
            break;
        }
        
        currentPitch = (float)(((Mathf.Abs(currentSpeed) - tempMinSpeed) / (tempMaxSpeed - tempMinSpeed)) + 0.8);

        if (currentPitch > 2) {
            currentPitch = 2;
        }


        if(currentSpeed < 0.1f) {
            motorAudioSource.volume = 0;
            motorAudioSourceLow.volume = 0;
            motorAudioSourceMid.volume = 0;
        }
        else {
            motorAudioSource.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();
            motorAudioSourceLow.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();
            motorAudioSourceMid.volume = (float)GameProfiles.Current.GetAudioEffectsVolume();
        }

        motorAudioSource.pitch = currentPitch;

        motorAudioSourceLow.pitch = currentPitch;
        motorAudioSourceMid.pitch = currentPitch;       
        
    }    

}
