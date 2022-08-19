using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using System;

namespace AI_BehaviorEditor
{
    public class AIGraphView : GraphView
    {
        private NodeSearchWindow searchWindow;
        public class Port_States { };
        public class Port_Actions { };



        public AIGraphView(AIGraph _editorWindow)
        {
            // --- Set up StyleSheets
            styleSheets.Add(styleSheet: Resources.Load<StyleSheet>(path: "StyleSheets/AIGraph")); 
            styleSheets.Add(styleSheet: Resources.Load<StyleSheet>(path: "StyleSheets/Nodes"));

            // --- Camera position and zoom (or content's I'm not sure) 
            //SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale); // without it there's no zoom
            viewTransform.position = new Vector3(100, 200, 0); // default camera position... sort of
            SetupZoom(.25f, 1); // zoom range. Without this there's no zoom!

            // --- Stuff from API to intercat with elements
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(ContextMenu_NewState()); // this one to create states on RMB click but it's obsolete since I added creation on edge realease

            // --- adds grid to the background, just for visuals
            var grid = new GridBackground();
            Insert(index: 0, grid);
            grid.StretchToParentSize();

            // --- Entry Node is a part of any graphview
            AddElement(EntryNode.Create_EntryNode(new EntryNode()));


            AddSearchWindow(_editorWindow);
        }
        public void ClearGraph()
        {
            foreach (Node node in nodes)
            {
                RemoveElement(node);
            }
            foreach (Edge edge in edges)
            {
                RemoveElement(edge);
            }

            // --- Entry Node is a part of any graphview
            AddElement(EntryNode.Create_EntryNode(new EntryNode()));
        }
        IManipulator ContextMenu_NewState()
        {
            /// This thing is sorta obsolete cause now new states can be created by simply dragging an edge from state type port
            /// 


            // --- Button to add state nodes via context menu
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction("Add State Node", actionEvent => Create(actionEvent))
                );

            void Create(DropdownMenuAction actionEvent)
            {
                StateNode newNode = StateNode.Create_StateNode(new StateNode(this));
                newNode.SetPosition(new Rect(viewTransform.matrix.inverse.MultiplyPoint(actionEvent.eventInfo.localMousePosition), Vector3.zero)); //no idea how it works
                AddElement(newNode);
            }
            return contextualMenuManipulator;
        }




        private void AddSearchWindow(AIGraph graphWindow)
        {
            /// Adds node creation window that pops up when you release an edge


            searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            searchWindow.Configure(graphWindow, this);
            /* //This would add "Create Node" button to context menu that summons searchwindow

            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), searchWindow);
            */
        }

        class MyIEdgeConnectorListener : IEdgeConnectorListener
        {
            // Very important class instance of which are added to ports so there's callback for edge release


            public void OnDrop(GraphView graphView, Edge edge) { }

