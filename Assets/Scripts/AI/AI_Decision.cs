using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public abstract class AI_Decision : ScriptableObject
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


    public void Decide(AI_Controller controller, AI_StateData stateData, AI_DecisionData decisionData)
    {
        UpdateValues(stateData, decisionData);
        DoDecision(controller, decisionData);
    }

    protected abstract void DoDecision(AI_Controller controller, AI_DecisionData decisionData);



    void UpdateValues(AI_StateData stateData, AI_DecisionData decisionData)
    {
        foreach (var valueData in stateData.valuesData)
        {
            if (valueData.Value.targetGUID == decisionData.GUID)
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
