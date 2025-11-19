using UnityEngine;

public class MB_Model : MonoBehaviour
{
    [HideInInspector] public SO_ActorEventManager ActorEventManager;

    private void Start()
    {
        ActorEventManager.EventActorAttack.AddListener(OnAttack);
        ActorEventManager.EventActorWalk.AddListener(OnWalk);
        ActorEventManager.EventActorHurt.AddListener(OnHurt);
        ActorEventManager.EventActorPushed.AddListener(OnPush);
        ActorEventManager.EventActorIdle.AddListener(OnIdle);
    }

    //A script with event listeners and associated functions for use in front end visual development


    private void OnAttack()
    {
        Debug.Log("Attack");
    }

    private void OnWalk()
    {
        Debug.Log("Walking");
    }

    private void OnHurt()
    {
        Debug.Log("Ouch");
    }

    private void OnPush()
    {
        Debug.Log("BeingPushed");
    }
    
    private void OnIdle()
    {
        Debug.Log("DoinNothing");
    }


}