            public void OnDropOutsidePort(Edge edge, Vector2 position)
            {
                var graphView = AIGraph.graphView;
                // --- When Dragging Output State Port
                if (edge.output?.portType == typeof(Port_States))
                {
                    // When we drag and release an edge from State type port it creates new statenode and connects it

                    StateNode newNode = StateNode.Create_StateNode(new StateNode(graphView));
                    newNode.SetPosition(new Rect(graphView.viewTransform.matrix.inverse.MultiplyPoint(edge.candidatePosition), Vector3.zero));
                    graphView.AddElement(newNode);
                    AIGraph.graphView.TryToConnectTwoPorts(edge.output, newNode.inputPort);
                }
                // --- When Dragging Output/Input Action Port
                if (edge.output?.portType == typeof(Port_Actions) || edge.input?.portType == typeof(Port_Actions))
                {
                    Port originPort;
                    if (edge.output != null)
                    {
                        originPort = edge.output;
                    }
                    else
                    {
                        originPort = edge.input;
                    }

                    // find state node this node belongs to
                    if (originPort.node as StateNode != null)
                    {
                        // if node itself is a state node
                        graphView.searchWindow.stateNode = originPort.node as StateNode;                      
                    }
                    else
                    {
                        foreach (var node in graphView.nodes)
                        {
                            if (node.GetType() == typeof(StateNode))
                            {
                                // Find state node containing GUID for this action node
                                if (((StateNode)node).childNodesGUID.Contains(((CustomNode)originPort.node).nodeData.GUID))
                                {
                                    graphView.searchWindow.stateNode = (StateNode)node;                                  
                                }
                            }
                        }
                    }

                    // Set up data for potantial new node and open search window
                    graphView.searchWindow.newNodeRect = new Rect(graphView.viewTransform.matrix.inverse.MultiplyPoint(edge.candidatePosition), Vector3.zero);
                    graphView.searchWindow.originPort = originPort;

                    SearchWindow.Open(new SearchWindowContext(Vector2.zero), graphView.searchWindow);
                    SearchWindow.focusedWindow.position = new Rect(edge.candidatePosition, Vector3.zero);
                }

                // --- When Dragging Dynamic Values InputPort => Create Value Node
                if (edge.input?.portType == typeof(int)
                        || edge.input?.portType == typeof(float)
                        || edge.input?.portType == typeof(bool)
                        || edge.input?.portType == typeof(string))
                {
                    StateNode stateNode = null;
                    // find state node this node belongs to
                    if (edge.input.node as StateNode != null)
                    {
                        // if node itself is a state node
                        stateNode = edge.input.node as StateNode;
                    }
                    else
                    {
                        foreach (var node in graphView.nodes)
                        {
                            if (node.GetType() == typeof(StateNode))
                            {
                                // Find state node containing GUID for this action node
                                if (((StateNode)node).childNodesGUID.Contains(((CustomNode)edge.input.node).nodeData.GUID))
                                {
                                    stateNode = (StateNode)node;
                                }
                            }
                        }
                    }

                    if (edge.input.node as ValueNode != null) // value input shouldnt create another value node! no use for that
                    {
                        ValueChangerNode valueChangerNode = new ValueChangerNode();
                        valueChangerNode.typeField.value = edge.input?.portType;
                        stateNode.AddChild(valueChangerNode);
                        valueChangerNode.stateNodeGUID = stateNode.nodeData.GUID;

                        var newNodeRect = new Rect(graphView.viewTransform.matrix.inverse.MultiplyPoint(edge.candidatePosition), Vector3.zero);
                        valueChangerNode.SetPosition(newNodeRect);

                        graphView.AddElement(ValueChangerNode.Create_ValueChangerNode(valueChangerNode));
                        graphView.TryToConnectTwoPorts(valueChangerNode.valuePort, edge.input);
                    }

                    else // instead value input should create value changer node
                    {
                        ValueNode valueNode = new ValueNode();
                        valueNode.typeField.value = edge.input?.portType;
                        stateNode.AddChild(valueNode);
                        valueNode.stateNodeGUID = stateNode.nodeData.GUID;

                        var newNodeRect = new Rect(graphView.viewTransform.matrix.inverse.MultiplyPoint(edge.candidatePosition), Vector3.zero);
                        valueNode.SetPosition(newNodeRect);

                        graphView.AddElement(ValueNode.Create_ValueNode(valueNode));
                        graphView.TryToConnectTwoPorts(valueNode.outputPort, edge.input);                 
                    }
                }              
            }          
        }
        

        
      
        public CustomNode FindCustomNodeByGUID(string GUID)
        {
            foreach (CustomNode node in nodes)
            {
                if (node.nodeData.GUID == GUID)
                {
                    return node;
                }
            }
            return null;
        }




