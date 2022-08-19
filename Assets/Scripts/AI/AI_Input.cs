using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Input : MonoBehaviour
{
    AI_Controller controller;
    
    ActorConnector connector;


    void Awake()
    {
        controller = GetComponent<AI_Controller>();

        connector = controller.actor.GetComponent<ActorConnector>();
    }

  
    public void Input_Movement(Vector3 movementDirection)
    {
        movementDirection = movementDirection.normalized;

        connector.Input_Movement(movementDirection);

    }





}
