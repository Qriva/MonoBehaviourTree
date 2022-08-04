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
        private GUIStyle foldStyle;
        private Editor nodeEditor;
        
        void InitStyle()
        {
            if (foldStyle == null)
            {
                boxStyle = new GUIStyle(EditorStyles.helpBox);
                foldStyle = new GUIStyle(EditorStyles.foldoutHeader);
                foldStyle.onNormal = foldStyle.onFocused;
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
            InitStyle();

            DrawDefaultInspector();
            GUILayout.Space(5);

            if (GUILayout.Button("Open editor")) {
                BehaviourTreeWindow.OpenEditor();
            }

            EditorGUILayout.Space();
            
            MonoBehaviourTree mbt = ((MonoBehaviourTree) target);
            bool renderNodeInspector = mbt.selectedEditorNode != null;

            EditorGUILayout.BeginFoldoutHeaderGroup(renderNodeInspector, "Node inspector", foldStyle);
                EditorGUILayout.Space(1);
                if (renderNodeInspector)
                {
                    EditorGUILayout.BeginHorizontal(boxStyle);
                        GUILayout.Space(3);
                        EditorGUILayout.BeginVertical();
                            GUILayout.Space(5);
                            Editor.CreateCachedEditor(mbt.selectedEditorNode, null, ref nodeEditor);
                            nodeEditor.OnInspectorGUI();
                            GUILayout.Space(5);
                        EditorGUILayout.EndVertical();
                    EditorGUILayout.EndHorizontal();
                }
            EditorGUILayout.EndFoldoutHeaderGroup();
            EditorGUILayout.Space();
        }
    }
}
