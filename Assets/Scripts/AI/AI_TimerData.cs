


[System.Serializable]
public class AI_TimerData : AI_Data
{
    public AI_StateData stateData;

    public float timeInterval = 0;
    public float intervalRandomOffset = 0;

    public float currentRandomOffset = 0;
    public float currentTime = 0;



    public string trueGUID = "";
    public string falseGUID = "";

    public string nextGUID = "";


    public AI_TimerData(AI_TimerData data, AI_StateData stateData)
    {
        this.stateData = stateData;


        GUID = data.GUID;

        timeInterval = data.timeInterval;
        intervalRandomOffset = data.intervalRandomOffset;

        trueGUID = data.trueGUID;
        falseGUID = data.falseGUID;
    }

    public AI_TimerData()
    {
 
    }
}
