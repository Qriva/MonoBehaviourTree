using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using MBT;

namespace MBTEditor
{    
    public class BehaviourTreeWindow : EditorWindow
    {
        private MonoBehaviourTree currentMBT;
        private Editor currentMBTEditor;
        private Node[] currentNodes;
        private Node selectedNode;
        private bool nodeMoved = false;
        private Vector2 workspaceOffset;
        private NodeHandle currentHandle;
        private NodeHandle dropdownHandleCache;
        private bool snapNodesToGrid;

        private Rect nodeFinderActivatorRect;
        private NodeDropdown nodeDropdown;
        private Vector2 nodeDropdownTargetPosition;

        private readonly float _handleDetectionDistance = 8f;
        private readonly Color _editorBackgroundColor = new Color(0.16f, 0.19f, 0.25f, 1);
        private GUIStyle _defaultNodeStyle;
        private GUIStyle _selectedNodeStyle;
        private GUIStyle _successNodeStyle;
        private GUIStyle _failureNodeStyle;
        private GUIStyle _runningNodeStyle;
        private GUIStyle _nodeContentBoxStyle;
        private GUIStyle _nodeLabelStyle;
        private GUIStyle _nodeBreakpointLabelStyle;

        private void OnEnable()
        {
            // Read snap option
            snapNodesToGrid = EditorPrefs.GetBool("snapNodesToGrid", true);
            // Setup events
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
            Undo.undoRedoPerformed -= UpdateSelection;
            Undo.undoRedoPerformed += UpdateSelection;
            // Node finder
            nodeDropdown = new NodeDropdown(new AdvancedDropdownState(), AddNode);
            // Standard node
            _defaultNodeStyle = new GUIStyle();
            _defaultNodeStyle.border = new RectOffset(10,10,10,10);
            _defaultNodeStyle.normal.background = Resources.Load("mbt_node_default", typeof(Texture2D)) as Texture2D;
            // Selected node
            _selectedNodeStyle = new GUIStyle();
            _selectedNodeStyle.border = new RectOffset(10,10,10,10);
            _selectedNodeStyle.normal.background = Resources.Load("mbt_node_selected", typeof(Texture2D)) as Texture2D;
            // Success node
            _successNodeStyle = new GUIStyle();
            _successNodeStyle.border = new RectOffset(10,10,10,10);
            _successNodeStyle.normal.background = Resources.Load("mbt_node_success", typeof(Texture2D)) as Texture2D;
            // Failure node
            _failureNodeStyle = new GUIStyle();
            _failureNodeStyle.border = new RectOffset(10,10,10,10);
            _failureNodeStyle.normal.background = Resources.Load("mbt_node_failure", typeof(Texture2D)) as Texture2D;
            // Running node
            _runningNodeStyle = new GUIStyle();
            _runningNodeStyle.border = new RectOffset(10,10,10,10);
            _runningNodeStyle.normal.background = Resources.Load("mbt_node_running", typeof(Texture2D)) as Texture2D;
            // Node content box
            _nodeContentBoxStyle = new GUIStyle();
            _nodeContentBoxStyle.padding = new RectOffset(0,0,15,15);
            // Node label
            _nodeLabelStyle = new GUIStyle();
            _nodeLabelStyle.normal.textColor = Color.white;
            _nodeLabelStyle.alignment = TextAnchor.MiddleCenter;
            _nodeLabelStyle.wordWrap = true;
            _nodeLabelStyle.margin = new RectOffset(10,10,10,10);
            _nodeLabelStyle.font = Resources.Load("mbt_Lato-Regular", typeof(Font)) as Font;
            _nodeLabelStyle.fontSize = 12;
            // Node label when breakpoint is set to true
            _nodeBreakpointLabelStyle = new GUIStyle(_nodeLabelStyle);
            _nodeBreakpointLabelStyle.normal.textColor = new Color(1f, 0.35f, 0.18f);
        }
    
        private void OnDisable()
        {
            EditorApplication.playModeStateChanged -= OnPlayModeStateChanged;
            Undo.undoRedoPerformed -= UpdateSelection;
        }

        [MenuItem("Window/Mono Behaviour Tree")]
        public static void OpenEditor()
        {
            BehaviourTreeWindow window = GetWindow<BehaviourTreeWindow>();
            window.titleContent = new GUIContent(
                "Behaviour Tree",
                Resources.Load("mbt_window_icon", typeof(Texture2D)) as Texture2D
            );
        }

