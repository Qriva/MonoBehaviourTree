using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MBT;

namespace MBTEditor
{
    [CustomEditor(typeof(IsSetCondition))]
    public class IsSetConditionEditor : Editor
    {
        SerializedProperty abortProp;
        SerializedProperty boolReferenceProp;
        SerializedProperty objectReferenceProp;
        SerializedProperty transformReferenceProp;
        SerializedProperty typeProp;
        SerializedProperty invertProp;

        void OnEnable()
        {
            boolReferenceProp = serializedObject.FindProperty("boolReference");
            objectReferenceProp = serializedObject.FindProperty("objectReference");
            transformReferenceProp = serializedObject.FindProperty("transformReference");
            abortProp = serializedObject.FindProperty("abort");
            typeProp = serializedObject.FindProperty("type");
            invertProp = serializedObject.FindProperty("invert");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(abortProp);
            EditorGUILayout.PropertyField(invertProp);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(typeProp);
            if (typeProp.enumValueIndex == (int)IsSetCondition.Type.Boolean)
            {
                EditorGUILayout.PropertyField(boolReferenceProp, new GUIContent("Variable"));
            }
            else if (typeProp.enumValueIndex == (int)IsSetCondition.Type.Object)
            {
                EditorGUILayout.PropertyField(objectReferenceProp, new GUIContent("Variable"));
            }
            else
            {
                EditorGUILayout.PropertyField(transformReferenceProp, new GUIContent("Variable"));
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
