using UnityEngine;


using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System;






namespace AI_BehaviorEditor
{
    public class ValueNode : CustomNode
    {
        //public ValueNode_Data data = new ValueNode_Data();
        public string stateNodeGUID;

        public Port inputPort;
        public Port outputPort;

        public bool nodeIsPrepared = false;



        public PopupField<Type> typeField = new PopupField<Type>
        {
            choices = new List<Type> // if there's protection level error then use later version of unity, 2021.2.15 worked fine but eerlier 2021 didnt!
            {
                typeof(int),
                typeof(float),
                typeof(bool),
                typeof(string)
            }
        };


        public IntegerField intField = new IntegerField();
        public FloatField floatField = new FloatField();
        public Toggle boolToggle = new Toggle();
        public TextField stringField = new TextField();


        public static ValueNode Create_ValueNode(ValueNode thisNode)
        {
            thisNode.title = "Value";
            thisNode.inputContainer.AddToClassList("horizontal-arrangement");

            // Field to select type before finishing generating node
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
                    case Type type when type == typeof(string):
                        {
                            SetUpNode(typeof(string));
                            break;
                        }
                }
            });
            thisNode.mainContainer.Add(thisNode.typeField);

            // if we load node from savedata we shouldn't wait for value change to trigger
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
                    case Type type when type == typeof(string):
                        {
                            SetUpNode(typeof(string));
                            break;
                        }
                }
            }

            void SetUpNode(Type valueType)
            {
                AddPorts(valueType);


                thisNode.mainContainer.Remove(thisNode.typeField);

                thisNode.titleContainer.AddToClassList("valueNodeTitle");


                if (valueType == typeof(int))
                {
                    thisNode.intField.AddToClassList("valueNodeField");

                    thisNode.inputContainer.Add(thisNode.intField);
                }
                else if (valueType == typeof(float))
                {
                    thisNode.floatField.AddToClassList("valueNodeField");

                    thisNode.inputContainer.Add(thisNode.floatField);
                }
                else if (valueType == typeof(bool))
                {
                    thisNode.inputContainer.Add(thisNode.boolToggle);
                }
                else if (valueType == typeof(string))
                {
                    thisNode.inputContainer.Add(thisNode.stringField);
                }
                else
                {
                    AIGraph.graphView.RemoveElement(thisNode); // to prevent impossible errors?..
                    Debug.Log("ERROR: CREATING NODE WITH UNSUPPORTED VALUE TYPE = " + valueType.ToString());
                }





                void AddPorts(Type valueType)
                {
                    // --- Generate Input Port
                    {
                        thisNode.inputPort = AIGraphView.GeneratePort(thisNode, Direction.Input, valueType, Port.Capacity.Multi);
                        thisNode.inputPort.portColor = Color.white;
                        thisNode.inputPort.portName = "Input";
                        thisNode.inputPort.contentContainer.Q<Label>(name: "type").AddToClassList("port-labelHidden");
                        thisNode.inputContainer.Add(thisNode.inputPort);
                    }

                    // --- Generate Output Port
                    {
                        thisNode.outputPort = AIGraphView.GeneratePort(thisNode, Direction.Output, valueType, Port.Capacity.Multi);
                        thisNode.outputPort.portColor = Color.white;
                        thisNode.outputPort.portName = "Output";
                        //outputPort.contentContainer.Q<Label>(name: "type").AddToClassList("port-labelHidden");
                        thisNode.outputContainer.Add(thisNode.outputPort);
                    }
                }

                thisNode.nodeIsPrepared = true; // so we won't bother with saving unsetuped nodes

                thisNode.RefreshExpandedState();
                thisNode.RefreshPorts();
            }

            thisNode.RefreshExpandedState();
            thisNode.RefreshPorts();

            return thisNode;
        }


    }


}