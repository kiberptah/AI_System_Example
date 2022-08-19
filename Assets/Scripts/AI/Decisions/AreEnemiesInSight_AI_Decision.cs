using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(menuName = "AI/Decisions/AreEnemiesInSight")]
public class AreEnemiesInSight_AI_Decision : AI_Decision
{
    protected override void AddDynamicValues()
    {

    }

    protected override void DoDecision(AI_Controller controller, AI_DecisionData decisionData)
    {
        if (seeAnEnemy(controller))
        {
            //Debug.Log("see enemy");
            decisionData.nextActionGUID = decisionData.trueGUID;
        }
        else
        {
            decisionData.nextActionGUID = decisionData.falseGUID;
        }
        
    }
    
    bool seeAnEnemy(AI_Controller controller)
    {
        foreach (var obj in controller.sightedObjects)
        {
            if (obj.tag == "Player")
            {
                controller.currentTarget = obj;
                return true;
            }                   
        }
        return false;
    }
}
