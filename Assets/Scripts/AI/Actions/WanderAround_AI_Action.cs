using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Actions/WanderAround")]
public class WanderAround_AI_Action : AI_Action
{
    protected override void AddDynamicValues()
    {
        dynamicValues.floatValues.Add("xDirection", 0);
        dynamicValues.floatValues.Add("yDirection", 0);
    }

    protected override void DoTheActions(AI_Controller controller, AI_StateData stateData, AI_ActionData actionData)
    {
        controller.input.Input_Movement(new Vector2(dynamicValues.floatValues["xDirection"], dynamicValues.floatValues["yDirection"]));
    }
}
