

using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;


namespace AI_BehaviorEditor
{
    public class ActionNode : CustomNode
    {
        //public ActionNode_Data data = new ActionNode_Data();
        public string stateNodeGUID;


        public ObjectField actionField = new ObjectField()
        {
            objectType = typeof(AI_Action)
        };

        public List<Port> dynamicValuesPorts = new List<Port>();

        public Port inputPort;
        public Port outputPort;


        public static ActionNode Create_ActionNode(ActionNode thisNode)
        {
            thisNode.title = "ACTION";
            thisNode.mainContainer.contentContainer.AddToClassList("contentContainer");

            thisNode.mainContainer.Add(thisNode.actionField);
            Box dynamicValuesContainer = new Box();
            thisNode.mainContainer.Add(dynamicValuesContainer);

            // --- Generate Input Port
            {
                thisNode.inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(AIGraphView.Port_Actions), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Prev";
                thisNode.inputPort.portColor = Color.yellow;

                thisNode.inputContainer.Add(thisNode.inputPort);
            }
            // --- Generate Output Port
            {
                thisNode.outputPort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_Actions), Port.Capacity.Single);
                thisNode.outputPort.portName = "Next";
                thisNode.outputPort.portColor = Color.yellow;

                thisNode.outputContainer.Add(thisNode.outputPort);
            }

            // --- Action Field
            {
                thisNode.actionField.RegisterValueChangedCallback(x =>
                {
                    if (thisNode.actionField.value != null)
                    {
                        thisNode.actionField.tooltip = thisNode.actionField.value?.name;
                        AddDynamicValues();
                    }
                    else
                    {
                        dynamicValuesContainer.Clear();
                    }
                });


                // if we load from savedata
                if (thisNode.actionField.value != null)
                {
                    AddDynamicValues();
                }



                void AddDynamicValues()
                {
                    dynamicValuesContainer.Clear(); // clear it from values from previous action

                    // INTEGER VALUES
                    if (((AI_Action)thisNode.actionField.value).dynamicValues.intValues.Count != 0)
                    {
                        Label label = new Label();
                        label.text = "INTEGERs:";
                        dynamicValuesContainer.Add(label);
                    }
                    foreach (var value in ((AI_Action)(thisNode.actionField.value)).dynamicValues.intValues)
                    {
                        // --- int Port
                        {
                            Port inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(int), Port.Capacity.Multi);
                            inputPort.portName = value.Key;
                            inputPort.portColor = Color.white;

                            dynamicValuesContainer.Add(inputPort);
                            thisNode.dynamicValuesPorts.Add(inputPort);
                        }
                    }
                    // FLOAT VALUES
                    if (((AI_Action)thisNode.actionField.value).dynamicValues.floatValues.Count != 0)
                    {
                        Label label = new Label();
                        label.text = "FLOATs:";
                        dynamicValuesContainer.Add(label);
                    }
                    foreach (var value in ((AI_Action)(thisNode.actionField.value)).dynamicValues.floatValues)
                    {
                        // --- float Port
                        {
                            Port inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(float), Port.Capacity.Multi);
                            inputPort.portName = value.Key;
                            inputPort.portColor = Color.white;

                            dynamicValuesContainer.Add(inputPort);
                            thisNode.dynamicValuesPorts.Add(inputPort);
                        }
                    }
                    // BOOL VALUES
                    if (((AI_Action)thisNode.actionField.value).dynamicValues.boolValues.Count != 0)
                    {
                        Label label = new Label();
                        label.text = "BOOLEANs:";
                        dynamicValuesContainer.Add(label);
                    }
                    foreach (var value in ((AI_Action)(thisNode.actionField.value)).dynamicValues.boolValues)
                    {
                        // --- bool Port
                        {
                            Port inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(bool), Port.Capacity.Multi);
                            inputPort.portName = value.Key;
                            inputPort.portColor = Color.white;

                            dynamicValuesContainer.Add(inputPort);
                            thisNode.dynamicValuesPorts.Add(inputPort);
                        }
                    }
                    // STRING VALUES
                    if (((AI_Action)thisNode.actionField.value).dynamicValues.stringValues.Count != 0)
                    {
                        Label label = new Label();
                        label.text = "STRINGs:";
                        dynamicValuesContainer.Add(label);
                    }
                    foreach (var value in ((AI_Action)(thisNode.actionField.value)).dynamicValues.stringValues)
                    {
                        // --- string Port
                        {
                            Port inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(string), Port.Capacity.Multi);
                            inputPort.portName = value.Key;
                            inputPort.portColor = Color.white;

                            dynamicValuesContainer.Add(inputPort);
                            thisNode.dynamicValuesPorts.Add(inputPort);
                        }
                    }
                }
            }


            thisNode.RefreshExpandedState();
            thisNode.RefreshPorts();

            return thisNode;
        }



    }
}