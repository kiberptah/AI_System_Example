using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;


namespace AI_BehaviorEditor
{
    public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
    {
        private EditorWindow window;
        private AIGraphView graphView;
        private Texture2D indentationIcon;


        // Stuff we need to specify in edge release in graphview script
        public StateNode stateNode;
        public Rect newNodeRect = new Rect();
        public Port originPort = null;


        public void Configure(EditorWindow window, AIGraphView graphView)
        {
            this.window = window;
            this.graphView = graphView;

            //Transparent 1px indentation icon as a hack for lack of left margin
            indentationIcon = new Texture2D(1, 1);
            indentationIcon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            indentationIcon.Apply();
        }

        // Here we specify all possible options in searchwindow, they can be on differen levels (in folders, sorta) but i dont need it
        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
                new SearchTreeEntry(new GUIContent("Action Node", indentationIcon))
                {
                    level = 1, 
                    userData = new ActionNode()
                },
                new SearchTreeEntry(new GUIContent("Decision Node",indentationIcon))
                {
                    level = 1,
                    userData = new DecisionNode()
                },
                new SearchTreeEntry(new GUIContent("Timer Node",indentationIcon))
                {
                    level = 1,
                    userData = new TimerNode()
                },
                new SearchTreeEntry(new GUIContent("Value Changer Node",indentationIcon))
                {
                    level = 1,
                    userData = new ValueChangerNode()
                },
                new SearchTreeEntry(new GUIContent("Transition Node",indentationIcon))
                {
                    level = 1,
                    userData = new TransitionNode()
                },
            };

            return tree;
        }



        public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            switch (SearchTreeEntry.userData)
            {
                case ActionNode newNode:
                    stateNode.AddChild(newNode);
                    newNode.stateNodeGUID = stateNode.nodeData.GUID;

                    newNode.SetPosition(newNodeRect);
                    graphView.AddElement(ActionNode.Create_ActionNode(newNode));
                    if (originPort.direction == Direction.Output)
                    {
                        graphView.TryToConnectTwoPorts(newNode.inputPort, originPort);
                    }
                    else
                    {
                        graphView.TryToConnectTwoPorts(newNode.outputPort, originPort);
                    }                  
                    return true;
                case DecisionNode newNode:
                    stateNode.AddChild(newNode);
                    newNode.stateNodeGUID = stateNode.nodeData.GUID;

                    newNode.SetPosition(newNodeRect);
                    graphView.AddElement(DecisionNode.Create_DecisionNode(newNode));
                    if (originPort.direction == Direction.Output)
                    {
                        graphView.TryToConnectTwoPorts(newNode.inputPort, originPort);
                    }
                    else
                    {
                    }
                    return true;
                case TimerNode newNode:
                    stateNode.AddChild(newNode);
                    newNode.stateNodeGUID = stateNode.nodeData.GUID;

                    newNode.SetPosition(newNodeRect);
                    graphView.AddElement(TimerNode.Create_TimerNode(newNode));
                    if (originPort.direction == Direction.Output)
                    {
                        graphView.TryToConnectTwoPorts(newNode.inputPort, originPort);
                    }
                    else
                    {
                    }
                    return true;
                case TransitionNode newNode:
                    stateNode.AddChild(newNode);
                    newNode.stateNodeGUID = stateNode.nodeData.GUID;

                    newNode.SetPosition(newNodeRect);
                    graphView.AddElement(TransitionNode.Create_TransitionNode(newNode));
                    if (originPort.direction == Direction.Output)
                    {
                        graphView.TryToConnectTwoPorts(newNode.inputPort, originPort);
                    }
                    else
                    {
                        
                    }
                    return true;
                case ValueChangerNode newNode:
                    stateNode.AddChild(newNode);
                    newNode.stateNodeGUID = stateNode.nodeData.GUID;

                    newNode.SetPosition(newNodeRect);
                    graphView.AddElement(ValueChangerNode.Create_ValueChangerNode(newNode));
                    if (originPort.direction == Direction.Output)
                    {
                        graphView.TryToConnectTwoPorts(newNode.inputPort, originPort);
                    }
                    else
                    {
                        graphView.TryToConnectTwoPorts(newNode.outputPort, originPort);
                    }
                    return true;
            }
            return false;
        }
    }
}