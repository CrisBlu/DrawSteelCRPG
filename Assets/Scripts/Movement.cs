using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public static class Movement
{
    

    public static bool UpdateEntityPosition(MB_Entity actor, Tile newTile)
    {
        //Add to grid will update this in grid data, I think I do not like this
        if (newTile.parentGrid.AddToGrid(newTile, actor))
        {
            //If GridData tells us that our desired tile is empty

            //remove actor from tile in grid data
            actor.currentTile.entity = null;

            //assign new current tile in actor 
            actor.currentTile = newTile;

            //Move actor in physical space, not to be done outside of a coroutine
            //actor.transform.position = new Vector3(newTile.position.x, 0, newTile.position.y);

            return true;
        }

        return false;
    }

    public static async Task ActorMovement(TurnData turn, Tile destination)
    {
        MB_Actor actor = turn.actor;
        Dictionary<E_ActionType, int> actions = turn.actions;



        List<Tile> stepsToTake = CS_GridUtility.GetStepsToTake(destination, actor.currentTile);
        List<Vector2Int> stepsTaken = new List<Vector2Int>() { actor.currentTile.position };

        for (int i = 0; i < stepsToTake.Count; i++)
        { 
            if (actions[E_ActionType.move] <= 0)
            {
                break;
            }

            actions[E_ActionType.move]--;

            //CS_BattleLog.BattleEvents.TriggerActorLeftTileEvents(actor.currentTile, stepsToTake[i], actor);
            UpdateEntityPosition(actor, stepsToTake[i]);
            
            stepsTaken.Add(stepsToTake[i].position);

        }

        //In order to reverse this, I need the destination the actor came from, the movement the actor spent to get there, and what state the action was
        // set the turn state
        // refund the actor their movement
        // then teleport them back to their previous spot


        //I suspect that AnimationWalking will be an issue, solution? Cancel Animation Walking if not in Waiting For Animation state

        await AnimationWalking(actor, stepsTaken);

        return;
    }

    //A variant that can be used to move off turn
    public static async Task ActorMovement(MB_Actor actor, List<Tile> stepsToTake)
    {


        //List<Tile> stepsToTake = CS_GridUtility.GetStepsToTake(destination, actor.currentTile);
        List<Vector2Int> stepsTaken = new List<Vector2Int>() { actor.currentTile.position };

        for (int i = 0; i < stepsToTake.Count; i++)
        {

            CS_BattleLog.BattleEvents.TriggerActorLeftTileEvents(actor.currentTile, stepsToTake[i], actor);
            UpdateEntityPosition(actor, stepsToTake[i]);

            stepsTaken.Add(stepsToTake[i].position);

        }

        await AnimationWalking(actor, stepsTaken);

        return;
    }

    //I do not think it is wise to only trigger animation walking at the ned
    private static async Task AnimationWalking(MB_Actor actor, List<Vector2Int> stepsTaken)
    {
        await Task.Delay(1000);

        for (int i = 1; i < stepsTaken.Count; i++)
        {
            Vector2 position = Vector2.Lerp(stepsTaken[i - 1], stepsTaken[i], 1);
            actor.transform.position = new Vector3(position.x, 0, position.y);
            await Task.Delay(200);
        }

        return;

    }


}

//What are the things that can change within the scope of an action
// Stamina
// Recoveries
// Move, Main, Manuever
// Position (If position is changed directly, UpdateEntityPosition must be called after it and transform.position must be also updated)
// Status (Dead, Prone, Bleeding, Poisoned)

public struct CS_Variables
{
    public MB_Actor actor;
    public int stamina;
    public E_TurnState turnState;
    //public int recoveries;
    public Dictionary<E_ActionType, int> actions;
    public Vector2Int position;

    public CS_Variables(TurnData turn)
    {

        actor = turn.actor;
        stamina = turn.actor.stamina;
        turnState = turn.turnState;
        //this.recoveries = recoveries;
        actions = turn.actions;
        position = turn.actor.position;
    }
}
