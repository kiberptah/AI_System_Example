


[System.Serializable]
public class AI_TransitionData : AI_Data
{
    public AI_StateData stateData;

    public string nextStateGUID = "";


    public AI_TransitionData(AI_TransitionData data, AI_StateData stateData)
    {
        this.stateData = stateData;


        GUID = data.GUID;
        nextStateGUID = data.nextStateGUID;
    }


    public AI_TransitionData()
    {

    }
}
