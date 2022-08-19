using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "AI/Actions/LookForActors")]
public class LookForActors_AI_Action : AI_Action
{
    protected override void AddDynamicValues()
    {

    }

    protected override void DoTheActions(AI_Controller controller, AI_StateData stateData, AI_ActionData actionData)
    {
        controller.sightedObjects = LookForTheObjectsAround(controller);
    }


    List<Transform> LookForTheObjectsAround(AI_Controller controller)
    {
        List<Transform> _sightedObjects = new List<Transform>();

        Collider[] sightedColliders;
        sightedColliders = Physics.OverlapSphere(controller.actor.transform.position, controller.actorStats.sightRadius);


        foreach (Collider obj in sightedColliders)
        {
            // Limit it to actors or objects or whatever you need for your game
            if (EyeContactWithTarget(controller, obj.transform))
            {
                _sightedObjects.Add(obj.transform);
            }
        }

        return _sightedObjects;
    }


    public bool EyeContactWithTarget(AI_Controller controller, Transform target)
    {
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(target.position, controller.actor.position);

            RaycastHit[] raycast =
                Physics.RaycastAll(
                    origin: controller.actor.transform.position,
                    direction: controller.actor.transform.TransformDirection(target.position - controller.actor.transform.position),
                    maxDistance: controller.actorStats.sightRadius,
                    layerMask: LayerMask.GetMask("Geometry")
                    );

            foreach (var potentialObstacle in raycast)
            {
                //Debug.Log("i see: " + potentialObstacle.transform.name + " in front of the ");

                if (potentialObstacle.transform.gameObject.tag == "Geometry")
                {
                    if (potentialObstacle.distance < distanceToTarget)
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        else
        {
            return false;
        }
    }



}
