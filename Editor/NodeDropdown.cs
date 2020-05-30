using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using UnityEditor.IMGUI.Controls;
using MBT;

namespace MBTEditor
{
    public class ClassTypeDropdownItem : AdvancedDropdownItem
    {
        public Type classType;
        public int order;
        public string path;

        public ClassTypeDropdownItem(string name, Type type = null, int order = 1000, string path = "") : base(name)
        {
            this.classType = type;
            this.order = order;
            this.path = path;
        }
    }

    public class NodeDropdown : AdvancedDropdown
    {
        protected Action<ClassTypeDropdownItem> Callback;
        
        public NodeDropdown(AdvancedDropdownState state, Action<ClassTypeDropdownItem> callback) : base(state)
        {
            this.Callback = callback;
            minimumSize = new Vector2(230,320);
        }

        protected override AdvancedDropdownItem BuildRoot()
        {
            var root = new ClassTypeDropdownItem("Nodes");
            
            // Find all subclasses of Node
            IEnumerable<Type> enumerable = System.Reflection.Assembly.GetAssembly(typeof(Node)).GetTypes()
            .Where(myType => myType.IsClass && !myType.IsAbstract && myType.IsSubclassOf(typeof(Node)));

            // Keep track of all paths to correctly build tree later
            Dictionary<string, ClassTypeDropdownItem> nodePathsDictionary = new Dictionary<string, ClassTypeDropdownItem>();
            nodePathsDictionary.Add("", root);
            // Create list of items
            List<ClassTypeDropdownItem> items = new List<ClassTypeDropdownItem>();
            foreach (Type type in enumerable)
            {
                if(type.IsDefined(typeof(MBTNode), false))
                {
                    MBTNode nodeMeta = type.GetCustomAttribute<MBTNode>();
                    string itemName;
                    string nodePath = "";
                    if (String.IsNullOrEmpty(nodeMeta.name))
                    {
                        itemName = type.Name;
                    }
                    else
                    {
                        string[] path = nodeMeta.name.Split('/');
                        itemName = path[path.Length-1];
                        nodePath = BuildPathIfNotExists(path, ref nodePathsDictionary);
                    }
                    ClassTypeDropdownItem classTypeDropdownItem = new ClassTypeDropdownItem(itemName, type, nodeMeta.order, nodePath);
                    if (nodeMeta.icon != null)
                    {
                        classTypeDropdownItem.icon = Resources.Load(nodeMeta.icon, typeof(Texture2D)) as Texture2D;
                    }
                    items.Add(classTypeDropdownItem);
                }
            }

            // Sort items
            items.Sort((x, y) => {
                int result = x.order.CompareTo(y.order);
                return result != 0 ? result : x.name.CompareTo(y.name);
            });
            
            // Add all nodes to menu
            for (int i = 0; i < items.Count; i++)
            {
                nodePathsDictionary[items[i].path].AddChild(items[i]);
            }

            // Remove root to avoid infinite root foler loop
            nodePathsDictionary.Remove("");
            List<ClassTypeDropdownItem> parentNodes = nodePathsDictionary.Values.ToList();
            parentNodes.Sort((x, y) => {
                return x.name.CompareTo(y.name);
            });

            // Add folders
            for (int i = 0; i < parentNodes.Count(); i++)
            {
                root.AddChild(parentNodes[i]);
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            Callback(item as ClassTypeDropdownItem);
        }

        /// <summary>
        /// Creates nodes if path does not exists. Supports only signle level folders.
        /// </summary>
        /// <param name="path">Path to build. Last element should be actual node name.</param>
        /// <param name="dictionary">Reference to dictionary to store references to items</param>
        /// <returns>Path to provided node in path</returns>
        protected string BuildPathIfNotExists(string[] path, ref Dictionary<string, ClassTypeDropdownItem> dictionary)
        {
            // IMPORTANT: This code supports only single level folders. Nodes can't be nested more than one level.
            if (path.Length != 2)
            {
                return "";
            }
            AdvancedDropdownItem root = dictionary[""];
            // // This code assumes the last element of path is actual name of node
            // string nodePath = String.Join("/", path, 0, path.Length-1);
            string nodePath = path[0];
            // Create path nodes if does not exists
            if(!dictionary.ContainsKey(nodePath))
            {
                ClassTypeDropdownItem node = new ClassTypeDropdownItem(nodePath);
                dictionary.Add(nodePath, node);
            }
            return nodePath;
        }
    }
}
