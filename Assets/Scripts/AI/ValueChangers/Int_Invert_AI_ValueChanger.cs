using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/ValueChanger/Int_Invert")]
public class Int_Invert_AI_ValueChanger : AI_ValueChanger_Int
{
    protected override void AddDynamicValues()
    {

    }
    protected override void ChangeValue_Int(AI_StateData stateData, AI_ValueData data)
    {
        data.intValue = -data.intValue;

    }
}
