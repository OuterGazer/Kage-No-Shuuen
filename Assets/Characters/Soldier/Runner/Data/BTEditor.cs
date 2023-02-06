using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using BehaviourTree;

[CustomEditor(typeof(SoldierRunnerBT))]
public class BTEditor : Editor
{
    SoldierRunnerBT t;
    SerializedProperty root;

    List<SerializedProperty> children;

    private void OnEnable()
    {
        t = (SoldierRunnerBT)target;
        root = serializedObject.FindProperty("root");
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        //root.objectReferenceValue = EditorGUILayout.ObjectField(root.objectReferenceValue, typeof(Node), true);

        //serializedObject.ApplyModifiedProperties();
        //foreach(Node item in root)
        //{
        //    SerializedObject temp = item.GetType();
        //    children.Add(item);
        //}
        //myElement.objectReferenceValue = EditorGUILayout.ObjectField(myElement.objectReferenceValue, typeof(SomeType), true);
    }
}
