using UnityEngine;
using System.Collections;
using System;
using UnityEditor;

[CustomEditor(typeof(GameVehicleRespawn))]
public class GameVehicleRespawnEditor : Editor
{
    SerializedObject m_soTarget;
    SerializedProperty m_soTimeTillRespawn;

    void OnEnable()
    {
        m_soTarget = new SerializedObject(target);
        m_soTimeTillRespawn = m_soTarget.FindProperty("timeTillRespawn");
    }

    void OnSceneGUI()
    {

    }

    public override void OnInspectorGUI()
    {
        m_soTarget.Update();

        EditorGUILayout.PropertyField(m_soTimeTillRespawn);

        m_soTarget.ApplyModifiedProperties();
    }
}
