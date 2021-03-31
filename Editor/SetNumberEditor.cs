using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MBT;

namespace MBTEditor
{
    [CustomEditor(typeof(SetNumber))]
    public class SetNumberEditor : Editor
    {
        SerializedProperty titleProp;
        SerializedProperty typeProp;
        SerializedProperty operationProp;
        SerializedProperty sourceIntProp;
        SerializedProperty sourceFloatProp;
        SerializedProperty destinationFloatProp;
        SerializedProperty destinationIntProp;

        void OnEnable()
        {
            titleProp = serializedObject.FindProperty("title");
            typeProp = serializedObject.FindProperty("type");
            operationProp = serializedObject.FindProperty("operation");
            sourceFloatProp = serializedObject.FindProperty("sourceFloat");
            sourceIntProp = serializedObject.FindProperty("sourceInt");
            destinationFloatProp = serializedObject.FindProperty("destinationFloat");
            destinationIntProp = serializedObject.FindProperty("destinationInt");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(titleProp);
            EditorGUILayout.PropertyField(typeProp);
            EditorGUILayout.Space();
            if (typeProp.enumValueIndex == (int)SetNumber.Type.Float)
            {
                // Float
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(destinationFloatProp, GUIContent.none);
                    EditorGUILayout.PropertyField(operationProp, GUIContent.none, GUILayout.MaxWidth(60f));
                    EditorGUILayout.PropertyField(sourceFloatProp, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                // Int
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(destinationIntProp, GUIContent.none);
                    EditorGUILayout.PropertyField(operationProp, GUIContent.none, GUILayout.MaxWidth(60f));
                    EditorGUILayout.PropertyField(sourceIntProp, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
