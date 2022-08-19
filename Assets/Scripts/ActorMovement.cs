using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorMovement : MonoBehaviour
{
    public float speed = 1;
    Vector2 direction;
    public void Input_Movement(Vector2 direction)
    {
        this.direction = direction;

    }


    private void Update()
    {
        if (direction != Vector2.zero)
        {
            transform.Translate(direction * speed * Time.deltaTime);
            direction = Vector2.zero; // to avoid bugs if input stopped without telling it is zero 
        }        
    }
}
