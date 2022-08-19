using UnityEditor.Experimental.GraphView;
using System.Collections.Generic;
using UnityEngine;

namespace AI_BehaviorEditor
{
    public class CustomNode : Node
    {
        public CustomNode_Data nodeData = new CustomNode_Data();

        public CustomNode()
        {
            nodeData.GUID = System.Guid.NewGuid().ToString();
        }
    }



}