using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class AI_State
{
    public static void EnterState(AI_StateData stateData)
    {
        foreach (var timerData in stateData.timersData)
        {
            AI_Timer.OnStateEnter(timerData.Value);
        }

        GoThroughActions(stateData, stateData.firstOnEnterActionGUID);
    }



    public static void StateLoop(AI_StateData stateData)
    {
        IncrementAllTimers(stateData);

        GoThroughActions(stateData, stateData.firstLoopActionGUID);

        GoThroughActions(stateData, stateData.firstTransitionCheckGUID);
    }



    public static void ExitState(AI_StateData stateData)
    {      
        GoThroughActions(stateData, stateData.firstOnExitActionGUID);        
    }








    static void GoThroughActions(AI_StateData stateData, string guid)
    {
        string currentActionGUID = guid;
        while (currentActionGUID != "")
        {
            if (stateData.actionsData.ContainsKey(currentActionGUID))
            {
                AI_ActionData thisData = stateData?.actionsData[currentActionGUID];

                //Debug.Log(stateData.behaviorData.controller);
                //Debug.Log(stateData);
                //Debug.Log(thisData);

                thisData?.action?.Act(stateData.behaviorData.controller, stateData, thisData);
                currentActionGUID = thisData?.nextActionGUID;
            }
            else if (stateData.decisionsData.ContainsKey(currentActionGUID))
            {
                AI_DecisionData thisData = stateData?.decisionsData[currentActionGUID];

                //Debug.Log(thisData.decision.name);

                stateData?.decisionsData[currentActionGUID]?.decision?.Decide(stateData.behaviorData.controller, stateData, thisData);
                currentActionGUID = thisData?.nextActionGUID;
            }
            else if (stateData.timersData.ContainsKey(currentActionGUID))
            {
                AI_TimerData thisData = stateData.timersData[currentActionGUID];
                currentActionGUID = thisData?.nextGUID;
            }
            else if (stateData.valueChangersData.ContainsKey(currentActionGUID))
            {
                AI_ValueChangerData thisData = stateData.valueChangersData[currentActionGUID];

                foreach(var value in thisData?.valuesGUIDs)
                {
                    thisData?.valueChanger?.ChangeValue(stateData, thisData, stateData.valuesData[value]);
                }
                
                currentActionGUID = thisData?.nextActionGUID;
            }
            
            else if (stateData.transitionsData.ContainsKey(currentActionGUID))
            {
                stateData?.behaviorData?.controller?.myBehavior?.ChangeState(stateData.behaviorData, stateData.transitionsData[currentActionGUID].nextStateGUID);

                currentActionGUID = "";
            }

            else
            {
                Debug.Log("ERROR: COULDN'T FIND AI NODE TYPE FOR GUID: " + currentActionGUID);
                currentActionGUID = "";
            }

        }
    }


    static void IncrementAllTimers(AI_StateData stateData)
    {
        foreach (var timerData in stateData.timersData)
        {
            AI_Timer.Tick(timerData.Value);
        }
    }


    public static void FindValueDataGUID(AI_StateData stateData, string GUID)
    {
        foreach (var data in stateData.valuesData)
        {
            
        }
    }

}
