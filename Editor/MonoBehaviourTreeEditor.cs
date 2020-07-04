using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using MBT;

namespace MBTEditor
{
    [CustomEditor(typeof(MonoBehaviourTree))]
    public class MonoBehaviourTreeEditor : Editor
    {
        private GUIStyle boxStyle;
        private Editor nodeEditor;
        
        void InitStyle()
        {
            if (boxStyle == null)
            {
                boxStyle = new GUIStyle(EditorStyles.helpBox);
            }
        }

        void OnEnable()
        {
            // Set hide flags in case object was duplicated or turned into prefab
            if (target == null)
            {
                return;
            }
            MonoBehaviourTree mbt = (MonoBehaviourTree) target;
            // Sample one component and check if its hidden. Hide all nodes if sample is visible.
            if (mbt.TryGetComponent<Node>(out Node n) && n.hideFlags != HideFlags.HideInInspector)
            {
                Node[] nodes = mbt.GetComponents<Node>();
                for (int i = 0; i < nodes.Length; i++)
                {
                    nodes[i].hideFlags = HideFlags.HideInInspector;
                }
            }
        }

        void OnDisable()
        {
            // Destroy editor if there is any
            if (nodeEditor != null)
            {
                DestroyImmediate(nodeEditor);
            }
        }

        public override void OnInspectorGUI()
        {
            // Destroy previous editor
            if (nodeEditor != null)
            {
                DestroyImmediate(nodeEditor);
            }

            InitStyle();

            DrawDefaultInspector();
            GUILayout.Space(5);

            if (GUILayout.Button("Open editor")) {
                BehaviourTreeWindow.OpenEditor();
            }

            EditorGUILayout.Space();

            MonoBehaviourTree mbt = ((MonoBehaviourTree) target);
            if (mbt.selectedEditorNode != null)
            {
                EditorGUILayout.LabelField("Node inspector", EditorStyles.boldLabel);
                
                EditorGUILayout.BeginHorizontal(boxStyle);
                    GUILayout.Space(3);
                    EditorGUILayout.BeginVertical();
                        GUILayout.Space(5);
                        nodeEditor = Editor.CreateEditor(mbt.selectedEditorNode);
                        nodeEditor.OnInspectorGUI();
                        GUILayout.Space(5);
                    EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
