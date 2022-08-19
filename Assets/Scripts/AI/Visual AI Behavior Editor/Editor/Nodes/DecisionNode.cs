

using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

using UnityEngine;
using UnityEngine.UIElements;



namespace AI_BehaviorEditor
{
    public class DecisionNode : CustomNode
    {
        //public DecisionNode_Data data = new DecisionNode_Data();.
        public string stateNodeGUID;


        public ObjectField decisionField = new ObjectField()
        {
            objectType = typeof(AI_Decision)
        };

        public List<Port> dynamicValuesPorts = new List<Port>();



        public Port inputPort;

        public Port truePort;
        public Port falsePort;


        public static DecisionNode Create_DecisionNode(DecisionNode thisNode)
        {
            thisNode.title = "DECISION";
            thisNode.mainContainer.contentContainer.AddToClassList("contentContainer");

            thisNode.mainContainer.Add(thisNode.decisionField);
            Box dynamicValuesContainer = new Box();
            thisNode.mainContainer.Add(dynamicValuesContainer);

            // --- Generate Input Port
            {
                thisNode.inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(AIGraphView.Port_Actions), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Input";
                thisNode.inputPort.portColor = Color.magenta;

                thisNode.inputContainer.Add(thisNode.inputPort);
            }

            // --- Generate Output Ports
            {
                thisNode.truePort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_Actions), Port.Capacity.Single);
                thisNode.truePort.portName = "True";
                thisNode.truePort.portColor = Color.magenta;
                thisNode.outputContainer.Add(thisNode.truePort);

                thisNode.falsePort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_Actions), Port.Capacity.Single);
                thisNode.falsePort.portName = "False";
                thisNode.falsePort.portColor = Color.magenta;
                thisNode.outputContainer.Add(thisNode.falsePort);
            }

            // --- Field
            {
                thisNode.decisionField.RegisterValueChangedCallback(x =>
                {
                    if (thisNode.decisionField.value != null)
                    {
                        thisNode.decisionField.tooltip = thisNode.decisionField.value.name;
                        AddDynamicValues();
                    }
                    else
                    {
                        dynamicValuesContainer.Clear();
                    }
                });

                // --- if we load node
                if (thisNode.decisionField.value != null)
                {
                    AddDynamicValues();
                }



                void AddDynamicValues()
                {
                    dynamicValuesContainer.Clear(); // clear on update to delete ports from prev script

                    if (thisNode.decisionField.value != null)
                    {
                        // INTEGER VALUES
                        if (((AI_Decision)thisNode.decisionField.value).dynamicValues.intValues.Count != 0)
                        {
                            Label label = new Label();
                            label.text = "INTEGERs:";
                            dynamicValuesContainer.Add(label);
                        }
                        foreach (var value in ((AI_Decision)(thisNode.decisionField.value)).dynamicValues.intValues)
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
                        if (((AI_Decision)thisNode.decisionField.value).dynamicValues.floatValues.Count != 0)
                        {
                            Label label = new Label();
                            label.text = "FLOATs:";
                            dynamicValuesContainer.Add(label);
                        }
                        foreach (var value in ((AI_Decision)(thisNode.decisionField.value)).dynamicValues.floatValues)
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
                        if (((AI_Decision)thisNode.decisionField.value).dynamicValues.boolValues.Count != 0)
                        {
                            Label label = new Label();
                            label.text = "BOOLEANs:";
                            dynamicValuesContainer.Add(label);
                        }
                        foreach (var value in ((AI_Decision)(thisNode.decisionField.value)).dynamicValues.boolValues)
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
                        if (((AI_Decision)thisNode.decisionField.value).dynamicValues.stringValues.Count != 0)
                        {
                            Label label = new Label();
                            label.text = "STRINGs:";
                            dynamicValuesContainer.Add(label);
                        }
                        foreach (var value in ((AI_Decision)(thisNode.decisionField.value)).dynamicValues.stringValues)
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

            }

            thisNode.RefreshExpandedState();
            thisNode.RefreshPorts();

            return thisNode;
        }
    }
}


