
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEngine;


namespace AI_BehaviorEditor
{
    public class TransitionNode : CustomNode
    {
        //public TransitionNode_Data data = new TransitionNode_Data();

        public string stateNodeGUID;

        public Port inputPort;
        public Port outputPort;



        public static TransitionNode Create_TransitionNode(TransitionNode thisNode)
        {
            thisNode.title = "TRANSITION";

            // --- Generate Input Port
            {
                thisNode.inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(AIGraphView.Port_Actions), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Decision";
                thisNode.inputPort.portColor = Color.yellow;

                thisNode.inputContainer.Add(thisNode.inputPort);
            }

            // --- Generate Output Port
            {
                thisNode.outputPort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_States), Port.Capacity.Single);
                thisNode.outputPort.portName = "Next State";
                thisNode.outputPort.portColor = Color.red;

                thisNode.outputContainer.Add(thisNode.outputPort);
            }

            return thisNode;
        }
    }
}
