using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Behavior
{
    //AI_BehaviorData initialData;

    public void Initialize(AI_Controller controller, AI_SaveData saveData)
    {
        controller.behaviorData = new AI_BehaviorData(saveData);
        controller.behaviorData.controller = controller;

        foreach (var stateData in saveData.statesData)
        {
            AI_StateData newStateData = new AI_StateData(stateData);
            newStateData.behaviorData = controller.behaviorData;


            foreach (var data in saveData.actionsData)
            {
                var newData = new AI_ActionData(data, newStateData);

                newStateData.actionsData.Add(newData.GUID, newData);
            }
            foreach (var data in saveData.decisionsData)
            {
                var newData = new AI_DecisionData(data, newStateData);

                newStateData.decisionsData.Add(newData.GUID, newData);
            }
            foreach (var data in saveData.timersData)
            {
                var newData = new AI_TimerData(data, newStateData);

                newStateData.timersData.Add(newData.GUID, newData);
            }
            foreach (var data in saveData.valuesData)
            {
                var newData = new AI_ValueData(data, newStateData);

                newStateData.valuesData.Add(newData.GUID, newData);
            }
            foreach (var data in saveData.valueChangersData)
            {
                var newData = new AI_ValueChangerData(data, newStateData);

                newStateData.valueChangersData.Add(newData.GUID, newData);
            }
            foreach (var data in saveData.transitionsData)
            {
                var newData = new AI_TransitionData(data, newStateData);

                newStateData.transitionsData.Add(newData.GUID, newData);
            }




            controller.behaviorData.statesData.Add(newStateData.GUID, newStateData);                   
        }



        controller.behaviorData.currentStateGUID = "";

        ChangeState(controller.behaviorData, saveData.firstStateGUID);
    }


    public void ChangeState(AI_BehaviorData behaviorData, string newStateGUID)
    {
        if (newStateGUID != behaviorData.currentStateGUID && behaviorData.statesData.ContainsKey(newStateGUID))
        {
            if (behaviorData.statesData.ContainsKey(behaviorData.currentStateGUID))
            {
                AI_State.ExitState(behaviorData.statesData[behaviorData.currentStateGUID]);
            }
            
            behaviorData.currentStateGUID = newStateGUID;

            AI_State.EnterState(behaviorData.statesData[behaviorData.currentStateGUID]);
        }
    }

    public void BehaviorLoop(AI_BehaviorData behaviorData)
    {
        AI_State.StateLoop(behaviorData.statesData[behaviorData.currentStateGUID]);
    }



    


}
