using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AI_ValueChangerData : AI_Data
{
    public AI_StateData stateData;

    public string nextActionGUID;

    public string valueType; // SO can not serialize system.type! so it is saved as string

    public AI_ValueChanger valueChanger = null;

    public List<string> valuesGUIDs = new List<string>();

    public AI_ValueChangerData(AI_ValueChangerData data, AI_StateData stateData)
    {
        this.stateData = stateData;


        GUID = data.GUID;

        nextActionGUID = data.nextActionGUID;
        valueType = data.valueType;

        valueChanger = data.valueChanger;

        valuesGUIDs = data.valuesGUIDs;
    }

    public AI_ValueChangerData()
    {

    }
}
