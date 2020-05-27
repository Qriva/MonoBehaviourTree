using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using MBT;

namespace MBTEditor
{
    [CustomEditor(typeof(Blackboard))]
    public class BlackboardEditor : Editor
    {
        readonly string[] varOptions = new string[]{"Delete"};
        SerializedProperty variables;
        GUIStyle popupStyle;
        string newVariableKey = "";
        string[] variableTypesNames = new string[0];
        Type[] variableTypes = new Type[0];
        int selectedVariableType = 0;
        Blackboard blackboard;

        void OnEnable()
        {
            variables = serializedObject.FindProperty("variables");
            blackboard = target as Blackboard;
            SetupVariableTypes();
        }

        private void SetupVariableTypes()
        {
            // Find all types
            IEnumerable<Type> enumerable = System.Reflection.Assembly.GetAssembly(typeof(BlackboardVariable)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(BlackboardVariable)));
            List<string> names = new List<string>();
            foreach (Type type in enumerable)
            {
                names.Add(type.Name);
            }
            variableTypesNames = names.ToArray();
            variableTypes = enumerable.ToArray();
        }

        public override void OnInspectorGUI()
        {
            // Init styles
            if (popupStyle == null) {
                popupStyle = new GUIStyle(GUI.skin.GetStyle("PaneOptions"));
                popupStyle.imagePosition = ImagePosition.ImageOnly;
                popupStyle.margin.top += 3;
            }

            // Fields used to add variables
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Key", GUILayout.MaxWidth(80));
                newVariableKey = EditorGUILayout.TextField(newVariableKey);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
                GUILayout.Label("Type", GUILayout.MaxWidth(80));
                selectedVariableType = EditorGUILayout.Popup(selectedVariableType, variableTypesNames);
                GUI.SetNextControlName("AddButton");
                if (GUILayout.Button("Add", EditorStyles.miniButton)) {
                    CreateVariableAndResetInput();
                }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            // DrawDefaultInspector();
            // EditorGUILayout.Space();

            // serializedObject.Update();
            // EditorGUI.BeginChangeCheck();

            SerializedProperty vars = variables.Copy();
            if (vars.isArray && Event.current.type != EventType.DragPerform) {
                for (int i = 0; i < vars.arraySize; i++)
                {
                    EditorGUI.BeginChangeCheck();
                    int popupOption = -1;
                    SerializedProperty serializedV = vars.GetArrayElementAtIndex(i);
                    SerializedObject serializedVariable = new SerializedObject(serializedV.objectReferenceValue);
                    EditorGUILayout.BeginHorizontal();
                        EditorGUILayout.PrefixLabel(serializedVariable.FindProperty("key").stringValue);
                        int v = EditorGUILayout.Popup(popupOption, varOptions, popupStyle, GUILayout.MaxWidth(20));
                        EditorGUILayout.PropertyField(serializedVariable.FindProperty("val"), GUIContent.none);
                    EditorGUILayout.EndHorizontal();

                    if (EditorGUI.EndChangeCheck()) {
                        serializedVariable.ApplyModifiedProperties();
                    }
                    // Delete on change
                    if (v != popupOption) {
                        DeleteVariabe(serializedV.objectReferenceValue as BlackboardVariable);
                    }
                }
            }

            // if (EditorGUI.EndChangeCheck()) {
            //     Debug.Log("tak");
                
            //     serializedObject.ApplyModifiedProperties();
            // }
        }

        private void DeleteVariabe(BlackboardVariable blackboardVariable)
        {
            Undo.RecordObject(blackboard, "Delete Blackboard Variable");
            blackboard.variables.Remove(blackboardVariable);
            Undo.DestroyObjectImmediate(blackboardVariable);
        }

        private void CreateVariableAndResetInput()
        {
            if (string.IsNullOrEmpty(newVariableKey)) {
                return;
            }
            string k = new string( newVariableKey.ToCharArray().Where(c => !Char.IsWhiteSpace(c)).ToArray() );
            // Check for key duplicates
            for (int i = 0; i < blackboard.variables.Count; i++)
            {
                if (blackboard.variables[i].key == k) {
                    Debug.LogWarning("Variable '"+k+"' already exists.");
                    return;
                }
            }
            // Add variable
            Undo.RecordObject(blackboard, "Create Blackboard Variable");
            BlackboardVariable var = Undo.AddComponent(blackboard.gameObject, variableTypes[selectedVariableType]) as BlackboardVariable;
            var.key = k;
            blackboard.variables.Add(var);
            // Reset field
            newVariableKey = "";
            GUI.FocusControl("Clear");
        }
    }
}
