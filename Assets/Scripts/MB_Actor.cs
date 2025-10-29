
using Mono.Cecil.Cil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MBActor : MBEntity
{
    private bool activated = false;
    private List<Vector2Int> stepsToTake = new List<Vector2Int>();


    public void ActivateActor()
    {
        Debug.Log(gameObject.name + " is active!");
    }

    public void MoveActor(Vector2Int cords)
    {
        //Need algorithm to determine where I can walk
        

        Vector2Int nextStep = cords;

        //If input position is equal to actor position
        if (X == cords.x && Y == cords.y)
        {
            StartCoroutine(ActorWalking());
            return;
        }

        if (X != cords.x)
        {
            nextStep.x = X > cords.x ? cords.x + 1 : cords.x - 1;
        }

        if (Y != cords.y)
        {
            nextStep.y = Y > cords.y ? cords.y + 1 : cords.y - 1;
        }



    
        MoveActor(nextStep);

        stepsToTake.Add(cords);

        


    }

    private IEnumerator ActorWalking()
    {
        yield return new WaitForSeconds(1f);
        while (stepsToTake.Count > 0)
        {
            Debug.Log("Moving to " + stepsToTake[0]);

            Vector2Int lastPos = new Vector2Int(X, Y);
            X = stepsToTake[0].x; Y = stepsToTake[0].y;

            UpdatePosition(lastPos);
            Vector3 stepPos = new Vector3(X, 0, Y);
            transform.position = stepPos;

            stepsToTake.RemoveAt(0);
            yield return new WaitForSeconds(.2f);
        }
        yield return null;
    }






    /*On click to move do following
     * 1. Take input position and compare to actor position
     * 2. If input position is equal to actor position return
     * 3a. If target position x is greater than actor position x, add one to x
     * 3b. If target position x is less than actor position x, remove one from x
     * 4. do the same with y to get the next Position
     * 5. check if next Position is full if not do 1. with next Position as input position
     * 6. move actor to the input position
     * */
}
