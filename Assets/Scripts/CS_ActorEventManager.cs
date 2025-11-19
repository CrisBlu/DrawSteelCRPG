using UnityEngine;
using UnityEngine.Events;

//[CreateAssetMenu(fileName = "SO_ActorEventManager", menuName = "Scriptable Objects/SO_ActorEventManager")]
public class CS_ActorEventManager
{
    [HideInInspector] public UnityEvent EventActorIdle;
    [HideInInspector] public UnityEvent EventActorWalk;
    [HideInInspector] public UnityEvent EventActorAttack;
    [HideInInspector] public UnityEvent EventActorHurt;
    [HideInInspector] public UnityEvent EventActorPushed;

    public CS_ActorEventManager()
    {


        EventActorIdle = new UnityEvent();
        EventActorWalk = new UnityEvent();
        EventActorAttack = new UnityEvent();
        EventActorHurt = new UnityEvent();
        EventActorPushed = new UnityEvent();
        
    }





    public void OnDisable()
    {
        EventActorIdle?.RemoveAllListeners();
        EventActorWalk?.RemoveAllListeners();
        EventActorAttack?.RemoveAllListeners();
        EventActorHurt?.RemoveAllListeners();
        EventActorPushed?.RemoveAllListeners();
    }
}
