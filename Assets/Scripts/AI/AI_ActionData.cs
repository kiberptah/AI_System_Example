
[System.Serializable]
public class AI_ActionData : AI_Data
{
    public AI_StateData stateData;
    public AI_Action action;


    public string nextActionGUID = "";



    public AI_ActionData(AI_ActionData data, AI_StateData stateData)
    {
        this.stateData = stateData;


        GUID = data.GUID;

        action = data.action;
        nextActionGUID = data.nextActionGUID;
    }

    public AI_ActionData()
    {

    }
}
