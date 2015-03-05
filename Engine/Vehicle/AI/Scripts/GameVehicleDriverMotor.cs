#pragma warning disable 0414
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameVehicleAIDriverMotor : GameObjectBehavior {
    
    public float maxSpeed = 200.0f;
    public float torque = 150.0f;
    public float brakeTorque = 500.0f;
    public float maxSteerAngle = 20.0f;
    private float m_currentMaxSpeed = 0;
    private float m_currentSpeed = 0.0f;
    private bool m_isBraking = false;

    //wenn die Raeder sich falsch herum drehen, dann diesen Parameter aktivieren.
    private bool m_inverseWheelTurning = false;
    private int m_wheelTurningParameter = 1;
    //Gaenge
    public int gears = 5;
    private List<int> m_gearSpeed = new List<int>();
    private int m_currentGear = 0;

    //Autosound
    public bool playSound = true;
    public AudioClip motorSound;
    public float soundVolume = 1;
    private AudioSource m_motorAudioSource;
    private float m_targetAngle;
    private float m_wheelRadius;
    [HideInInspector]
    public int
        currentWaypoint = 0;
    [HideInInspector]
    public float
        aiSteerAngle;
    [HideInInspector]
    public float
        aiSpeedPedal = 1;
    [HideInInspector]
    public float
        aiBrakePedal = 0;
    public Transform centerOfMass;

    //Reifenvisualisierung
    public Transform flWheel;
    public Transform frWheel;
    public Transform rlWheel;
    public Transform rrWheel;
    //Fahrzeugsteuerung
    public WheelCollider flWheelCollider;
    public WheelCollider frWheelCollider;
    public WheelCollider rlWheelCollider;
    public WheelCollider rrWheelCollider;

    //Event 1
    public delegate void LastWaypointHandler(GameVehicleEventArgs e);

    public static LastWaypointHandler onLastWaypoint;
    
    void Awake() {
        m_currentMaxSpeed = maxSpeed;
        m_wheelRadius = flWheelCollider.radius;
        
        InitGearSpeeds();

        if (m_inverseWheelTurning) {
            m_wheelTurningParameter = -1;
        }
        else {
            m_wheelTurningParameter = 1;
        }

        
        if (playSound && motorSound != null) {
            InitSound();
        }
        
        if (centerOfMass != null) {
            rigidbody.centerOfMass = centerOfMass.localPosition;
        }
                
    }
    
    void InitSound() {

        m_motorAudioSource = gameObject.AddComponent<AudioSource>();
        m_motorAudioSource.clip = motorSound;
        m_motorAudioSource.loop = true;
        m_motorAudioSource.volume = soundVolume;
        m_motorAudioSource.playOnAwake = false;
        m_motorAudioSource.pitch = 0.1f;
        //motorAudioSource.rolloffMode = AudioRolloffMode.Linear;
        m_motorAudioSource.Play();

    }

    void FixedUpdate() {
        m_currentSpeed = (Mathf.PI * 2 * m_wheelRadius) * flWheelCollider.rpm * 60 / 1000;       
        m_currentSpeed = Mathf.Round(m_currentSpeed);
       
        flWheelCollider.motorTorque = torque * aiSpeedPedal;
        frWheelCollider.motorTorque = torque * aiSpeedPedal;        
        
        flWheelCollider.brakeTorque = brakeTorque * aiBrakePedal;       
        frWheelCollider.brakeTorque = brakeTorque * aiBrakePedal;       

        flWheelCollider.steerAngle = maxSteerAngle * aiSteerAngle;
        frWheelCollider.steerAngle = maxSteerAngle * aiSteerAngle;

        if (playSound && motorSound != null) {
            SetCurrentGear();
            GearSound();
        }
    }

    void Update() {
        
        if(GameConfigs.isUIRunning) {
            return;
        }

        RotateWheels();
        SteelWheels();        
    }

    void RotateWheels() {
        flWheel.Rotate(flWheelCollider.rpm / 60 * 360 * Time.deltaTime * m_wheelTurningParameter, 0, 0);
        frWheel.Rotate(frWheelCollider.rpm / 60 * 360 * Time.deltaTime * m_wheelTurningParameter, 0, 0);
        rlWheel.Rotate(rlWheelCollider.rpm / 60 * 360 * Time.deltaTime * m_wheelTurningParameter, 0, 0);
        rrWheel.Rotate(rrWheelCollider.rpm / 60 * 360 * Time.deltaTime * m_wheelTurningParameter, 0, 0);
    }

    void SteelWheels() {
        flWheel.localEulerAngles = new Vector3(flWheel.localEulerAngles.x, flWheelCollider.steerAngle - flWheel.localEulerAngles.z, flWheel.localEulerAngles.z);
        frWheel.localEulerAngles = new Vector3(frWheel.localEulerAngles.x, frWheelCollider.steerAngle - frWheel.localEulerAngles.z, frWheel.localEulerAngles.z);
    }

    void SetCurrentGear() {
        int gearNumber;
        //gearNumber = gearSpeed.length;
        gearNumber = m_gearSpeed.Count;
        m_currentGear = gearNumber - 1;
        for (var i = 0; i < gearNumber; i++) {
            if (m_gearSpeed[i] >= m_currentSpeed) {
                m_currentGear = i;
                break;
            }
        }
    }

    void GearSound() {
        float tempMinSpeed = 0.00f;
        float tempMaxSpeed = 0.00f;
        float currentPitch = 0.00f;

        switch (m_currentGear) {
        case 0:
            tempMinSpeed = 0.00f;
            tempMaxSpeed = m_gearSpeed[m_currentGear];
            break;

        default:
            tempMinSpeed = m_gearSpeed[m_currentGear - 1];
            tempMaxSpeed = m_gearSpeed[m_currentGear];
            break;
        }

        //currentPitch = (float)(((Mathf.Abs(currentSpeed) - tempMinSpeed) / (tempMaxSpeed - tempMinSpeed)) + 0.8);
        float delta = tempMaxSpeed - tempMinSpeed;
        currentPitch = (float)((((m_currentSpeed - tempMinSpeed) / delta)) / 2 + 0.8);

        //LogUtil.Log(currentPitch + ";" + currentSpeed + ";" + tempMinSpeed + ";" + delta +";" + currentGear);
        if (currentPitch > 2) {
            currentPitch = 2;
        }
        m_motorAudioSource.pitch = currentPitch;

    }

    void InitGearSpeeds() {
        int gearSpeedStep;

        if (gears < 1) {
            gears = 1;
        }

        gearSpeedStep = (int)Mathf.Round(maxSpeed / gears);
        m_gearSpeed.Clear();

        for (int i = 0; i < gears; i++) {
            m_gearSpeed.Add(gearSpeedStep * (i + 1));
        }

    }

}
