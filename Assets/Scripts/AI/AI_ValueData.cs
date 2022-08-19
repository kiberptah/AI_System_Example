using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AI_ValueData : AI_Data
{
    public AI_StateData stateData;


    public string valueName;
    public string targetGUID;


    public string valueType;

    public int intValue = 0;
    public float floatValue = 0;
    public bool boolValue = false;
    public string stringValue = "";

    public AI_ValueData(AI_ValueData data, AI_StateData stateData)
    {
        this.stateData = stateData;

        GUID = data.GUID;

        valueName = data.valueName;
        targetGUID = data.targetGUID;

        valueType = data.valueType;

        intValue = data.intValue;
        floatValue = data.floatValue;
        boolValue = data.boolValue;
        stringValue = data.stringValue;
    }


    public AI_ValueData()
    {

    }
}
