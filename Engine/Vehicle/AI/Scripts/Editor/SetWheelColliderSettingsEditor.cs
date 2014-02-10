using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[CustomEditor(typeof(SetWheelColliderSettings))]
public class SetWheelColliderSettingsEditor : Editor
{    
    SerializedObject m_soTarget;   
    SerializedProperty m_soRadiusFront;
    SerializedProperty m_soRadiusBack;
    SerializedProperty m_soMirrorWheels;

    void OnEnable()
    {
        m_soTarget = new SerializedObject(target);
        m_soRadiusFront = m_soTarget.FindProperty("radiusFront");
        m_soRadiusBack = m_soTarget.FindProperty("radiusBack");
        m_soMirrorWheels = m_soTarget.FindProperty("mirrorWheels");
    }

    void OnSceneGUI()
    {               

    }

    public override void OnInspectorGUI()
    {        
        m_soTarget.Update();
        EditorGUILayout.PropertyField(m_soRadiusFront);
        EditorGUILayout.PropertyField(m_soRadiusBack);
        EditorGUILayout.PropertyField(m_soMirrorWheels);
        m_soTarget.ApplyModifiedProperties();        
    }


}
