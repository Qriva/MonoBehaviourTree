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

        public ClassTypeDropdownItem(string name, Type type = null, int order = 1000) : base(name)
        {
            this.classType = type;
            this.order = order;
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

            // Create list of items
            List<ClassTypeDropdownItem> items = new List<ClassTypeDropdownItem>();
            foreach (Type type in enumerable)
            {
                if(type.IsDefined(typeof(MBTNode), false))
                {
                    MBTNode nodeMeta = type.GetCustomAttribute<MBTNode>();
                    string itemName = (nodeMeta.name == null)? type.Name : nodeMeta.name;
                    ClassTypeDropdownItem classTypeDropdownItem = new ClassTypeDropdownItem(itemName, type, nodeMeta.order);
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
                root.AddChild(items[i]);
            }

            return root;
        }

        protected override void ItemSelected(AdvancedDropdownItem item)
        {
            Callback(item as ClassTypeDropdownItem);
        }

    }
}
