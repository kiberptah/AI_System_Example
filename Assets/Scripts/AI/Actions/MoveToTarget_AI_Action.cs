using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AI/Actions/MoveToTarget_AI_Action")]
public class MoveToTarget_AI_Action : AI_Action
{
    protected override void AddDynamicValues()
    {
    }

    protected override void DoTheActions(AI_Controller controller, AI_StateData stateData, AI_ActionData actionData)
    {
        controller.input.Input_Movement(controller.currentTarget.position - controller.actor.position);
    }
}
