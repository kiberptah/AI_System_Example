
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class AI_BehaviorData 
{
    public AI_Controller controller;

    public string behaviorName = "";
   
    public string currentStateGUID = "";

    public Dictionary<string, AI_StateData> statesData = new Dictionary<string, AI_StateData>();



    public AI_BehaviorData(AI_SaveData data)
    {
        behaviorName = data.name;
        currentStateGUID = "";
    }

    public AI_BehaviorData()
    {

    }

}

