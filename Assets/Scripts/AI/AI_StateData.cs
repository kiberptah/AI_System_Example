using System.Collections.Generic;


[System.Serializable]
public class AI_StateData : AI_Data
{
    public AI_BehaviorData behaviorData;

    public string stateName = "";


    public string nextActionGUID = "";


    public string firstLoopActionGUID = "";

    public string firstOnEnterActionGUID = "";
    public string firstOnExitActionGUID = "";

    public string firstTransitionCheckGUID = "";




    public List<string> childNodesGUID = new List<string>();

    
    public Dictionary<string, AI_ActionData> actionsData = new Dictionary<string, AI_ActionData>();
    public Dictionary<string, AI_DecisionData> decisionsData = new Dictionary<string, AI_DecisionData>();
    public Dictionary<string, AI_TimerData> timersData = new Dictionary<string, AI_TimerData>();
    public Dictionary<string, AI_ValueData> valuesData = new Dictionary<string, AI_ValueData>();
    public Dictionary<string, AI_ValueChangerData> valueChangersData = new Dictionary<string, AI_ValueChangerData>();
    public Dictionary<string, AI_TransitionData> transitionsData = new Dictionary<string, AI_TransitionData>();
    

    public AI_StateData(AI_StateData data)
    {
        GUID = data.GUID;

        stateName = data.stateName;

        nextActionGUID = data.nextActionGUID;

        firstLoopActionGUID = data.firstLoopActionGUID;
        firstOnEnterActionGUID = data.firstOnEnterActionGUID;
        firstOnExitActionGUID = data.firstOnExitActionGUID;
        firstTransitionCheckGUID = data.firstTransitionCheckGUID;

        childNodesGUID = data.childNodesGUID;
    }

    public AI_StateData()
    {

    }

}


