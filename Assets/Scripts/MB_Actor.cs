using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;

public class MB_Actor : MB_Entity
{
    public List<CS_Ability> abilties = new List<CS_Ability>();
    [HideInInspector] public int speed = 5;

    private void OnEnable()
    {
        abilties.Add(new A_MeleeFreeStrike());
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

    public void ActorFreeStrike(Tile target)
    {
        //abilties["MeleeFreeStrike"].Use(target);
    }
}
