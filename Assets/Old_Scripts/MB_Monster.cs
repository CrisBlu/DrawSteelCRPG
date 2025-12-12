using System;
using System.Collections;
using UnityEngine;

public class MB_Monster : MB_Old_Actor
{
    [SerializeField] private SO_Old_TurnManager Team;
    public bool advancing = true;
    public bool watching = false;
    public bool gaurding = false;

    
    protected override void Awake()
    {
        
        base.Awake();

        //desiredAbilities.Add(new A_Charge());

        Team.actorsUnderControl.Add(this);
    }


    protected override void RemoveFromWorld()
    {
        Team.actorsUnderControl.Remove(this);
        base.RemoveFromWorld();
    }



    public void StartStagger(Action<CS_Ability, MB_Monster> callback)
    {
        StartCoroutine(Stagger(callback));
    }

    IEnumerator Stagger(Action<CS_Ability, MB_Monster> callback)
    {
        yield return new WaitUntil(() => !isWalking);
        Debug.Log("Done walking");

        callback(abilities[3], this);


    }


}
