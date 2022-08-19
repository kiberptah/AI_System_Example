using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorConnector : MonoBehaviour
{
    ActorMovement actorMovement;
    void Awake()
    {
        actorMovement = GetComponent<ActorMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Input_Movement(Vector2 direction)
    {
        actorMovement.Input_Movement(direction);
    }
}
