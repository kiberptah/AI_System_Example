using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    ActorMovement actorMovement;
    private void Awake()
    {
        actorMovement = GetComponent<ActorMovement>();

    }
    private void Update()
    {
        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;
        actorMovement.Input_Movement(input);
    }
}
