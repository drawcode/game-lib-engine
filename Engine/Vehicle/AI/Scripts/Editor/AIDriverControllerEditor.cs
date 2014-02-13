using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[CustomEditor(typeof(GameVehicleAIDriver))]
public class GameVehicleAIDriverControllerEditor : Editor
{
    SerializedObject m_soTarget;
    //SerializedProperty m_soCalcMaxSpeed;
    //SerializedProperty m_soTorque;
    //SerializedProperty m_soBrakeTorque;
    //SerializedProperty m_soSteerAngle;
    SerializedProperty m_soHsSteerAngle;
    SerializedProperty m_soSteeringSpeed;
    SerializedProperty m_soGears;
    //SerializedProperty m_soPlaySound;
    //SerializedProperty m_soMotorSound;
    //SerializedProperty m_soSoundVolume;
    SerializedProperty m_soDriveMode;
    SerializedProperty m_soUseObAvoidance; 
    SerializedProperty m_soCenterOfMassY;
    SerializedProperty m_soVisibleLayers;
    SerializedProperty m_soRoadMaxWidth;
    SerializedProperty m_soObAvoidDistance;	
    SerializedProperty m_soObAvoidWidth;
    SerializedProperty m_soObAvoidSideDistance;
    SerializedProperty m_soSteeringMode;    

    void OnEnable()
    {
        m_soTarget = new SerializedObject(target);

        //m_soCalcMaxSpeed = m_soTarget.FindProperty("calcMaxSpeed");
        //m_soTorque = m_soTarget.FindProperty("torque");
        //m_soBrakeTorque = m_soTarget.FindProperty("brakeTorque");
        //m_soSteerAngle = m_soTarget.FindProperty("steerAngle");
        m_soHsSteerAngle = m_soTarget.FindProperty("hsSteerAngle");
        m_soSteeringSpeed = m_soTarget.FindProperty("steeringSpeed");
        m_soGears = m_soTarget.FindProperty("gears");
        //m_soPlaySound = m_soTarget.FindProperty("playSound");
        //m_soMotorSound = m_soTarget.FindProperty("motorSound");
        //m_soSoundVolume = m_soTarget.FindProperty("soundVolume");
        m_soDriveMode = m_soTarget.FindProperty("driveMode");
        m_soCenterOfMassY = m_soTarget.FindProperty("centerOfMassY");
        m_soUseObAvoidance = m_soTarget.FindProperty("useObstacleAvoidance");
        m_soObAvoidDistance = m_soTarget.FindProperty("oADistance");		
        m_soObAvoidWidth = m_soTarget.FindProperty("oAWidth");
        m_soObAvoidSideDistance = m_soTarget.FindProperty("oASideDistance");
        m_soSteeringMode = m_soTarget.FindProperty("steeringMode");
        m_soRoadMaxWidth = m_soTarget.FindProperty("roadMaxWidth");
        m_soVisibleLayers = m_soTarget.FindProperty("visibleLayers");
       
    }
        
    public override void OnInspectorGUI()
    {
        m_soTarget.Update();
        
        //EditorGUILayout.PropertyField(m_soCalcMaxSpeed);     
        //EditorGUILayout.PropertyField(m_soTorque);
        //EditorGUILayout.PropertyField(m_soBrakeTorque);
        //EditorGUILayout.PropertyField(m_soSteerAngle);
        EditorGUILayout.PropertyField(m_soHsSteerAngle);
        EditorGUILayout.PropertyField(m_soSteeringSpeed);
        EditorGUILayout.PropertyField(m_soGears);
        //EditorGUILayout.PropertyField(m_soPlaySound);
        //EditorGUILayout.PropertyField(m_soMotorSound);
        //EditorGUILayout.PropertyField(m_soSoundVolume);
        EditorGUILayout.PropertyField(m_soDriveMode);
        EditorGUILayout.PropertyField(m_soCenterOfMassY);
        EditorGUILayout.PropertyField(m_soUseObAvoidance);
        EditorGUILayout.PropertyField(m_soObAvoidDistance);			
        EditorGUILayout.PropertyField(m_soObAvoidWidth);
        EditorGUILayout.PropertyField(m_soObAvoidSideDistance);
        EditorGUILayout.PropertyField(m_soSteeringMode);       
        EditorGUILayout.PropertyField(m_soRoadMaxWidth);
        EditorGUILayout.PropertyField(m_soVisibleLayers);
        m_soTarget.ApplyModifiedProperties();
    }
	
}
