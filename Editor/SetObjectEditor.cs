using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MBT;

namespace MBTEditor
{
    [CustomEditor(typeof(SetObject))]
    public class SetObjectEditor : Editor
    {
        SerializedProperty titleProp;
        SerializedProperty typeProp;
        SerializedProperty sourceTransformProp;
        SerializedProperty sourceGameObjectProp;
        SerializedProperty destinationTransformProp;
        SerializedProperty destinationGameObjectProp;

        private static readonly GUIContent destinationLabel = new GUIContent("Destination");
        private static readonly GUIContent sourceLabel = new GUIContent("Source");

        void OnEnable()
        {
            titleProp = serializedObject.FindProperty("title");
            typeProp = serializedObject.FindProperty("type");
            sourceGameObjectProp = serializedObject.FindProperty("sourceGameObject");
            sourceTransformProp = serializedObject.FindProperty("sourceTransform");
            destinationGameObjectProp = serializedObject.FindProperty("destinationGameObject");
            destinationTransformProp = serializedObject.FindProperty("destinationTransform");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(titleProp);
            EditorGUILayout.PropertyField(typeProp);
            EditorGUILayout.Space();

            const int transformIndex = 0;
            if (typeProp.enumValueIndex == transformIndex)
            {
                // Transform
                EditorGUILayout.PropertyField(destinationTransformProp, destinationLabel);
                EditorGUILayout.PropertyField(sourceTransformProp, sourceLabel);
            }
            else
            {
                // GameObject
                EditorGUILayout.PropertyField(destinationGameObjectProp, destinationLabel);
                EditorGUILayout.PropertyField(sourceGameObjectProp, sourceLabel);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
