using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AI_ValueChanger : ScriptableObject
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



    public void ChangeValue(AI_StateData stateData, AI_ValueChangerData valueChangerData, AI_ValueData valueData)
    {
        UpdateValues(stateData, valueChangerData);
        DoValueChanging(stateData, valueData);
    }
    protected abstract void DoValueChanging(AI_StateData stateData, AI_ValueData valueData);


    void UpdateValues(AI_StateData stateData, AI_ValueChangerData valueChangerData)
    {
        foreach (var valueData in stateData.valuesData)
        {
            if (valueData.Value.targetGUID == valueChangerData.GUID)
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
public abstract class AI_ValueChanger_Int : AI_ValueChanger
{
    protected override void DoValueChanging(AI_StateData stateData, AI_ValueData valueData)
    {
        ChangeValue_Int(stateData, valueData);
    }
    protected abstract void ChangeValue_Int(AI_StateData stateData, AI_ValueData valueData);
}
public abstract class AI_ValueChanger_Float : AI_ValueChanger
{
    protected override void DoValueChanging(AI_StateData stateData, AI_ValueData valueData)
    {
        ChangeValue_Float(stateData, valueData);
    }
    protected abstract void ChangeValue_Float(AI_StateData stateData, AI_ValueData valueData);
}
public abstract class AI_ValueChanger_Bool : AI_ValueChanger
{
    protected override void DoValueChanging(AI_StateData stateData, AI_ValueData valueData)
    {
        ChangeValue_Bool(stateData, valueData);
    }
    protected abstract void ChangeValue_Bool(AI_StateData stateData, AI_ValueData valueData);
}