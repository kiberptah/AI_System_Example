using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEditor.Experimental.GraphView;

using System.IO;
using UnityEditor;


using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System.Linq;
using System.Threading.Tasks;

namespace AI_BehaviorEditor
{
    // Very ugly hard to read script that saves nodes and their data to a savefile and loads it back!
    public static class AIGraph_SaveUtility
    {
        static string saveFilePath = "Assets/Resources/AIGraph_SaveData/";

        #region useful methods
        static string FindNextNodeGUID(List<AI_EdgesData> edgesData, Port outputPort, string outputNodeGUID)
        {
            foreach (var edge in edgesData)
            {
                if (edge.outputPortName == outputPort.portName && edge.outputNodeGUID == outputNodeGUID)
                {
                    return edge.targetNodeGUID;
                }
            }
            return string.Empty;
        }
        static List<string> FindAllConnectedNodesGUIDs(List<AI_EdgesData> edgesData, Port outputPort, string outputNodeGUID)
        {
            List<string> guids = new List<string>();
            foreach (var edge in edgesData)
            {
                if (edge.outputPortName == outputPort.portName && edge.outputNodeGUID == outputNodeGUID)
                {
                    guids.Add(edge.targetNodeGUID);
                }
            }
            return guids;
        }
        static string FindTargetPortName(List<AI_EdgesData> edgesData, Port outputPort, string outputNodeGUID)
        {
            string targetPortName = "";
            foreach (var edge in edgesData)
            {
                if (edge.outputPortName == outputPort.portName && edge.outputNodeGUID == outputNodeGUID)
                {
                    targetPortName = edge.targetPortName;
                }
            }
            return targetPortName;
        }
        #endregion

        public static AI_SaveData SaveGraph(AIGraphView targetGraphView, AIGraph graph, bool saveAsNew)
        {
            AI_SaveData newSaveData = ScriptableObject.CreateInstance<AI_SaveData>();

            if (graph.filenameField.value == "")
            {
                graph.filenameField.value = "New Behavior";
            }
            newSaveData.name = graph.filenameField.value;


            // ----- SAVE EDGES
            {
                List<Edge> edges = targetGraphView.edges.ToList();
                foreach (var edge in edges)
                {
                    // --- to avoid visualgraph bug when you end up with unconnected edge, so it prevents it from saving
                    if (edge.output.node == null || edge.input.node == null)
                        continue;

                    var outputNode = edge.output.node as CustomNode;
                    var inputNode = edge.input.node as CustomNode;

                    newSaveData.edgesData.Add(new AI_EdgesData
                    {
                        outputPortName = edge.output.portName,
                        outputNodeGUID = outputNode.nodeData.GUID,

                        targetPortName = edge.input.portName,
                        targetNodeGUID = inputNode.nodeData.GUID
                    });
                }
            }


            // ----- SAVE NODES
            foreach (var node in targetGraphView.nodes)
            {
                ((CustomNode)node).nodeData.nodeRect = node.GetPosition();
                newSaveData.nodesData.Add(((CustomNode)node).nodeData);

                // Entry Node
                
                if (node.GetType() == typeof(EntryNode))
                {
                    newSaveData.firstStateGUID = "";
                    if (((EntryNode)node).outputPort.edgeConnector.target != null)
                    {
                        foreach (var edge in targetGraphView.edges)
                        {
                            if (edge.output == ((EntryNode)node).outputPort)
                            {
                                newSaveData.firstStateGUID = ((StateNode)edge.input.node).nodeData.GUID;
                                
                            }
                        }
                    }              
                }


                // State Node
                if (node.GetType() == typeof(StateNode))
                {
                    var tempNode = node as StateNode;
                    var data = new AI_StateData();

                    data.GUID = tempNode.nodeData.GUID;
                    //data.behaviorData = newSaveData.behaviorData;
                    data.stateName = tempNode.nameField.value;

                    data.firstLoopActionGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.actionsLoopPort, data.GUID);
                    data.firstOnEnterActionGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.onEnterStatePort, data.GUID);
                    data.firstOnExitActionGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.onExitStatePort, data.GUID);
                    data.firstTransitionCheckGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.transitionsChecksLoopPort, data.GUID);