        void OnGUI()
        {
            // Draw grid in background first
            PaintBackground();

            // If there is no tree to render just skip rest
            if (currentMBT == null) {
                // Keep toolbar rendered
                PaintWindowToolbar();
                return;
            }

            PaintConnections(Event.current);

            // Repaint nodes
            PaintNodes();

            PaintWindowToolbar();

            // Update selection and drag
            ProcessEvents(Event.current);

            if (GUI.changed) Repaint();
        }

        private void PaintConnections(Event e)
        {
            // Paint line when dragging connection
            if (currentHandle != null) {
                Handles.BeginGUI();
                Vector3 p1 = new Vector3(currentHandle.position.x, currentHandle.position.y, 0f);
                Vector3 p2 = new Vector3(e.mousePosition.x, e.mousePosition.y, 0f);
                Handles.DrawBezier(p1, p2, p1, p2, new Color(0.3f, 0.36f, 0.5f), null, 4f);
                Handles.EndGUI();
            }
            // Paint all current connections
            for (int i = 0; i < currentNodes.Length; i++)
            {
                Node n = currentNodes[i];
                Vector3 p1 = GetBottomHandlePosition(n.rect) + workspaceOffset;
                for (int j = 0; j < n.children.Count; j++)
                {
                    Handles.BeginGUI();
                    Vector3 p2 = GetTopHandlePosition(n.children[j].rect) + workspaceOffset;
                    Handles.DrawBezier(p1, p2, p1, p2, new Color(0.3f, 0.36f, 0.5f), null, 4f);
                    Handles.EndGUI();   
                }
            }
        }

        private void PaintWindowToolbar()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
                EditorGUI.BeginDisabledGroup(currentMBT == null);
                if (GUILayout.Toggle(snapNodesToGrid, "Snap Nodes", EditorStyles.toolbarButton) != snapNodesToGrid)
                {
                    snapNodesToGrid = !snapNodesToGrid;
                    // Store this setting
                    EditorPrefs.SetBool("snapNodesToGrid", snapNodesToGrid);
                }
                if (GUILayout.Button("Auto Layout", EditorStyles.toolbarButton)){
                    Debug.Log("Auto layout is not implemented.");
                }
                EditorGUILayout.Space();
                if (GUILayout.Button("Focus Root", EditorStyles.toolbarButton)){
                    FocusRoot();
                }
                if (GUILayout.Button("Add Node", EditorStyles.toolbarDropDown)){
                    OpenNodeFinder(nodeFinderActivatorRect, false);
                }
                if (Event.current.type == EventType.Repaint) nodeFinderActivatorRect = GUILayoutUtility.GetLastRect();
                EditorGUI.EndDisabledGroup();
                GUILayout.FlexibleSpace();
                GUILayout.Label((-workspaceOffset).ToString());
            EditorGUILayout.EndHorizontal();
        }

        void FocusRoot()
        {
            Root rootNode = null;
            for (int i = 0; i < currentNodes.Length; i++)
            {
                if (currentNodes[i] is Root) {
                    rootNode = currentNodes[i] as Root;
                    break;
                }
            }
            if (rootNode != null) {
                workspaceOffset = -rootNode.rect.center + new Vector2(this.position.width/2, this.position.height/2);
            }
        }

