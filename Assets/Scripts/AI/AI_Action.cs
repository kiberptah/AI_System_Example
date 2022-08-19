using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class AI_Action : ScriptableObject
{
    public AI_DynamicValues dynamicValues = new AI_DynamicValues();

    void Awake()
    {
        dynamicValues.ClearAll();
        AddDynamicValues();
    }
    void OnValidate()
    {
        dynamicValues.ClearAll();
        AddDynamicValues();
    }
    protected abstract void AddDynamicValues();


    public void Act(AI_Controller controller, AI_StateData stateData, AI_ActionData actionData)
    {
        UpdateValues(stateData, actionData);
        DoTheActions(controller, stateData, actionData);
    }
    protected abstract void DoTheActions(AI_Controller controller, AI_StateData stateData, AI_ActionData actionData);


    void UpdateValues(AI_StateData stateData, AI_ActionData actionData)
    {
        foreach (var valueData in stateData.valuesData)
        {
            if (valueData.Value.targetGUID == actionData.GUID)
            {
                if (dynamicValues.intValues.ContainsKey(valueData.Value.valueName))
                {
                    dynamicValues.intValues[valueData.Value.valueName] = valueData.Value.intValue;
                }
                if (dynamicValues.floatValues.ContainsKey(valueData.Value.valueName))
                {
                    dynamicValues.floatValues[valueData.Value.valueName] = valueData.Value.floatValue;
                }
                if (dynamicValues.boolValues.ContainsKey(valueData.Value.valueName))
                {
                    dynamicValues.boolValues[valueData.Value.valueName] = valueData.Value.boolValue;
                }
                if (dynamicValues.stringValues.ContainsKey(valueData.Value.valueName))
                {
                    dynamicValues.stringValues[valueData.Value.valueName] = valueData.Value.stringValue;
                }
            }
        }
    }
}