using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MonoBT
{
    [CustomEditor(typeof(MonoBehaviourTree))]
    public class MonoBehaviourTreeEditor : Editor
    {
        private GUIStyle boxStyle;
        
        void InitStyle()
        {
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(EditorStyles.helpBox);
            }
        }

        public override void OnInspectorGUI()
        {
            InitStyle();

            DrawDefaultInspector();

            if (GUILayout.Button("Open editor")) {
                BehaviourTreeWindow.OpenEditor();
            }

            EditorGUILayout.Space();

            MonoBehaviourTree mbt = ((MonoBehaviourTree) target);
            if (mbt.selectedEditorNode != null)
            {
                EditorGUILayout.LabelField("Node inspector", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal(boxStyle);
                    GUILayout.Space(10);
                    EditorGUILayout.BeginVertical();
                        Editor.CreateEditor(mbt.selectedEditorNode).OnInspectorGUI();
                    EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
