using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MBT;

namespace MBTEditor
{
    [CustomEditor(typeof(SetVector))]
    public class SetVectorEditor : Editor
    {
        SerializedProperty titleProp;
        SerializedProperty typeProp;
        SerializedProperty sourceVector2Prop;
        SerializedProperty sourceVector3Prop;
        SerializedProperty destinationVector2Prop;
        SerializedProperty destinationVector3Prop;

        private static readonly GUIContent destinationLabel = new GUIContent("Destination");
        private static readonly GUIContent sourceLabel = new GUIContent("Source");

        void OnEnable()
        {
            titleProp = serializedObject.FindProperty("title");
            typeProp = serializedObject.FindProperty("type");
            sourceVector3Prop = serializedObject.FindProperty("sourceVector3");
            sourceVector2Prop = serializedObject.FindProperty("sourceVector2");
            destinationVector3Prop = serializedObject.FindProperty("destinationVector3");
            destinationVector2Prop = serializedObject.FindProperty("destinationVector2");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(titleProp);
            EditorGUILayout.PropertyField(typeProp);
            EditorGUILayout.Space();

            const int vector3Index = 1;
            if (typeProp.enumValueIndex == vector3Index)
            {
                // Vector3
                // EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(destinationVector3Prop, destinationLabel);
                    EditorGUILayout.PropertyField(sourceVector3Prop, sourceLabel);
                // EditorGUILayout.EndHorizontal();
            }
            else
            {
                // Vector2
                // EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(destinationVector2Prop, destinationLabel);
                    EditorGUILayout.PropertyField(sourceVector2Prop, sourceLabel);
                // EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
