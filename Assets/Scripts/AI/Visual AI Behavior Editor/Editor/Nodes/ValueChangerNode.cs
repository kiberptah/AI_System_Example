
using UnityEngine.UIElements;
using UnityEngine;

using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System;


namespace AI_BehaviorEditor
{
    public class ValueChangerNode : CustomNode
    {
        //public ValueChangerNode_Data data = new ValueChangerNode_Data();
        public string stateNodeGUID;


        public bool nodeIsPrepared = false;


        public PopupField<Type> typeField = new PopupField<Type>
        {
            choices = new List<Type> // if there's protection level error then use later version of unity, 2021.2.15 worked fine but eerlier 2021 didnt!
            {
                typeof(int),
                typeof(float),
                typeof(bool)
            }
        };

        public ObjectField valueChangerField = new ObjectField();

        public List<Port> dynamicValuesPorts = new List<Port>();

        public Port valuePort;

        public Port outputPort;
        public Port inputPort;


        public static ValueChangerNode Create_ValueChangerNode(ValueChangerNode thisNode)
        {
            thisNode.title = "Value Changer";
            thisNode.mainContainer.contentContainer.AddToClassList("contentContainer");
            Box dynamicValuesContainer = new Box();


            // --- Generate Input Port
            {
                thisNode.inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(AIGraphView.Port_Actions), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Input";
                thisNode.inputPort.portColor = Color.green;

                thisNode.inputContainer.Add(thisNode.inputPort);
            }
            // --- Generate Output Port
            {
                thisNode.outputPort = AIGraphView.GeneratePort(thisNode, Direction.Output, typeof(AIGraphView.Port_Actions), Port.Capacity.Single);
                thisNode.outputPort.portName = "Output";
                thisNode.outputPort.portColor = Color.green;

                thisNode.outputContainer.Add(thisNode.outputPort);
            }

            // --- field to select node type before generating the rest of it
            thisNode.typeField.RegisterValueChangedCallback(evt =>
            {
                switch (thisNode.typeField.value)
                {
                    case Type type when type == typeof(int):
                        {
                            SetUpNode(typeof(int));
                            break;
                        }
                    case Type type when type == typeof(float):
                        {
                            SetUpNode(typeof(float));
                            break;
                        }
                    case Type type when type == typeof(bool):
                        {
                            SetUpNode(typeof(bool));
                            break;
                        }
                }
            });
            thisNode.mainContainer.Add(thisNode.typeField);

            // --- if we load node from savedata
            if (thisNode.typeField.value != null)
            {
                switch (thisNode.typeField.value)
                {
                    case Type type when type == typeof(int):
                        {
                            SetUpNode(typeof(int));
                            break;
                        }
                    case Type type when type == typeof(float):
                        {
                            SetUpNode(typeof(float));
                            break;
                        }
                    case Type type when type == typeof(bool):
                        {
                            SetUpNode(typeof(bool));
                            break;
                        }
                }
            }

            // Set Up the rest of the node
            void SetUpNode(Type valueType)
            {
                thisNode.mainContainer.Remove(thisNode.typeField);


                // Set Up Field
                {
                    Type fieldType = typeof(AI_ValueChanger_Int);
                    if (valueType == typeof(int))
                    {
                        fieldType = typeof(AI_ValueChanger_Int);
                    }
                    else if (valueType == typeof(float))
                    {
                        fieldType = typeof(AI_ValueChanger_Float);
                    }
                    else if (valueType == typeof(bool))
                    {
                        fieldType = typeof(AI_ValueChanger_Bool);
                    }
                    else
                    {
                        AIGraph.graphView.RemoveElement(thisNode); // to prevent impossible errors?..
                        Debug.Log("ERROR: CREATING NODE WITH UNSUPPORTED VALUE TYPE");
                    }
                    thisNode.valueChangerField.objectType = fieldType;

                    thisNode.mainContainer.Add(dynamicValuesContainer);


                    thisNode.mainContainer.Add(thisNode.valueChangerField);
                    thisNode.valueChangerField.RegisterValueChangedCallback(x =>
                    {
                        if (thisNode.valueChangerField.value != null)
                        {
                            thisNode.valueChangerField.tooltip = thisNode.valueChangerField.value.name;
                            AddDynamicValues();
                        }
                        else
                        {
                            dynamicValuesContainer.Clear();
                        }
                    });


                    // --- again, happens if we load node from savedata
                    if (thisNode.valueChangerField.value != null)
                    {
                        AddDynamicValues();
                    }
                }

                AddValuePort(valueType);



                void AddValuePort(Type type)
                {
                    // --- Generate port to connect with value
                    {
                        thisNode.valuePort = AIGraphView.GeneratePort(thisNode, Direction.Output, type, Port.Capacity.Multi);
                        thisNode.valuePort.portName = "Value";
                        thisNode.valuePort.portColor = Color.white;

                        thisNode.outputContainer.Add(thisNode.valuePort);
                    }
                }
                void AddDynamicValues()
                {
                    dynamicValuesContainer.Clear(); // clear it from previous changer's values

                    // --- INT VALUES
                    if (((AI_ValueChanger)thisNode.valueChangerField.value).dynamicValues.intValues.Count != 0)
                    {
                        Label label = new Label();
                        label.text = "INTEGERs:";
                        dynamicValuesContainer.Add(label);
                    }
                    foreach (var value in ((AI_ValueChanger)thisNode.valueChangerField.value).dynamicValues.intValues)
                    {
                        {
                            Port inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(int), Port.Capacity.Single);
                            inputPort.portName = value.Key;
                            inputPort.portColor = Color.white;

                            dynamicValuesContainer.Add(inputPort);
                            thisNode.dynamicValuesPorts.Add(inputPort);
                        }
                    }

                    // --- FLOAT VALUES
                    if (((AI_ValueChanger)thisNode.valueChangerField.value).dynamicValues.floatValues.Count != 0)
                    {
                        Label label = new Label();
                        label.text = "FLOATs:";
                        dynamicValuesContainer.Add(label);
                    }
                    foreach (var value in ((AI_ValueChanger)thisNode.valueChangerField.value).dynamicValues.floatValues)
                    {
                        {
                            Port inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(float), Port.Capacity.Single);
                            inputPort.portName = value.Key;
                            inputPort.portColor = Color.white;

                            dynamicValuesContainer.Add(inputPort);
                            thisNode.dynamicValuesPorts.Add(inputPort);
                        }
                    }

                    // --- BOOL VALUES
                    if (((AI_ValueChanger)thisNode.valueChangerField.value).dynamicValues.boolValues.Count != 0)
                    {
                        Label label = new Label();
                        label.text = "BOOLEANs:";
                        dynamicValuesContainer.Add(label);
                    }
                    foreach (var value in ((AI_ValueChanger)thisNode.valueChangerField.value).dynamicValues.boolValues)
                    {
                        {
                            Port inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, typeof(bool), Port.Capacity.Single);
                            inputPort.portName = value.Key;
                            inputPort.portColor = Color.white;

                            dynamicValuesContainer.Add(inputPort);
                            thisNode.dynamicValuesPorts.Add(inputPort);
                        }
                    }
                    // STRING VALUES
                    if (((AI_ValueChanger)thisNode.valueChangerField.value).dynamicValues.stringValues.Count != 0)
                    {
                        Label label = new Label();
                        label.text = "STRINGs:";
                        dynamicValuesContainer.Add(label);
                    }
                    foreach (var value in ((AI_ValueChanger)(thisNode.valueChangerField.value)).dynamicValues.stringValues)
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

                thisNode.nodeIsPrepared = true;

                thisNode.RefreshExpandedState();
                thisNode.RefreshPorts();
            }

            thisNode.RefreshExpandedState();
            thisNode.RefreshPorts();

            return thisNode;
        }


    }

}