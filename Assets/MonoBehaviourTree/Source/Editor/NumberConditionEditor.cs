using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MBT;

namespace MBTEditor
{
    [CustomEditor(typeof(NumberCondition))]
    public class NumberConditionEditor : Editor
    {
        SerializedProperty titleProp;
        SerializedProperty abortProp;
        SerializedProperty floatReferenceProp;
        SerializedProperty intReferenceProp;
        SerializedProperty floatReference2Prop;
        SerializedProperty intReference2Prop;
        SerializedProperty typeProp;
        SerializedProperty comparatorProp;

        void OnEnable()
        {
            titleProp = serializedObject.FindProperty("title");
            floatReferenceProp = serializedObject.FindProperty("floatReference");
            intReferenceProp = serializedObject.FindProperty("intReference");
            floatReference2Prop = serializedObject.FindProperty("floatReference2");
            intReference2Prop = serializedObject.FindProperty("intReference2");
            abortProp = serializedObject.FindProperty("abort");
            typeProp = serializedObject.FindProperty("type");
            comparatorProp = serializedObject.FindProperty("comparator");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(titleProp);
            EditorGUILayout.PropertyField(abortProp);
            EditorGUILayout.PropertyField(typeProp);
            EditorGUILayout.Space();
            // GUILayout.Label("Condition");
            if (typeProp.enumValueIndex == (int)NumberCondition.Type.Float)
            {
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(floatReferenceProp, GUIContent.none);
                    EditorGUILayout.PropertyField(comparatorProp, GUIContent.none, GUILayout.MaxWidth(60f));
                    EditorGUILayout.PropertyField(floatReference2Prop, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.PropertyField(intReferenceProp, GUIContent.none);
                    EditorGUILayout.PropertyField(comparatorProp, GUIContent.none, GUILayout.MaxWidth(60f));
                    EditorGUILayout.PropertyField(intReference2Prop, GUIContent.none);
                EditorGUILayout.EndHorizontal();
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
