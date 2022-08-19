using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class AI_SaveData : ScriptableObject
{
    public string firstStateGUID;

    public List<AI_EdgesData> edgesData = new List<AI_EdgesData>();
    public List<CustomNode_Data> nodesData = new List<CustomNode_Data>();



    public List<AI_StateData> statesData = new List<AI_StateData>();

    public List<AI_ActionData> actionsData = new List<AI_ActionData>();
    public List<AI_DecisionData> decisionsData = new List<AI_DecisionData>();
    public List<AI_TimerData> timersData = new List<AI_TimerData>();
    public List<AI_ValueData> valuesData = new List<AI_ValueData>();
    public List<AI_ValueChangerData> valueChangersData = new List<AI_ValueChangerData>();
    public List<AI_TransitionData> transitionsData = new List<AI_TransitionData>();

}


