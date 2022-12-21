using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ObjectBinder))]
public class ObjectBinderEditor : Editor
{
    ObjectBinder t;
    SerializedProperty objectToBindToProp;
    SerializedProperty localPositionOffsetProp;
    SerializedProperty localRotationOffsetProp;


    private void OnEnable()
    {
        t = (ObjectBinder)target;
        objectToBindToProp = serializedObject.FindProperty("objectToBindTo");
        localPositionOffsetProp = serializedObject.FindProperty("localPositionOffset");
        localRotationOffsetProp = serializedObject.FindProperty("localRotationOffset");
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.PropertyField(objectToBindToProp);
        GUI.enabled = false;
        EditorGUILayout.PropertyField(localPositionOffsetProp);
        EditorGUILayout.PropertyField(localRotationOffsetProp);
        GUI.enabled = true;

        if (GUILayout.Button("Calculate Bind Offsets"))
        { CalculateBindOffsets(); }

        serializedObject.ApplyModifiedProperties();
    }

    private void CalculateBindOffsets()
    {
        Transform objectToBindTo = (Transform)objectToBindToProp.objectReferenceValue;
        localPositionOffsetProp.vector3Value = objectToBindTo.InverseTransformDirection(t.transform.position - objectToBindTo.position);
        localRotationOffsetProp.quaternionValue = QuaternionDiff(objectToBindTo.rotation, t.transform.rotation);
    }


    // https://stackoverflow.com/questions/22157435/difference-between-the-two-quaternions
    // a * diff = b
    private Quaternion QuaternionDiff(Quaternion a, Quaternion b)
    {
        return Quaternion.Inverse(a) * b;
    }
}
