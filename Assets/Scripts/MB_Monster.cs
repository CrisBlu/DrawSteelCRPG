using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MB_Monster : MB_Actor
{
    [SerializeField] private SO_TurnManager Team;
    
    protected override void Awake()
    {
        base.Awake();
        abilities.Add(new A_SpearCharge());
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


        Team.EndTurn();
    }


}
