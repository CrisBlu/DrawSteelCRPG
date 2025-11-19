using Mono.Cecil.Cil;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;

public class MB_Actor : MB_Entity
{
    public List<CS_Ability> abilities = new List<CS_Ability>();

    private SO_ActorEventManager ActorEventManager;

    [SerializeField] MB_Model ActorModel;

    [HideInInspector] public int Speed = 5;
    [HideInInspector] public int movement = 0;

    public bool turnTaken = false;
    protected bool isWalking = false;

    protected virtual void Awake()
    {
        base.Awake();
        ActorEventManager = new SO_ActorEventManager();
        ActorModel.ActorEventManager = ActorEventManager;
        abilities.Add(new A_MeleeFreeStrike());
        abilities.Add(new A_RangedFreeStrike());
        abilities.Add(new A_Knockback());
        abilities.Add(new A_Advance());
    }


    public void ActorStartWalking(List<Tile> stepsToTake, Action<E_SelectState> callbackForState)
    {
        StartCoroutine(ActorWalking(stepsToTake, callbackForState));

        ActorEventManager.EventActorWalk.Invoke();
    }

    private IEnumerator ActorWalking(List<Tile> stepsToTake, Action<E_SelectState> callbackForState)
    {
        isWalking = true;

        yield return new WaitForSeconds(1f);
        while (stepsToTake.Count > 0)
        {
            if(movement <= 0)
            {
                break;
            }

            movement--;

            Debug.Log("Moving to " + stepsToTake[0].position);

            Vector2Int lastPos = new Vector2Int(X, Y);
            X = stepsToTake[0].position.x; Y = stepsToTake[0].position.y;

            UpdatePosition(lastPos);

            stepsToTake.RemoveAt(0);
            yield return new WaitForSeconds(.2f);
        }

        //When done moving
        gridSystem.GridDisplayPossibleSteps(movement);
        isWalking = false;
        callbackForState(E_SelectState.LookingForMove);


        yield return null;
    }

    public void SetTurnTaken(bool state)
    {
        turnTaken = state;
    }


    public override void ForcedMovement(Tile cellPushedInto, int distance)
    {
        ActorEventManager.EventActorPushed.Invoke();


        //Shoves the actor into the next square to their destination, up to the distance
        //If something exists in that space, take damage and don't move 
        Vector2Int origin = new Vector2Int(X, Y);
        Vector2Int nextCell = new Vector2Int(X, Y);
        while(X != cellPushedInto.position.x || Y != cellPushedInto.position.y)
        {
            if(distance == 0)
            {
                break;
            }

            if (X != cellPushedInto.position.x)
            {
                nextCell.x = cellPushedInto.position.x > X ? nextCell.x + 1 : nextCell.x - 1;
            }

            if (Y != cellPushedInto.position.y)
            {
                nextCell.y = cellPushedInto.position.y > Y ? nextCell.y + 1 : nextCell.y - 1;
            }
            distance -= 1;

            if (gridSystem.GridCheckIfFull(gridSystem.GridMatrix[nextCell.x, nextCell.y]))
            {
                TakeDamage(1);

                gridSystem.GridMatrix[nextCell.x, nextCell.y].entity.TakeDamage(1);

                nextCell.x = X;
                nextCell.y = Y;
            }
            else
            {
                X = nextCell.x;
                Y = nextCell.y;
            }
        }

        UpdatePosition(origin);

        //stepsToTake.Add(cords);
    }

    public override void TakeDamage(int damage)
    {
        ActorEventManager.EventActorHurt.Invoke();
        base.TakeDamage(damage);
    }

    public void UseAbility()
    {
        ActorEventManager.EventActorAttack.Invoke();
    }



    SO_BattleManager tesst;
    public void TestTurnEnd(SO_BattleManager test)
    {
        tesst = test;
        Invoke("TesterTurnEnd", 2f);
    }
    private void TesterTurnEnd()
    {
        tesst.OnTurnBegin();
    }




}
