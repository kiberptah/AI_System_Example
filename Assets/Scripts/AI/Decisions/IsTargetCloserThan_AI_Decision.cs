using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AI/Decisions/IsTargetCloserThan")]
public class IsTargetCloserThan_AI_Decision : AI_Decision
{
    protected override void AddDynamicValues()
    {
        dynamicValues.floatValues.Add("distance", 0);
    }
    protected override void DoDecision(AI_Controller controller, AI_DecisionData decisionData)
    {
        if (Vector3.Distance(controller.currentTarget.position, controller.transform.position) < dynamicValues.floatValues["distance"])
        {
            decisionData.nextActionGUID = decisionData.trueGUID;
        }
        else
        {
            decisionData.nextActionGUID = decisionData.falseGUID;
        }
    }
}
