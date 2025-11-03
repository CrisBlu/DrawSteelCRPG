

using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class MBActor : MBEntity
{
    private bool activated = false;
    private List<Tile> stepsToTake = new List<Tile>();
    //public SO_UniversalAbilities stuff;
    Dictionary<string, Ability> abilties = new Dictionary<string, Ability>();
    //List<Ability> abilties = new List<Ability>();

   Queue<Vector2Int> openSet = new Queue<Vector2Int>();

    private void OnEnable()
    {
        abilties.Add("FreeStrike", new A_FreeStrike());
    }


    public void ActivateActor()
    {
        Debug.Log(gameObject.name + " is active!");

        


    }

    public void MoveActor(Vector2Int cords)
    {
        openSet.Clear();
        openSet.Enqueue(new Vector2Int(X,Y));
        List<Vector2Int> currentFrontier = new List<Vector2Int>();

        while (openSet.Count > 0)
        {
            Vector2Int currentPos = openSet.Dequeue();

            foreach (Vector2Int neighbor in FindNeighbors(currentPos))
            {
                if (openSet.Contains(neighbor))
                    continue;

                //adjacentTile.cost = currentTile.cost + 1;

                /*if (!IsValidTile(adjacentTile, character.movedata.MaxMove))
                    continue; This is a limitation on speed*/
                if (currentFrontier.Contains(neighbor))
                {
                    continue;
                }

                gridSystem.GridMatrix[neighbor.x, neighbor.y].parent = currentPos;


                /*if (neighbor.x == cords.x && neighbor.y == cords.y)
                {
                    Debug.Log("Found " + neighbor);
                    return;
                }*/
                
                openSet.Enqueue(neighbor);
                currentFrontier.Add(neighbor);
            }
        }



        
        //Different function
        //List<MBTile> stepsToTake = new List<MBTile>();
        //Vector2Int current = cords;
        Tile current = gridSystem.GridMatrix[cords.x, cords.y];
        Tile origin = gridSystem.GridMatrix[X, Y];
        while (current != origin)
        {
            stepsToTake.Add(current);
            if (current.parent != null)
            {
                current = gridSystem.GridMatrix[current.parent.x, current.parent.y];
            }
            else
                break;
            

        }

        //stepsToTake.Add(origin);
        stepsToTake.Reverse();

        StartCoroutine(ActorWalking());

    }


    public void UseMainAction(Tile target)
    {
        abilties["FreeStrike"].Use(target);
    }

    private List<Vector2Int> FindNeighbors(Vector2Int currentPos)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if(i == 0 && j == 0)
                    continue;

                //If outside the grid
                Vector2Int neighbor = new Vector2Int(currentPos.x - i, currentPos.y - j);
                if(neighbor.x < 0 || neighbor.y < 0)
                    continue;

                //If outside the grid, need to change with grid size
                if(neighbor.x >= 10 || neighbor.y >= 10)
                    continue;

                if (gridSystem.GridCheckIfFull(new Vector2Int(neighbor.x, neighbor.y)))
                    continue;


                //Check put diagonals first in the list (we reverse the list later)
                //If there is a cardinal straight line path, prioitize that
                if(Mathf.Abs(i) == Mathf.Abs(j))
                {
                    neighbors.Insert(0, neighbor);
                    continue;
                }

                neighbors.Add(neighbor);
            }

        }
        neighbors.Reverse();
        return neighbors;
    }

   

    private IEnumerator ActorWalking()
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
