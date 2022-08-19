using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AI_Controller : MonoBehaviour
{
    public bool aiActive = true;

    [HideInInspector] public AI_Input input;

    public Transform actor;
    [HideInInspector] public ActorStats actorStats;
    //[HideInInspector] public ActorHealth actorHealth;


    #region AI
    public AI_SaveData saveData = null;
    public AI_Behavior myBehavior = null;
    public AI_BehaviorData behaviorData;// = new AI_BehaviorData();
    #endregion

    #region Looking
    [HideInInspector] public List<Transform> sightedObjects = new List<Transform>();
    [HideInInspector] public Transform currentTarget;
    #endregion

    #region pathfinding
    [HideInInspector] public List<Vector3> path = new List<Vector3>();
    [HideInInspector] public Vector3 destination = Vector3.zero;
    #endregion



    #region MonoBehavior
    void Awake()
    {      
        if (saveData != null)
        {
            myBehavior = new AI_Behavior();
            myBehavior.Initialize(this, saveData);
        }
        

        input = GetComponent<AI_Input>();


        actorStats = actor.GetComponent<ActorStats>();
        //actorHealth = actor.GetComponent<ActorHealth>();
    }
    void Update()
    {
        if (!aiActive)
        { return; }

        myBehavior?.BehaviorLoop(behaviorData);   
    }
    #endregion








    [HideInInspector] public List<Vector3> debug_path = new List<Vector3>();
    [HideInInspector] public Vector3 debug_destination;

    /*
    void OnDrawGizmos()
    {
        
        if (Application.isPlaying)
        {

            foreach (var node in debug_path)
            {
                Gizmos.color = Color.white;
                if (node == debug_destination)
                {
                    Gizmos.color = Color.green;
                    Gizmos.DrawLine(transform.position, node);
                }

                Gizmos.DrawSphere(node, .25f);
            }

            // show state
            float sphereRadius = 1f;
            if (currentState != null)
            {
                currentState.debug_Color.a = 0.33f;
                Gizmos.color = currentState.debug_Color;

                Gizmos.DrawSphere(actor.position, sphereRadius);
            }
        }

    }
    */
}
