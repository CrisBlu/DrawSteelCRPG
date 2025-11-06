using Mono.Cecil.Cil;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Playables;
using UnityEngine;

public class MB_Actor : MB_Entity
{
    public List<CS_Ability> abilities = new List<CS_Ability>();
    [HideInInspector] public int speed = 5;

    private void OnEnable()
    {
        abilities.Add(new A_MeleeFreeStrike());
        abilities.Add(new A_Knockback());
    }


    public void ActorStartWalking(List<Tile> stepsToTake)
    {
        StartCoroutine(ActorWalking(stepsToTake));
    }

    private IEnumerator ActorWalking(List<Tile> stepsToTake)
    {
        yield return new WaitForSeconds(1f);
        while (stepsToTake.Count > 0)
        {
            Debug.Log("Moving to " + stepsToTake[0].position);

            Vector2Int lastPos = new Vector2Int(X, Y);
            X = stepsToTake[0].position.x; Y = stepsToTake[0].position.y;

            UpdatePosition(lastPos);
            Vector3 stepPos = new Vector3(X, 0, Y);
            transform.position = stepPos;

            stepsToTake.RemoveAt(0);
            yield return new WaitForSeconds(.2f);
        }
        yield return null;
    }


    //This doesn't move anyone anywhere
    public override void ForcedMovement(Tile cellPushedInto, int distance)
    {
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
