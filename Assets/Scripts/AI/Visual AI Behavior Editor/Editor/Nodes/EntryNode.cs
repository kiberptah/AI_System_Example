using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

namespace AI_BehaviorEditor
{
    public class EntryNode : CustomNode
    {
        public Port outputPort;


        public static EntryNode Create_EntryNode(EntryNode thisNode)
        {
            thisNode.nodeData.GUID = "0";

            thisNode.title = "ENTRY";

            thisNode.capabilities &= ~Capabilities.Deletable;
            thisNode.capabilities &= ~Capabilities.Movable;
            thisNode.capabilities &= ~Capabilities.Copiable;
            //thisNode.capabilities &= ~Capabilities.Collapsible; // doesn't work anyway

            // --- Generate Output Port
            {
                thisNode.outputPort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_States), Port.Capacity.Single);
                thisNode.outputPort.portName = "Output";
                thisNode.outputPort.portColor = Color.red;

                thisNode.outputContainer.Add(thisNode.outputPort);
            }

            thisNode.RefreshExpandedState();
            thisNode.RefreshPorts(); // better be safe than sorry

            return thisNode;
        }
    }

    
}