                    foreach (var n in targetGraphView.nodes)
                    {
                        if (n.GetType() == typeof(ActionNode))
                        {
                            ActionNode anotherNode = n as ActionNode;
                            if (anotherNode.stateNodeGUID == data.GUID)
                            {
                                data.childNodesGUID.Add(anotherNode.nodeData.GUID);
                            }
                        }
                        if (n.GetType() == typeof(DecisionNode))
                        {
                            DecisionNode anotherNode = n as DecisionNode;
                            if (anotherNode.stateNodeGUID == data.GUID)
                            {
                                data.childNodesGUID.Add(anotherNode.nodeData.GUID);
                            }
                        }
                        if (n.GetType() == typeof(TimerNode))
                        {
                            TimerNode anotherNode = n as TimerNode;
                            if (anotherNode.stateNodeGUID == data.GUID)
                            {
                                data.childNodesGUID.Add(anotherNode.nodeData.GUID);
                            }
                        }
                        if (n.GetType() == typeof(ValueChangerNode))
                        {
                            ValueChangerNode anotherNode = n as ValueChangerNode;
                            if (anotherNode.stateNodeGUID == data.GUID)
                            {
                                data.childNodesGUID.Add(anotherNode.nodeData.GUID);
                            }
                        }
                        if (n.GetType() == typeof(ValueNode))
                        {
                            ValueNode anotherNode = n as ValueNode;
                            if (anotherNode.stateNodeGUID == data.GUID)
                            {
                                data.childNodesGUID.Add(anotherNode.nodeData.GUID);
                            }
                        }
                        if (n.GetType() == typeof(TransitionNode))
                        {
                            TransitionNode anotherNode = n as TransitionNode;
                            if (anotherNode.stateNodeGUID == data.GUID)
                            {
                                data.childNodesGUID.Add(anotherNode.nodeData.GUID);
                            }
                        }
                    }


