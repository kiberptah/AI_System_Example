using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;

namespace AI_BehaviorEditor
{
    public class StateNode : CustomNode
    {
        public AIGraphView graphView; // don't forget to specify it!!!
        //public StateNode_Data data = new StateNode_Data();
        public List<string> childNodesGUID = new List<string>();

        public TextField nameField = new TextField();

        public Port inputPort;

        public Port onEnterStatePort;
        public Port onExitStatePort;
        public Port actionsLoopPort;
        public Port transitionsChecksLoopPort;



        public StateNode(AIGraphView graphView)
        {
            this.graphView = graphView;
        }

        public void AddChild(CustomNode node)
        {
            childNodesGUID.Add(node.nodeData.GUID);
        }

        public override void OnSelected()
        {
            for (int i = 0; i < childNodesGUID.Count; i++)
            {
                CustomNode tempNode = graphView.FindCustomNodeByGUID(childNodesGUID[i]);
                if (tempNode != null)
                {
                    graphView.AddToSelection(tempNode);
                }
                else
                {
                    childNodesGUID.RemoveAt(i);
                    i--;
                }
            }
        }



        public static StateNode Create_StateNode(StateNode thisNode)
        {
            thisNode.title = "STATE";
            thisNode.mainContainer.contentContainer.AddToClassList("contentContainer"); // To make it not transparent

            // --- State Name Field
            {
                thisNode.nameField.AddToClassList("stateNodeNameField");
                thisNode.titleContainer.Add(thisNode.nameField);
                thisNode.nameField.RegisterValueChangedCallback(x =>
                {
                    thisNode.nameField.value = thisNode.nameField.value.ToUpper();
                });

            }

            // --- Generate Input Port
            {
                thisNode.inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(AIGraphView.Port_States), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Input";
                thisNode.inputPort.portColor = Color.red;
                thisNode.inputContainer.Add(thisNode.inputPort);
            }

            // --- Generate Output Ports
            {
                // Port for onEnter actions
                thisNode.onEnterStatePort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_Actions), Port.Capacity.Single);
                thisNode.onEnterStatePort.portName = "onEnter";
                thisNode.onEnterStatePort.portColor = new Color(1, 0.6f, 0);
                thisNode.outputContainer.Add(thisNode.onEnterStatePort);

                // Port for actions loop
                thisNode.actionsLoopPort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_Actions), Port.Capacity.Single);
                thisNode.actionsLoopPort.portName = "Loop";
                thisNode.actionsLoopPort.portColor = Color.yellow;
                thisNode.outputContainer.Add(thisNode.actionsLoopPort);

                // Port for decisions loop
                thisNode.transitionsChecksLoopPort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_Actions), Port.Capacity.Single);
                thisNode.transitionsChecksLoopPort.portName = "Decisions";
                thisNode.transitionsChecksLoopPort.portColor = Color.magenta;
                thisNode.outputContainer.Add(thisNode.transitionsChecksLoopPort);

                // Port for onExit actions
                thisNode.onExitStatePort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_Actions), Port.Capacity.Single);
                thisNode.onExitStatePort.portName = "onExit";
                thisNode.onExitStatePort.portColor = new Color(1, 0.6f, 0);
                thisNode.outputContainer.Add(thisNode.onExitStatePort);
            }
 
            thisNode.RefreshExpandedState();
            thisNode.RefreshPorts();

            return thisNode;
        }

    }
}

/*
 // --- BUTTONS TO ADD NODES
 {
     // --- Button to add actions
     {
         Button button = new Button(clickEvent: () => AddActionToState())
         {
             text = "Add Action"
         };
         button.AddToClassList("newActionButt");
         thisNode.mainContainer.Add(button);

         void AddActionToState()
         {
             ActionNode newNode = Create_ActionNode(new ActionNode());

             newNode.stateNodeGUID = thisNode.nodeData.GUID;
             newNode.SetPosition(thisNode.GetPosition());

             AddElement(newNode);
             thisNode.AddChild(newNode);
         }
     }

     // --- Button to add decisions
     {
         Button button = new Button(clickEvent: () => AddActionToState())
         {
             text = "Add Decision"
         };
         button.AddToClassList("newDecisionButt");
         thisNode.mainContainer.Add(button);

         void AddActionToState()
         {
             DecisionNode newNode = Create_DecisionNode(new DecisionNode());

             newNode.stateNodeGUID = thisNode.nodeData.GUID;
             newNode.SetPosition(thisNode.GetPosition());

             AddElement(newNode);
             thisNode.AddChild(newNode);
         }
     }
     // --- Button to add timers
     {
         Button button = new Button(clickEvent: () => AddActionToState())
         {
             text = "Add Timer"
         };
         button.AddToClassList("newTimerButt");
         thisNode.mainContainer.Add(button);

         void AddActionToState()
         {
             TimerNode newNode = Create_TimerNode(new TimerNode());

             newNode.stateNodeGUID = thisNode.nodeData.GUID;
             newNode.SetPosition(thisNode.GetPosition());

             AddElement(newNode);
             thisNode.AddChild(newNode);
         }
     }
     // --- Button to add values
     {
         Button button = new Button(clickEvent: () => AddActionToState())
         {
             text = "Add Value"
         };
         button.AddToClassList("newValueButt");
         thisNode.mainContainer.Add(button);

         void AddActionToState()
         {
             ValueNode newNode = Create_ValueNode(new ValueNode() );

             newNode.stateNodeGUID = thisNode.nodeData.GUID;
             newNode.SetPosition(thisNode.GetPosition());

             AddElement(newNode);
             thisNode.AddChild(newNode);
         }
     }
     // --- Button to add valueschangers
     {
         Button button = new Button(clickEvent: () => AddActionToState())
         {
             text = "Add Value Changer"
         };
         button.AddToClassList("newValueChangerButt");
         thisNode.mainContainer.Add(button);

         void AddActionToState()
         {
             ValueChangerNode newNode = Create_ValueChangerNode(new ValueChangerNode());

             newNode.stateNodeGUID = thisNode.nodeData.GUID;
             newNode.SetPosition(thisNode.GetPosition());

             AddElement(newNode);
             thisNode.AddChild(newNode);
         }
     }

     // --- Button to add transition
     {
         Button button = new Button(clickEvent: () => AddActionToState())
         {
             text = "Add State Transition"
         };
         button.AddToClassList("newStateTransitionButt");
         thisNode.mainContainer.Add(button);

         void AddActionToState()
         {
             TransitionNode newNode = Create_TransitionNode(new TransitionNode());

             newNode.stateNodeGUID = thisNode.nodeData.GUID;
             newNode.SetPosition(thisNode.GetPosition());

             AddElement(newNode);
             thisNode.AddChild(newNode);
         }
     }

 }
 */