        private void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            UpdateSelection();
            Repaint();
        }

        void OnInspectorUpdate()
        {
            // OPTIMIZE: This can be optimized to call repaint once per second
            Repaint();
        }

        void OnSelectionChange() 
        {
            // Reset workspace position
            workspaceOffset = Vector2.zero;
            UpdateSelection();
            Repaint();
        }

        void OnFocus() 
        {
            UpdateSelection();
            Repaint();
        }

        void OnLostFocus()
        {
            // Debug.Log("lost focus");
        }

        private void UpdateSelection()
        {
            if (Selection.activeGameObject == null)
            {
                currentMBT = null;
                DestroyImmediate(currentMBTEditor);
                currentNodes = new Node[0];
                return;
            }
            // Get tree and list of nodes
            currentMBT = Selection.activeGameObject.GetComponent<MonoBehaviourTree>();
            currentMBTEditor = Editor.CreateEditor(currentMBT);
            if (currentMBT != null) {
                currentNodes = currentMBT.GetComponents<Node>();
            } else {
                currentNodes = new Node[0];
            }
            
        }

        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0) {
                        // Reset flag
                        nodeMoved = false;
                        // Frist check if any node handle was clicked
                        NodeHandle handle = FindHandle(e.mousePosition);
                        if (handle != null)
                        {
                            currentHandle = handle;
                            e.Use();
                            break;
                        }
                        Node node = FindNode(e.mousePosition);
                        // Select node if contains point
                        if (node != null) {
                            DeselectNode();
                            SelectNode(node);
                            if (e.clickCount == 2 && node is SubTree) {
                                SubTree subTree = node as SubTree;
                                if (subTree.tree != null) {
                                    Selection.activeGameObject = subTree.tree.gameObject;
                                }
                            }
                        } else {
                            DeselectNode();
                        }
                        e.Use();
                    } else if (e.button == 1) {
                        Node node = FindNode(e.mousePosition);
                        // Open proper context menu
                        if (node != null) {
                            OpenNodeMenu(e.mousePosition, node);
                        } else {
                            DeselectNode();
                            OpenNodeFinder(new Rect(e.mousePosition.x, e.mousePosition.y, 1, 1));
                        }
                        e.Use();
                    }
                    break;
                case EventType.MouseDrag:
                    // Drag node, workspace or connection
                    if (e.button == 0) {
                        if (currentHandle != null) {
                            // Let PaintConnections draw lines
                        } else if (selectedNode != null) {
                            Undo.RecordObject(selectedNode, "Move Node");
                            selectedNode.rect.position += Event.current.delta;
                            // Move whole branch when Ctrl is pressed
                            if (e.control) {
                                List<Node> movedNodes = selectedNode.GetAllSuccessors();
                                for (int i = 0; i < movedNodes.Count; i++)
                                {
                                    Undo.RecordObject(movedNodes[i], "Move Node");
                                    movedNodes[i].rect.position += Event.current.delta;
                                }
                            }
                            nodeMoved = true;
                        } else {
                            workspaceOffset += Event.current.delta;
                        }
                        GUI.changed = true;
                        e.Use();
                    }
                    break;
                case EventType.MouseUp:
                    if (currentHandle != null) {
                        TryConnectNodes(currentHandle, e.mousePosition);
                    }
                    // Reorder or snap nodes in case any of them was moved
                    if (nodeMoved && selectedNode != null) {
                        // Snap nodes if option is enabled
                        if (snapNodesToGrid)
                        {
                            Undo.RecordObject(selectedNode, "Move Node");
                            selectedNode.rect.position = SnapPositionToGrid(selectedNode.rect.position);
                            // When control is pressed snap successors too
                            if (e.control) {
                                List<Node> movedNodes = selectedNode.GetAllSuccessors();
                                for (int i = 0; i < movedNodes.Count; i++)
                                {
                                    Undo.RecordObject(movedNodes[i], "Move Node");
                                    movedNodes[i].rect.position = SnapPositionToGrid(movedNodes[i].rect.position);
                                }
                            }
                        }
                        // Reorder siblings if selected node has parent
                        if (selectedNode.parent != null)
                        {
                            Undo.RecordObject(selectedNode.parent, "Move Node");
                            selectedNode.parent.SortChildren();
                        }
                    }
                    nodeMoved = false;
                    currentHandle = null;
                    GUI.changed = true;
                    break;
            }
        }

        Vector2 SnapPositionToGrid(Vector2 position)
        {
            return new Vector2(
                Mathf.Round(position.x / 20f) * 20f, 
                Mathf.Round(position.y / 20f) * 20f
            );
        }

        private void TryConnectNodes(NodeHandle handle, Vector2 mousePosition)
        {
            // Find hovered node and connect or open dropdown
            Node targetNode = FindNode(mousePosition);
            if (targetNode == null) {
                OpenNodeFinder(new Rect(mousePosition.x, mousePosition.y, 1, 1), true, handle);
                return;
            }
            // Check if they are not the same node
            if (targetNode == handle.node) {
                return;
            }
            Undo.RecordObject(targetNode, "Connect Nodes");
            Undo.RecordObject(handle.node, "Connect Nodes");
            // There is node, try to connect if this is possible
            if (handle.type == HandleType.Input && targetNode is IParentNode) {
                // Do not allow connecting descendants as parents
                if (targetNode.IsDescendantOf(handle.node)) {
                    return;
                }
                // Then add as child to new parent
                targetNode.AddChild(handle.node);
                // Update order of nodes
                targetNode.SortChildren();
            } else if (handle.type == HandleType.Output && targetNode is IChildrenNode) {
                // Do not allow connecting descendants as parents
                if (handle.node.IsDescendantOf(targetNode)) {
                    return;
                }
                // Then add as child to new parent
                handle.node.AddChild(targetNode);
                // Update order of nodes
                handle.node.SortChildren();
            }
        }

        private void SelectNode(Node node)
        {
            currentMBT.selectedEditorNode = node;
            currentMBTEditor.Repaint();
            node.selected = true;
            selectedNode = node;
            GUI.changed = true;
        }

        private void DeselectNode(Node node)
        {
            currentMBT.selectedEditorNode = null;
            currentMBTEditor.Repaint();
            node.selected = false;
            selectedNode = null;
            GUI.changed = true;
        }

        private void DeselectNode()
        {
            currentMBT.selectedEditorNode = null;
            currentMBTEditor.Repaint();
            for (int i = 0; i < currentNodes.Length; i++)
            {
                currentNodes[i].selected = false;
            }
            selectedNode = null;
            GUI.changed = true;
        }

        private Node FindNode(Vector2 mousePosition)
        {
            for (int i = 0; i < currentNodes.Length; i++)
            {
                // Get correct position of node with offset
                Rect target = currentNodes[i].rect;
                target.position += workspaceOffset;
                if (target.Contains(mousePosition)) {
                    return currentNodes[i];
                }
            }
            return null;
        }

        private NodeHandle FindHandle(Vector2 mousePosition)
        {
            for (int i = 0; i < currentNodes.Length; i++)
            {
                Node node = currentNodes[i];
                // Get correct position of node with offset
                Rect targetRect = node.rect;
                targetRect.position += workspaceOffset;

                if (node is IChildrenNode) {
                    Vector2 handlePoint = GetTopHandlePosition(targetRect);
                    if (Vector2.Distance(handlePoint, mousePosition) < _handleDetectionDistance) {  
                        return new NodeHandle(node, handlePoint, HandleType.Input);
                    }
                }
                if (node is IParentNode) {
                    Vector2 handlePoint = GetBottomHandlePosition(targetRect);
                    if (Vector2.Distance(handlePoint, mousePosition) < _handleDetectionDistance) {  
                        return new NodeHandle(node, handlePoint, HandleType.Output);
                    }
                }
            }
            return null;
        }

        private void PaintNodes()
        {
            for (int i = currentNodes.Length - 1; i >= 0 ; i--)
            {
                Node node = currentNodes[i];
                Rect targetRect = node.rect;
                targetRect.position += workspaceOffset;
                // Draw node content
                GUILayout.BeginArea(targetRect, GetNodeStyle(node));
                    GUILayout.BeginVertical(_nodeContentBoxStyle);
                    if (node.breakpoint)
                    {
                        GUILayout.Label(node.title, _nodeBreakpointLabelStyle);
                    }
                    else
                    {
                        GUILayout.Label(node.title, _nodeLabelStyle);  
                    }
                    GUILayout.EndVertical();
                    if (Event.current.type == EventType.Repaint)
                    {
                        node.rect.height = GUILayoutUtility.GetLastRect().height;
                    }
                GUILayout.EndArea();
                
                // Draw connection handles if needed
                if (node is IChildrenNode)
                {
                    Vector2 top = GetTopHandlePosition(targetRect);
                    GUI.DrawTexture(
                        new Rect(top.x-8, top.y-5, 16, 16),
                        Resources.Load("mbt_node_handle", typeof(Texture2D)) as Texture2D
                    );
                }
                if (node is IParentNode)
                {
                    Vector2 bottom = GetBottomHandlePosition(targetRect);
                    GUI.DrawTexture(
                        new Rect(bottom.x-8, bottom.y-11, 16, 16),
                        Resources.Load("mbt_node_handle", typeof(Texture2D)) as Texture2D
                    );
                }
            }
        }

        private GUIStyle GetNodeStyle(Node node)
        {
            if (node.selected) {
                return _selectedNodeStyle;
            }
            switch (node.status)
            {
                case Status.Success:
                    return _successNodeStyle;
                case Status.Failure:
                    return _failureNodeStyle;
                case Status.Running:
                    return _runningNodeStyle;
            }
            return _defaultNodeStyle;
        }

        private Vector2 GetTopHandlePosition(Rect rect)
        {
            return new Vector2(rect.x + rect.width/2, rect.y);
        }

        private Vector2 GetBottomHandlePosition(Rect rect)
        {
            return new Vector2(rect.x + rect.width/2, rect.y + rect.height);
        }

        private void OpenNodeFinder(Rect rect, bool useRectPosition = true, NodeHandle handle = null)
        {
            // Store handle to connect later (null by default)
            dropdownHandleCache = handle;
            // Store real clicked position including workspace offset
            if (useRectPosition) {
                nodeDropdownTargetPosition = rect.position - workspaceOffset;
            } else {
                nodeDropdownTargetPosition = new Vector2(this.position.width/2, this.position.height/2) - workspaceOffset;
            }
            // Open dropdown
            nodeDropdown.Show(rect);
        }

        private void OpenNodeMenu(Vector2 mousePosition, Node node)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Breakpoint"), node.breakpoint, () => ToggleNodeBreakpoint(node));
            genericMenu.AddItem(new GUIContent("Disconnect Children"), false, () => DisconnectNodeChildren(node)); 
            genericMenu.AddItem(new GUIContent("Disconnect Parent"), false, () => DisconnectNodeParent(node)); 
            genericMenu.AddItem(new GUIContent("Delete Node"), false, () => DeleteNode(node)); 
            genericMenu.ShowAsContext();
        }

        void AddNode(ClassTypeDropdownItem item)
        {
            // In case there is nothing to add
            if (currentMBT == null || item.classType == null) {
                return;
            }
            // Allow only one root
            if (item.classType.IsAssignableFrom(typeof(Root)) && currentMBT.GetComponent<Root>() != null) {
                Debug.LogWarning("You can not add more than one Root node.");
                return;
            }
            Undo.SetCurrentGroupName("Create Node");
            Node node = (Node)Undo.AddComponent(currentMBT.gameObject, item.classType);
            node.title = item.name;
            node.hideFlags = HideFlags.HideInInspector;
            node.rect.position = nodeDropdownTargetPosition - new Vector2(node.rect.width/2, 0);
            UpdateSelection();
            if (dropdownHandleCache != null) {
                // Add additonal offset (3,3) to be sure that point is inside rect
                TryConnectNodes(dropdownHandleCache, nodeDropdownTargetPosition + workspaceOffset + new Vector2(3,3));
            }
        }

        private void ToggleNodeBreakpoint(Node node)
        {
            // Toggle breakpoint flag
            node.breakpoint = !node.breakpoint;
        }

        private void DeleteNode(Node node)
        {
            if (currentMBT == null) {
                return;
            }
            DeselectNode();
            // Disconnect all children and parent
            Undo.SetCurrentGroupName("Delete Node");
            DisconnectNodeChildren(node);
            DisconnectNodeParent(node);
            Undo.DestroyObjectImmediate(node);
            // DestroyImmediate(node, true);
            UpdateSelection();
        }

        private void DisconnectNodeParent(Node node)
        {
            if (node.parent != null) {
                Undo.RecordObject(node, "Disconnect Parent");
                Undo.RecordObject(node.parent, "Disconnect Parent");
                node.parent.RemoveChild(node);
            }
        }

        private void DisconnectNodeChildren(Node node)
        {
            Undo.RecordObject(node, "Disconnect Children");
            for (int i = node.children.Count - 1; i >= 0 ; i--)
            {
                Undo.RecordObject(node.children[i], "Disconnect Children");
                node.RemoveChild(node.children[i]);
            }
        }

        /// It is quite unique, but https://stackoverflow.com/questions/2920696/how-generate-unique-integers-based-on-guids
        private int GenerateId()
        {
            return System.Guid.NewGuid().GetHashCode();
        }

        private void PaintBackground()
        {
            // Background
            Handles.BeginGUI();
            Handles.DrawSolidRectangleWithOutline(new Rect(0, 0, position.width, position.height), _editorBackgroundColor, Color.gray);
            Handles.EndGUI();
            // Grid lines
            DrawBackgroundGrid(20, 0.1f, new Color(0.3f, 0.36f, 0.5f));
            DrawBackgroundGrid(100, 0.2f, new Color(0.3f, 0.36f, 0.5f));
        }

        /// Method copied from https://gram.gs/gramlog/creating-node-based-editor-unity/
        private void DrawBackgroundGrid(float gridSpacing, float gridOpacity, Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);
    
            Handles.BeginGUI();

            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);
    
            Vector3 newOffset = new Vector3(workspaceOffset.x % gridSpacing, workspaceOffset.y % gridSpacing, 0);
    
            for (int i = 0; i <= widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height+gridSpacing, 0f) + newOffset);
            }
    
            for (int j = 0; j <= heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width+gridSpacing, gridSpacing * j, 0f) + newOffset);
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }

        private class NodeHandle
        {
            public Node node;
            public Vector2 position;
            public HandleType type;

            public NodeHandle(Node node, Vector2 position, HandleType type)
            {
                this.node = node;
                this.position = position;
                this.type = type;
            }
        }

        private enum HandleType
        {
            Input, Output
        }
    }
}
