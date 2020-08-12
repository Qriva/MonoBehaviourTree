# MonoBehaviourTree — Simple behaviour tree for Unity
This project is simple event driven behaviour tree based on Unity engine component system. This asset comes with minimal node library and tree visual editor.

**Important:** This is not visual scripting tool and requires you to implement some of your own nodes.

## Contribute
// TODO

## Getting started
Copy the Assets/MonoBehaviourTree folder to your project. 
If you need some usage examples you might want to check folder **Example** containing demo scenes, otherwise you can delete it to get rid of redundant files. This documentation assumes you have basic knowledge about behaviour trees. If you don't, you should check some online resources like this 
[Gamasutra article](https://www.gamasutra.com/blogs/ChrisSimpson/20140717/221339/Behavior_trees_for_AI_How_they_work.php)
or [Unreal Engine documentation](https://docs.unrealengine.com/en-US/Engine/ArtificialIntelligence/BehaviorTrees/BehaviorTreesOverview/index.html).

## Event Driven Behaviour Tree Overview
Standard behaviour tree design assumes three types of nodes: composites, decorators and leafs. Composites are used to define the flow in the tree, decorators can modify node results and leafs perform tasks or check conditions. This design has one major flaw - tree must be traversed from the beginning every single tick, otherwise it will not be possible to react to changes in state or data. Event driven tree is the fix to that problem. When tree gets update it continues from the last executed running node. Normally it would mean, that executed higher priority nodes will not be reevaluated immediately, but event driven BT introduces **abort system** to give possibility to reset tree to some state, when certain event occur. Implementation used in this project is very similar to the one used in Unreal engine - leaf nodes are not used as conditions, instead they are in form of decorators. Additionally it is possible to create Service nodes which can perform task periodically.

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

## Component Reference
### MonoBehaviourTree component
### Blacboard component

## Variables and Events

## Node Reference
### Subtree
Subtree node allows connection of other tree as child. Parent of (sub)tree must be specified to work properly.

## Creating custom nodes

## Debugging
