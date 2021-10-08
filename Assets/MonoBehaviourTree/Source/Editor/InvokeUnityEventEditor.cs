using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MBT;
using System;

namespace MBTEditor
{
    [CustomEditor(typeof(InvokeUnityEvent))]
    public class InvokeUnityEventEditor : Editor
    {
        SerializedProperty titleProp;
        SerializedProperty typeProp;

        private SerializedProperty transformEventProp;
        private SerializedProperty gameObjectEventProp;
        private SerializedProperty floatEventProp;
        private SerializedProperty intEventProp;
        private SerializedProperty boolEventProp;
        private SerializedProperty stringEventProp;
        private SerializedProperty vector3EventProp;
        private SerializedProperty vector2EventProp;
        
        private SerializedProperty transformReferenceProp;
        private SerializedProperty gameObjectReferenceProp;
        private SerializedProperty floatReferenceProp;
        private SerializedProperty intReferenceProp;
        private SerializedProperty boolReferenceProp;
        private SerializedProperty stringReferenceProp;
        private SerializedProperty vector3ReferenceProp;
        private SerializedProperty vector2ReferenceProp;

        void OnEnable()
        {
            titleProp = serializedObject.FindProperty("title");
            typeProp = serializedObject.FindProperty("type");
            
            transformEventProp = serializedObject.FindProperty("transformEvent");
            gameObjectEventProp = serializedObject.FindProperty("gameObjectEvent");
            floatEventProp = serializedObject.FindProperty("floatEvent");
            intEventProp = serializedObject.FindProperty("intEvent");
            boolEventProp = serializedObject.FindProperty("boolEvent");
            stringEventProp = serializedObject.FindProperty("stringEvent");
            vector3EventProp = serializedObject.FindProperty("vector3Event");
            vector2EventProp = serializedObject.FindProperty("vector2Event");

            transformReferenceProp = serializedObject.FindProperty("transformReference");
            gameObjectReferenceProp = serializedObject.FindProperty("gameObjectReference");
            floatReferenceProp = serializedObject.FindProperty("floatReference");
            intReferenceProp = serializedObject.FindProperty("intReference");
            boolReferenceProp = serializedObject.FindProperty("boolReference");
            stringReferenceProp = serializedObject.FindProperty("stringReference");
            vector3ReferenceProp = serializedObject.FindProperty("vector3Reference");
            vector2ReferenceProp = serializedObject.FindProperty("vector2Reference");
        }

        private static readonly GUIContent variableLabel = new GUIContent("Parameter");
        private static readonly GUIContent eventLabel = new GUIContent("Event");

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(titleProp);
            EditorGUILayout.PropertyField(typeProp);
            EditorGUILayout.Space();

            if (GetSerializedProperties(out SerializedProperty eventProp, out SerializedProperty variableProp))
            {
                EditorGUILayout.PropertyField(variableProp, variableLabel);
                EditorGUILayout.PropertyField(eventProp, eventLabel);
            }

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private bool GetSerializedProperties(out SerializedProperty eventProp, out SerializedProperty referenceProp)
        {
            InvokeUnityEvent.EventType eventType = (InvokeUnityEvent.EventType)typeProp.enumValueIndex;
            switch (eventType)
            {
                case InvokeUnityEvent.EventType.Transform:
                    eventProp = transformEventProp;
                    referenceProp = transformReferenceProp;
                    return true;
                case InvokeUnityEvent.EventType.Float:
                    eventProp = floatEventProp;
                    referenceProp = floatReferenceProp;
                    return true;
                case InvokeUnityEvent.EventType.GameObject:
                    eventProp = gameObjectEventProp;
                    referenceProp = gameObjectReferenceProp;
                    return true;
                case InvokeUnityEvent.EventType.Int:
                    eventProp = intEventProp;
                    referenceProp = intReferenceProp;
                    return true;
                case InvokeUnityEvent.EventType.String:
                    eventProp = stringEventProp;
                    referenceProp = stringReferenceProp;
                    return true;
                case InvokeUnityEvent.EventType.Vector3:
                    eventProp = vector3EventProp;
                    referenceProp = vector3ReferenceProp;
                    return true;
                case InvokeUnityEvent.EventType.Vector2:
                    eventProp = vector2EventProp;
                    referenceProp = vector2ReferenceProp;
                    return true;
                case InvokeUnityEvent.EventType.Bool:
                    eventProp = boolEventProp;
                    referenceProp = boolReferenceProp;
                    return true;
                default:
                    eventProp = null;
                    referenceProp = null;
                    return false;
            }
        }
    }
}
