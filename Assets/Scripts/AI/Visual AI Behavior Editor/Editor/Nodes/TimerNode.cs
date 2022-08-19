using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEngine;


namespace AI_BehaviorEditor
{
    public class TimerNode : CustomNode
    {
        //public TimerNode_Data data = new TimerNode_Data();
        public string stateNodeGUID;


        public FloatField intervalField = new FloatField();
        public FloatField randomOffsetField = new FloatField();

        public Port inputPort;
        public Port truePort;
        public Port falsePort;



        public static TimerNode Create_TimerNode(TimerNode thisNode)
        {
            thisNode.title = "Timer";
            thisNode.mainContainer.contentContainer.AddToClassList("contentContainer");

            // --- Generate Input Port
            {
                thisNode.inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(AIGraphView.Port_Actions), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Input";
                thisNode.inputPort.portColor = Color.cyan;

                thisNode.inputContainer.Add(thisNode.inputPort);
            }

            Box horizontalContent = new Box(); // To save vertical space and put both fields next to each other
            horizontalContent.AddToClassList("horizontalContent");
            thisNode.mainContainer.Add(horizontalContent);

            // --- Time Interval
            {
                Box verticalContent = new Box(); // for vertical ordering
                verticalContent.AddToClassList("centeredContent");
                horizontalContent.Add(verticalContent);

                Label label = new Label("Time Interval:");
                label.AddToClassList("timerFieldLabel");
                verticalContent.Add(label);

                thisNode.intervalField.AddToClassList("timerField");
                verticalContent.Add(thisNode.intervalField);
            }
            // --- Interval Randomizer
            {
                Box verticalContent = new Box(); // for vertical ordering
                verticalContent.AddToClassList("centeredContent");
                horizontalContent.Add(verticalContent);

                Label label = new Label("Random Offset:");
                label.AddToClassList("timerFieldLabel");
                verticalContent.Add(label);

                thisNode.randomOffsetField.AddToClassList("timerField");
                verticalContent.Add(thisNode.randomOffsetField);
            }

            // --- Generate Output Ports
            {
                thisNode.falsePort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_Actions), Port.Capacity.Single);
                thisNode.falsePort.portName = "False";
                thisNode.falsePort.portColor = Color.cyan;
                thisNode.outputContainer.Add(thisNode.falsePort);

                thisNode.truePort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_Actions), Port.Capacity.Single);
                thisNode.truePort.portName = "True";
                thisNode.truePort.portColor = Color.cyan;
                thisNode.outputContainer.Add(thisNode.truePort);
            }

            return thisNode;
        }

    }


}
