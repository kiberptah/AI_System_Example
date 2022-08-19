







[System.Serializable]
public class AI_DecisionData : AI_Data
{
    public AI_StateData stateData;
    public AI_Decision decision;

    public string nextActionGUID = "";

    public string trueGUID = "";
    public string falseGUID = "";


    public AI_DecisionData(AI_DecisionData data, AI_StateData stateData)
    {
        this.stateData = stateData;


        GUID = data.GUID;

        decision = data.decision;

        nextActionGUID = data.nextActionGUID;
        trueGUID = data.trueGUID;
        falseGUID = data.falseGUID;
    }
    public AI_DecisionData()
    {

    }

}
