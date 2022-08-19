using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AI_Timer
{

    public static void OnStateEnter(AI_TimerData data)
    {
        data.currentTime = 0;
        data.currentRandomOffset = Random.Range(-data.intervalRandomOffset, data.intervalRandomOffset);       
    }

    public static void Tick(AI_TimerData data)
    {

        data.currentTime += Time.deltaTime;
        
        if (data.currentTime > data.timeInterval + data.currentRandomOffset)
        {
            data.currentTime = 0;
            data.currentRandomOffset = Random.Range(-data.intervalRandomOffset, data.intervalRandomOffset);
                      
            data.nextGUID = data.trueGUID;
        }
        else
        {
            data.nextGUID = data.falseGUID;
        }
    }
}
