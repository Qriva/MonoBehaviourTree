# MonoBehaviourTree — Simple behaviour tree for Unity
This project is simple event driven behaviour tree based on Unity engine component system. This asset comes with minimal node library and tree visual editor.

**Important:** This is not visual scripting tool and requires you to implement your own nodes.

## Contribute
// TODO

## Getting started
Copy the Assets/MonoBehaviourTree folder to your project. 
If you need some usage examples you might want to check folder **Example** containing demo scenes, otherwise you can delete it to get rid of redundant files. This documentation assumes you have basic knowledge about behaviour trees. If you don't, you should check some online resources like this 
[Gamasutra article](https://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php)
or [Unreal Engine documentation](https://docs.unrealengine.com/en-US/Engine/ArtificialIntelligence/BehaviorTrees/BehaviorTreesOverview/index.html).

## Event Driven Behaviour Tree Overview
Standard behaviour tree design assumes three types of nodes: composites, decorators and leafs. Composites are used to define the flow in the tree, decorators can modify node results and leafs perform tasks or check conditions. This design has one major flaw - tree must be traversed from the beginning every single tick, otherwise it will not be possible to react to changes in state or data. Event driven tree is the fix to that problem. When tree gets update it continues from the last executed running node. Normally it would mean, that executed higher priority nodes will not be reevaluated immediately, but event driven BT introduces **abort system** to give possibility to reset tree to previous state, when certain event occur. Implementation used in this project is very similar to the one used in Unreal engine - leaf nodes are not used as conditions, instead they are in form of decorators. Additionally it is possible to create Service nodes which can perform task periodically.

### Node Abort
When the tree is updated, the first evaluated thing are aborting nodes. If there is any aborting node, the tree will be reset to that position and execution will be continued from this node.
In case there are multiple aborting nodes, the one closest to root will be selected.
There are four abort types:
- **None** - don't do anything when change occurs
- **Self** - abort children running below
- **Lower Priority** - abort running nodes with lower priority (nodes to the right)
- **Both** - abort self and lower priority nodes

>Execution order (priority) of nodes with common ancestor is defined by position on X axis, nodes to the left has higher priority.

## Basic Usage
The main core of behaviour tree is **MonoBehaviourTree** component. It contains most of tree state during runtime. It is important to note, that tree does not run automatically and must be updated by other script. This design gives you possibility to tick the tree in Update, FixedUpdate or custom interval. However, most of the time Update event will be used, so you can use component **MBT Executor** to do that.

In most of cases you will need some place to store shared data for nodes. You can implement your own component to do that, but you can use **Blackboard** component. Blackboard allows you to create observable variables of predefined types that are compatible with default nodes.

## Node editor
To open tree editor window click "Open editor" button of MonoBehaviourTree component or click Unity menu: Window / Mono Behaviour Tree. In visual editor you can create, connect, delete and setup nodes. 

Every behaviour tree needs an entry point called **Root**. To add it right click on empty canvas to open node popup, then select Root. Execution of BT starts here and goes from top to down, left to right.

> **Implementation note:** All nodes and variables are in fact components, but they are invisible in inspector window.
> It is recommended to use separate empty game object to build the tree - this make it easier to create prefabs and avoid unnecessary unknown problems.

Most of nodes has additional properties that you can change. To do this select the node and list of options will show up in MonoBehaviourTree component inspector section (standard inspector).

### Editor Window Features
Right click on empty space to create new node. To connect nodes click on in/out handler (black dot on top and bottom of node), then drag and drop it above another node. In case node cannot have more than one child (decorator) the connection will be overridden by the new one.
To delete or disconnect nodes right click on node to open context menu and select the appropriate option.
Use left mouse button to drag workspace or nodes. You can drag whole nodes branch when ALT is pressed.

## Component Reference

### MonoBehaviourTree component
Main component used as hub of behaviour tree.

**Properties**
- **Description** - optional user description.
- **Repeat OnFinish** - whether the tree should be executed again when finished.
- **Max Executions Per Tick** - how many nodes should be executed during single update.
- **Parent** - parent reference if this tree is subtree. Read more in [Subtree node section](#subtree).

### Blacboard component
Component used to provide and manage observable variables.
To add variable fill the **Key** text field, select it's type and press "Add" button. Key is used as identifier to get or set variable value, this can be done by VariableReference or blackboard method: ```public T GetVariable<T>(string key)```.
Blackboard component displays all available variables in list and allows to set initial value for each of them.
> **Implementation note:** Changing variable value during playmode will not trigger change listeners. Additionally as  variables are components too, displayed values can be not up to date, because unity gui refreshes only when its property change. You can force repaint by hovering pointer above component inspector.

**Built In variable types:** Bool, Float, Int, Object, Quaternion, String, Transform, Vector2, Vector3. If you need to add your own custom type read [Variables and Events section](#custom-variable).

## Variables and Events
In most of situations nodes need to share some state data between each other, it can be done by Blackboard, Variable and VariableReference system. Variables are observale data containers, that can be accesed via Blackboard. To get variable you need to know its key, but inputting key manually to every node is not handy and very error prone. To avoid this you can use helper class VariableReference. This class allows you to automaticaly get and cache reference to blackboard variable.
VariableReference has also constant value mode in case you don't need to retrive values from blackboard. You can toggle VarRef mode in editor by clicking small button to the left.
```
// Get variable from blackboard by key
FloatVariable floatVar = blackboard.GetVariable<FloatVariable>("myKey");

// Attach listener to variable
floatVar.AddListener(MyVariableChangeListener);

// Create float reference property with default constant value
public FloatReference floatRef = new FloatReference(1f);

// Check if its in constant or reference mode
bool constant = floatRef.isConstant;

// Attach listener to variable reference
if (!constant)
{
    // GetVariable will return null if floatRef is constant
    floatRef.GetVariable().AddListener(MyVariableChangeListener);
}

// Get or set value of variable reference
floatRef.Value = 1.5f;
float value = floatRef.Value;
```
> **Important:** TransformVariable change listener will be called only when reference to object changes. Position or rotation changes do not trigger change listener.

### Custom Variable
If built in variables are not enough, you can create your own.
To create new Variable and VariableReference you must extend Variable class and VariableReference class. Variable inheriths MonoBehaviour, so to work properly it must by placed in file of the same name as your custom type. VariableReference is normal serializable class and can be placed in the same file. To disallow adding variable component manually add [AddComponentMenu("")] attribute.
```
[AddComponentMenu("")]
public class CustomVariable : Variable<CustomType>
{
    
}

[System.Serializable]
public class CustomReference : VariableReference<CustomVariable, CustomType>
{
    // You can create additional constructors and Value getter/setter
    // See FloatVariable.cs as example
}
```

## Node Reference

### Root
Entry node of behaviour tree.
### Sequence
Executes children from left to right as long as each subsequent child returns success. Returns success when all children succeeded. Failure if one of them failed. When Random option is enabled, then execution goes in random order.
### Selector
Executes children from left to right until one of them return failure. Returns success if any children succeed. Failure if all of them failed. When Random option is enabled, then execution goes in random order.
### Is Set Condition
Checks if blackboard variable is set. Node supports Bollean, Object and Transform variables. Selecting Invert option will produce "If not set" effect.
### Number Condition
Checks if blackboard number variable meets requirement. Node supports Float and Int variables.
### Cooldown
Blocks execition until the specified amount of time has elapsed.
Time starts counting after branch is exited. If abort is enabled, the execution will be moved back to this node after time has elapsed.
### Inverter
Inverts node result. Failure becomes Success and vice versa.
### Random Chance
There is fixed chance that node will be executed. Returns Failure if roll is not favorable.
### Repeat Until Fail
Repeats branch as long as Success is returned from child.
### Repeater
Repeat branch specified amount of times or infinitely.
### Succeeder
Always returns Success.
### Time Limit
Determines how long branch can be executed. After given time elapses branch is aborted and Failure is returned.
### Calculate Distance Service
Calculates distance between two transforms and updates blackboard flaot variable with the result.
### Update Position Service
Updates blackboard Vector3 variable with position of given source transform.
### Wait
Waits specifie time, then returns Success.
### Subtree
Subtree node allows connection of other behaviour tree as child, this gives you possibility to create reusable blocks of nodes. Such a tree must be created in separate game object and attached as children. Child tree is updated by its parent. **Parent of subtree must be specified in MonoBehaviourTree component to work properly.** 

## Creating custom nodes
// TODO
### Custom Leaf
### Custom Decorator
### Custom Service

## Debugging
During playmode you can preview tree execution flow in editor window. Nodes are marked with the appropriate color corresponding to their state:
- Ready - none (default)
- Success - green
- Failure - orange
- Running - purple

Except that, you can set breakpoints on multiple nodes. Breakpoint will stop execution and pause play mode after node is entered, but before it get executed. Nodes with breakpoint enabled will have red node names.