                    newSaveData.statesData.Add(data);
                }

                // Action Node
                if (node.GetType() == typeof(ActionNode))
                {
                    var tempNode = node as ActionNode;
                    var data = new AI_ActionData();

                    data.GUID = tempNode.nodeData.GUID;
                    data.stateData = (newSaveData.statesData.Where(x => x.GUID == tempNode.stateNodeGUID)).FirstOrDefault();
                    
                    data.action = tempNode.actionField.value as AI_Action;

                    data.nextActionGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.outputPort, data.GUID);


                    newSaveData.actionsData.Add(data);
                }



                // Decision Node
                if (node.GetType() == typeof(DecisionNode))
                {
                    var tempNode = node as DecisionNode;
                    var data = new AI_DecisionData();

                    data.GUID = tempNode.nodeData.GUID;
                    data.stateData = (newSaveData.statesData.Where(x => x.GUID == tempNode.stateNodeGUID)).FirstOrDefault();

                    data.decision = tempNode.decisionField.value as AI_Decision;
                    
                    data.trueGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.truePort, data.GUID);
                    data.falseGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.falsePort, data.GUID);


                    newSaveData.decisionsData.Add(data);
                }



                // Timer Node
                if (node.GetType() == typeof(TimerNode))
                {
                    var tempNode = node as TimerNode;
                    var data = new AI_TimerData();

                    data.GUID = tempNode.nodeData.GUID;
                    data.stateData = (newSaveData.statesData.Where(x => x.GUID == tempNode.stateNodeGUID)).FirstOrDefault();

                    data.timeInterval = tempNode.intervalField.value;
                    data.intervalRandomOffset = tempNode.randomOffsetField.value;

                    data.trueGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.truePort, data.GUID);
                    data.falseGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.falsePort, data.GUID);


                    newSaveData.timersData.Add(data);
                }


                // Value Node
                if (node.GetType() == typeof(ValueNode))
                {
                    var tempNode = node as ValueNode;
                    var data = new AI_ValueData();

                    if (tempNode.nodeIsPrepared)
                    {

                        data.GUID = tempNode.nodeData.GUID;
                        data.stateData = (newSaveData.statesData.Where(x => x.GUID == tempNode.stateNodeGUID)).FirstOrDefault();

                        data.valueName = FindTargetPortName(newSaveData.edgesData, tempNode.outputPort, data.GUID);
                        data.targetGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.outputPort, data.GUID);


                        {
                            if (tempNode.typeField.value == typeof(int))
                            {
                                data.valueType = typeof(int).ToString();
                                data.intValue = tempNode.intField.value;
                            }
                            if (tempNode.typeField.value == typeof(float))
                            {
                                data.valueType = typeof(float).ToString();
                                data.floatValue = tempNode.floatField.value;
                            }
                            if (tempNode.typeField.value == typeof(bool))
                            {
                                data.valueType = typeof(bool).ToString();
                                data.boolValue = tempNode.boolToggle.value;
                            }
                            if (tempNode.typeField.value == typeof(string))
                            {
                                data.valueType = typeof(string).ToString();
                                data.stringValue = tempNode.stringField.value;
                            }
                        }


                        newSaveData.valuesData.Add(data);
                    }
                }


                // ValueChanger Node
                if (node.GetType() == typeof(ValueChangerNode))
                {
                    var tempNode = node as ValueChangerNode;
                    var data = new AI_ValueChangerData();

                    if (tempNode.nodeIsPrepared)
                    {
                        data.GUID = tempNode.nodeData.GUID;
                        data.stateData = (newSaveData.statesData.Where(x => x.GUID == tempNode.stateNodeGUID)).FirstOrDefault();

                        data.valueChanger = (AI_ValueChanger)tempNode.valueChangerField.value;


                        //data.valueType = typeof(int).ToString();
                        data.valueType = tempNode.typeField.value.ToString();

                        data.valuesGUIDs = FindAllConnectedNodesGUIDs(newSaveData.edgesData, tempNode.valuePort, data.GUID);

                        data.nextActionGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.outputPort, data.GUID);


                        newSaveData.valueChangersData.Add(data);
                    }          
                }


                // Transition Node
                if (node.GetType() == typeof(TransitionNode))
                {
                    var tempNode = node as TransitionNode;
                    var data = new AI_TransitionData();

                    data.GUID = tempNode.nodeData.GUID;
                    data.stateData = (newSaveData.statesData.Where(x => x.GUID == tempNode.stateNodeGUID)).FirstOrDefault();


                    data.nextStateGUID = FindNextNodeGUID(newSaveData.edgesData, tempNode.outputPort, data.GUID);


                    newSaveData.transitionsData.Add(data);
                }
            }


            // ----- SAVE SCRIPTABLE OBJECT OF A SAVE FILE TO A SPECIAL FOLDER
            {
                // ----- create resources folder if it doesnt exist
                Directory.CreateDirectory(saveFilePath);

                if (saveAsNew == true || graph.behaviorField.value == null)
                {
                    SaveNew(newSaveData);
                }
                else
                {
                    // -----  OVERRIDE it if file already exists!!! important overwise references in scene get reset
                    AI_SaveData existingContainer = Resources.Load<AI_SaveData>("AIGraph_SaveData/" + newSaveData.name);
                    if (existingContainer == null)
                    {
                        SaveNew(newSaveData);
                    }
                    else
                    {
                        SaveReplace(newSaveData, existingContainer);
                    }
                }
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            return newSaveData;




            void SaveNew(AI_SaveData saveData)
            {
                int i = 0;
                string filename = saveData.name;
                bool filenameAlreadyTaken = true;

                while (filenameAlreadyTaken == true)
                {
                    AI_SaveData existingContainer = Resources.Load<AI_SaveData>("AIGraph_SaveData/" + filename);
                    if (existingContainer == null)
                    {
                        filenameAlreadyTaken = false;
                        saveData.name = filename;
                        break;
                    }


                    if (i >= 99)
                    {
                        saveData.name = filename;

                        SaveReplace(saveData, existingContainer);
                        return;
                    }


                    ++i;
                    filename = saveData.name + " " + i;
                }


                Debug.Log("AI BEHAVIOR SAVED: NEW DATA CREATED");
                AssetDatabase.CreateAsset(saveData, path: saveFilePath + $"{filename}.asset");
                //Clear Dirty??? no reason to do it yet


            }

            void SaveReplace(AI_SaveData saveData, AI_SaveData existingContainer)
            {
                Debug.Log("AI BEHAVIOR SAVED: OLD DATA REPLACED");
                EditorUtility.SetDirty(existingContainer); // there's data loss on reload without this shit
                //existingContainer = saveData; it would be easier but it doesnt work and i am too much of a code monkey to figure out proper way to clone an object
                //Debug.Log("existingContainer.firstStateGUID = " + existingContainer.firstStateGUID);             
                existingContainer.firstStateGUID = saveData.firstStateGUID;

                existingContainer.edgesData = saveData.edgesData;
                existingContainer.nodesData = saveData.nodesData;

                existingContainer.statesData = saveData.statesData;

                existingContainer.actionsData = saveData.actionsData;
                existingContainer.decisionsData = saveData.decisionsData;
                existingContainer.timersData = saveData.timersData;
                existingContainer.valuesData = saveData.valuesData;
                existingContainer.valueChangersData = saveData.valueChangersData;
                existingContainer.transitionsData = saveData.transitionsData;
            }
            
        }



        

        public static void LoadGraph(AIGraphView targetGraphView, AI_SaveData saveData)
        {       
            // --- Clear GraphView 
            targetGraphView.ClearGraph();

            // --- Load Nodes
            GenerateLoadedNodes();

            // --- Load Edges
            ConnectLoadedNodes();

            

            void GenerateLoadedNodes()
            {

                foreach (var data in saveData.statesData)
                {
                    StateNode node = new StateNode(targetGraphView);
                    node.nodeData.GUID = data.GUID;


                    node.nameField.value = data.stateName;
                    node.childNodesGUID = data.childNodesGUID;

                    StateNode.Create_StateNode(node);
                    targetGraphView.AddElement(node);
                }


                foreach (var data in saveData.actionsData)
                {
                    ActionNode node = new ActionNode();
                    node.nodeData.GUID = data.GUID;

                    node.stateNodeGUID = data.stateData.GUID;
                    node.actionField.value = data.action;

                    ActionNode.Create_ActionNode(node);
                    targetGraphView.AddElement(node);
                }

                foreach (var data in saveData.decisionsData)
                {
                    DecisionNode node = new DecisionNode();
                    
                    node.nodeData.GUID = data.GUID;

                    node.stateNodeGUID = data.stateData.GUID;
                    node.decisionField.value = data.decision;

                    DecisionNode.Create_DecisionNode(node);
                    targetGraphView.AddElement(node);
                }

                foreach (var data in saveData.timersData)
                {
                    TimerNode node = new TimerNode();
                    node.nodeData.GUID = data.GUID;

                    node.stateNodeGUID = data.stateData.GUID;
                    node.intervalField.value = data.timeInterval;
                    node.randomOffsetField.value = data.intervalRandomOffset;

                    TimerNode.Create_TimerNode(node);
                    targetGraphView.AddElement(node);
                }

                foreach (var data in saveData.valuesData)
                {
                    ValueNode node = new ValueNode();

                    node.nodeData.GUID = data.GUID;

                    node.stateNodeGUID = data.stateData.GUID;
                    node.typeField.value = System.Type.GetType(data.valueType);

                    node.intField.value = data.intValue;
                    node.floatField.value = data.floatValue;
                    node.boolToggle.value = data.boolValue;
                    node.stringField.value = data.stringValue;


                    ValueNode.Create_ValueNode(node);
                    targetGraphView.AddElement(node);
                }

                foreach (var data in saveData.valueChangersData)
                {
                    ValueChangerNode node = new ValueChangerNode();
                    node.nodeData.GUID = data.GUID;

                    node.stateNodeGUID = data.stateData.GUID;
                    node.typeField.value = System.Type.GetType(data.valueType);
                    node.valueChangerField.value = data.valueChanger;

                    ValueChangerNode.Create_ValueChangerNode(node);
                    targetGraphView.AddElement(node);
                }

                foreach (var data in saveData.transitionsData)
                {
                    TransitionNode node = new TransitionNode();
                    node.nodeData.GUID = data.GUID;

                    node.stateNodeGUID = data.stateData.GUID;

                    TransitionNode.Create_TransitionNode(node);
                    targetGraphView.AddElement(node);
                }


                

                // Load nodes positions
                foreach (var nodeData in saveData.nodesData)
                {
                    CustomNode tempNode = targetGraphView.FindCustomNodeByGUID(nodeData.GUID);
                    if (tempNode != null)
                    {
                        tempNode.SetPosition(nodeData.nodeRect);
                    }
                }


            }

            void ConnectLoadedNodes()
            {
                /// we have generated nodes and ports and now we need to connect them all
                ///
                var loadedEdges = saveData.edgesData;
                foreach (var loadedEdge in loadedEdges)
                {
                    var tempEdge = new Edge();

                    foreach (var port in targetGraphView.ports)
                    {
                        CustomNode portNode = (CustomNode)port.node;
                        if (port.portName == loadedEdge.outputPortName && portNode.nodeData.GUID == loadedEdge.outputNodeGUID)
                        {
                            tempEdge.output = port;                        
                        }
                        else if (port.portName == loadedEdge.targetPortName && portNode.nodeData.GUID == loadedEdge.targetNodeGUID)
                        {
                            tempEdge.input = port;
                        }
                    }
                    
                    if (tempEdge.input != null && tempEdge.output != null)
                    {
                        tempEdge.input.Connect(tempEdge);
                        tempEdge.output.Connect(tempEdge);
                        targetGraphView.Add(tempEdge);
                    }             
                }
            }
            


            

        }
        
    }

    
}