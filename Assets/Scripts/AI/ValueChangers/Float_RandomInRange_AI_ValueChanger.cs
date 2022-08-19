using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AI/ValueChanger/Float_RandomInRange")]
public class Float_RandomInRange_AI_ValueChanger : AI_ValueChanger_Float
{
    protected override void AddDynamicValues()
    {
        dynamicValues.floatValues.Add("min", -1);
        dynamicValues.floatValues.Add("max", 1);
    }
    protected override void ChangeValue_Float(AI_StateData stateData, AI_ValueData valueData)
    {
        
        valueData.floatValue = Random.Range(dynamicValues.floatValues["min"], dynamicValues.floatValues["max"]);
    }

}