        public void TryToConnectTwoPorts(Port port_1, Port port_2)
        {
            if (port_1.direction != port_2.direction && port_1.portType == port_2.portType)
            {
                Port output = port_1;
                Port input = port_2;

                if (port_1.direction == Direction.Input)
                {
                    output = port_2;
                    input = port_1;
                }

                Edge newEdge = new Edge();
                AddElement(newEdge);

                if (input.capacity == Port.Capacity.Single)
                {
                    DeleteElements(input.connections);
                }
                if (output.capacity == Port.Capacity.Single)
                {
                    DeleteElements(output.connections);
                }

                input.Connect(newEdge);
                output.Connect(newEdge);

                newEdge.input = input;
                newEdge.output = output;
            }

        }






        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            /// Overrides some default Unity function which is called when you start dragging from an output port
            /// Without this functionn you can't connect ports
            /// also it has logic to prevent illogical node connections



            var compatablePorts = new List<Port>();
            ports.ForEach(targetPort =>
            {
                if (startPort != targetPort
                 && startPort.node != targetPort.node
                 && startPort.direction != targetPort.direction
                 && startPort.portType == targetPort.portType)
                {
                    compatablePorts.Add(targetPort);
                }
            });

            return compatablePorts;
        }
        public static Port GeneratePort(Node _node, Direction _portDirection, Type type, Port.Capacity _capacity = Port.Capacity.Single, Orientation orientation = Orientation.Horizontal)
        {
            Port port = _node.InstantiatePort(orientation, _portDirection, _capacity, type); 

            // style for label, usually to hide it
            Label portLabel = port.contentContainer.Q<Label>(name: "type");
            portLabel.AddToClassList("port-label");
            //port.contentContainer.AddToClassList("port-container");

            port.AddManipulator(new EdgeConnector<Edge>(new MyIEdgeConnectorListener())); // this thing is needed to detect edges release (to create menu when edge dropped outside port)

            return port;
        }


        // I moved functions below in separate classes for convenience 


        /*
        public EntryNode Create_EntryNode(EntryNode thisNode)
        {
            thisNode.nodeData.GUID = "0";

            thisNode.title = "ENTRY";

            thisNode.capabilities &= ~Capabilities.Deletable;
            thisNode.capabilities &= ~Capabilities.Movable;
            thisNode.capabilities &= ~Capabilities.Copiable;
            //thisNode.capabilities &= ~Capabilities.Collapsible; // doesn't work anyway

            // --- Generate Output Port
            {
                thisNode.outputPort = GeneratePort(thisNode, Direction.Output, typeof(Port_States), Port.Capacity.Single);
                thisNode.outputPort.portName = "Output";
                thisNode.outputPort.portColor = Color.red;

                thisNode.outputContainer.Add(thisNode.outputPort);
            }

            thisNode.RefreshExpandedState();
            thisNode.RefreshPorts(); // better be safe than sorry

            return thisNode;
        }
        */

        /*
        public StateNode Create_StateNode(StateNode thisNode)
        {
            thisNode.title = "STATE";
            thisNode.mainContainer.contentContainer.AddToClassList("contentContainer"); // To make it not transparent

            // --- State Name Field
            {
                thisNode.nameField.AddToClassList("stateNodeNameField");
                thisNode.mainContainer.contentContainer.Add(thisNode.nameField);
            }

            // --- Generate Input Port
            {
                thisNode.inputPort = GeneratePort(thisNode, Direction.Input, typeof(Port_States), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Input";
                thisNode.inputPort.portColor = Color.red;
                thisNode.inputContainer.Add(thisNode.inputPort);
            }

            // --- Generate Output Ports
            {
                // Port for onEnter actions
                thisNode.onEnterStatePort = GeneratePort(thisNode, Direction.Output, typeof(Port_Actions), Port.Capacity.Single);
                thisNode.onEnterStatePort.portName = "onEnter";
                thisNode.onEnterStatePort.portColor = new Color(1, 0.6f, 0);
                thisNode.outputContainer.Add(thisNode.onEnterStatePort);

                // Port for actions loop
                thisNode.actionsLoopPort = GeneratePort(thisNode, Direction.Output, typeof(Port_Actions), Port.Capacity.Single);
                thisNode.actionsLoopPort.portName = "Loop";
                thisNode.actionsLoopPort.portColor = Color.yellow;
                thisNode.outputContainer.Add(thisNode.actionsLoopPort);

                // Port for decisions loop
                thisNode.transitionsChecksLoopPort = GeneratePort(thisNode, Direction.Output, typeof(Port_Actions), Port.Capacity.Single);
                thisNode.transitionsChecksLoopPort.portName = "Decisions";
                thisNode.transitionsChecksLoopPort.portColor = Color.magenta;
                thisNode.outputContainer.Add(thisNode.transitionsChecksLoopPort);

                // Port for onExit actions
                thisNode.onExitStatePort = GeneratePort(thisNode, Direction.Output, typeof(Port_Actions), Port.Capacity.Single);
                thisNode.onExitStatePort.portName = "onExit";
                thisNode.onExitStatePort.portColor = new Color(1, 0.6f, 0);
                thisNode.outputContainer.Add(thisNode.onExitStatePort);
            }
            
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
            
        thisNode.RefreshExpandedState();
            thisNode.RefreshPorts();

            return thisNode;
        }
    */

        /*
        public ActionNode Create_ActionNode(ActionNode thisNode)
        {
            thisNode.title = "ACTION";
            thisNode.mainContainer.contentContainer.AddToClassList("contentContainer");

            thisNode.mainContainer.Add(thisNode.actionField);
            Box dynamicValuesContainer = new Box();
            thisNode.mainContainer.Add(dynamicValuesContainer);

            // --- Generate Input Port
            {
                thisNode.inputPort = GeneratePort(thisNode, Direction.Input, typeof(Port_Actions), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Prev";
                thisNode.inputPort.portColor = Color.yellow;

                thisNode.inputContainer.Add(thisNode.inputPort);
            }
            // --- Generate Output Port
            {
                thisNode.outputPort = GeneratePort(thisNode, Direction.Output, typeof(Port_Actions), Port.Capacity.Single);
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
                            Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(int), Port.Capacity.Multi);
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
                            Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(float), Port.Capacity.Multi);
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
                            Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(bool), Port.Capacity.Multi);
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
                            Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(string), Port.Capacity.Multi);
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
        */

        /*
        public DecisionNode Create_DecisionNode(DecisionNode thisNode)
        {
            thisNode.title = "DECISION";
            thisNode.mainContainer.contentContainer.AddToClassList("contentContainer");

            thisNode.mainContainer.Add(thisNode.decisionField);
            Box dynamicValuesContainer = new Box();
            thisNode.mainContainer.Add(dynamicValuesContainer);

            // --- Generate Input Port
            {
                thisNode.inputPort = GeneratePort(thisNode, Direction.Input, typeof(Port_Actions), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Input";
                thisNode.inputPort.portColor = Color.magenta;

                thisNode.inputContainer.Add(thisNode.inputPort);
            }

            // --- Generate Output Ports
            {
                thisNode.truePort = GeneratePort(thisNode, Direction.Output, typeof(Port_Actions), Port.Capacity.Single);
                thisNode.truePort.portName = "True";
                thisNode.truePort.portColor = Color.magenta;
                thisNode.outputContainer.Add(thisNode.truePort);

                thisNode.falsePort = GeneratePort(thisNode, Direction.Output, typeof(Port_Actions), Port.Capacity.Single);
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
                                Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(int), Port.Capacity.Multi);
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
                                Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(float), Port.Capacity.Multi);
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
                                Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(bool), Port.Capacity.Multi);
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
                                Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(string), Port.Capacity.Multi);
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
        */

        /*
        public ValueNode Create_ValueNode(ValueNode thisNode)
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
                    RemoveElement(thisNode); // to prevent impossible errors?..
                    Debug.Log("ERROR: CREATING NODE WITH UNSUPPORTED VALUE TYPE = " + valueType.ToString());
                }

                



                void AddPorts(Type valueType)
                {
                    // --- Generate Input Port
                    {
                        thisNode.inputPort = GeneratePort(thisNode, Direction.Input, valueType, Port.Capacity.Multi);
                        thisNode.inputPort.portColor = Color.white;
                        thisNode.inputPort.portName = "Input";
                        thisNode.inputPort.contentContainer.Q<Label>(name: "type").AddToClassList("port-labelHidden");
                        thisNode.inputContainer.Add(thisNode.inputPort);
                    }

                    // --- Generate Output Port
                    {
                        thisNode.outputPort = GeneratePort(thisNode, Direction.Output, valueType, Port.Capacity.Multi);
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
        */
        /*
        public ValueChangerNode Create_ValueChangerNode(ValueChangerNode thisNode)
        {
            thisNode.title = "Value Changer";
            thisNode.mainContainer.contentContainer.AddToClassList("contentContainer");
            Box dynamicValuesContainer = new Box();


            // --- Generate Input Port
            {
                thisNode.inputPort = GeneratePort(thisNode, Direction.Input, typeof(Port_Actions), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Input";
                thisNode.inputPort.portColor = Color.green;

                thisNode.inputContainer.Add(thisNode.inputPort);
            }
            // --- Generate Output Port
            {
                thisNode.outputPort = GeneratePort(thisNode, Direction.Output, typeof(Port_Actions), Port.Capacity.Single);
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
                        RemoveElement(thisNode); // to prevent impossible errors?..
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
                        thisNode.valuePort = GeneratePort(thisNode, Direction.Output, type, Port.Capacity.Multi);
                        thisNode.valuePort.portName = "Value";
                        thisNode.valuePort.portColor = Color.white;

                        thisNode.mainContainer.Add(thisNode.valuePort);
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
                            Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(int), Port.Capacity.Single);
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
                            Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(float), Port.Capacity.Single);
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
                            Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(bool), Port.Capacity.Single);
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
                            Port inputPort = GeneratePort(thisNode, Direction.Input, typeof(string), Port.Capacity.Multi);
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

        */
        /*
        public TimerNode Create_TimerNode(TimerNode thisNode)    
        {
            thisNode.title = "Timer";
            thisNode.mainContainer.contentContainer.AddToClassList("contentContainer");

            // --- Generate Input Port
            {
                thisNode.inputPort = GeneratePort(thisNode, Direction.Input, typeof(Port_Actions), Port.Capacity.Multi);
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
                thisNode.falsePort = GeneratePort(thisNode, Direction.Output, typeof(Port_Actions), Port.Capacity.Single);
                thisNode.falsePort.portName = "False";
                thisNode.falsePort.portColor = Color.cyan;
                thisNode.outputContainer.Add(thisNode.falsePort);

                thisNode.truePort = GeneratePort(thisNode, Direction.Output, typeof(Port_Actions), Port.Capacity.Single);
                thisNode.truePort.portName = "True";
                thisNode.truePort.portColor = Color.cyan;
                thisNode.outputContainer.Add(thisNode.truePort);              
            }

            return thisNode;
        }
        */

        /*
        public TransitionNode Create_TransitionNode(TransitionNode thisNode)
        {
            thisNode.title = "TRANSITION";

            // --- Generate Input Port
            {
                thisNode.inputPort = GeneratePort(thisNode, Direction.Input, typeof(Port_Actions), Port.Capacity.Multi);
                thisNode.inputPort.portName = "Decision";
                thisNode.inputPort.portColor = Color.yellow;

                thisNode.inputContainer.Add(thisNode.inputPort);
            }

            // --- Generate Output Port
            {
                thisNode.outputPort = GeneratePort(thisNode, Direction.Output, typeof(Port_States), Port.Capacity.Single);
                thisNode.outputPort.portName = "Next State";
                thisNode.outputPort.portColor = Color.red;

                thisNode.outputContainer.Add(thisNode.outputPort);
            }

            return thisNode;
        }*/
    }
}
