
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public interface IRequest
{
    public Task InvokeBeforeTriggers();
    public Task Resolve();

    public bool Cancel { get; set; }

}


public class RequestDamage : IRequest
{
    public Tile target;
    public int damage;

    public bool Cancel
    {
        get { return cancelled; }
        set { cancelled = value; }
    }
    private bool cancelled = false;


    public RequestDamage(Tile target, int damage)
    {
        this.target = target;
        this.damage = damage;
    }

    public async Task InvokeBeforeTriggers()
    {
        await SO_BattleEvents.TriggerBeforeTakeDamageEvents(this);
    }

    public async Task Resolve()
    {
        MB_Actor targetActor = target.entity as MB_Actor;

        targetActor.stamina -= damage;
        targetActor.ActorAnimator.SetTrigger("Damaged");

        if (targetActor.stamina <= 0)
        {
            Debug.Log(targetActor.gameObject.name + " is dead");
        }

        await SO_BattleEvents.TriggerActorTookDamageEvents(damage, targetActor);
    }
}

public class RequestForceMove : IRequest
{
    public Tile target;
    public int distance;
    public Tile origin;
    public bool Cancel
    {
        get { return cancelled; }
        set { cancelled = value; }
    }
    private bool cancelled = false;

    public RequestForceMove(Tile target, int distance, Tile origin)
    {
        this.target = target;
        this.distance = distance;
        this.origin = origin;
    }

    public async Task InvokeBeforeTriggers()
    {
        //Suspect that this needs some buffer to prevent game moving on without letting triggers form
        await SO_BattleEvents.TriggerBeforeForcedMovedEvents(this);
    }

    public async Task Resolve()
    {

        Tile tileToPushTarget = await SelectTileToMoveTo();

        
        await ForcedMovement(tileToPushTarget);
    }



    //I already know this is gonna need to be tweak to allow for off turn stuff
    public async Task<Tile> SelectTileToMoveTo()
    {
        
        List<Tile> validPushLocations = CS_GridUtility.GetValidPushArea(origin, target, distance);
        //if original distance is (0,1), then this is along the y axis, only y needs to increase every cell
        //if original distance is (1,0), then this is along the y axis, only x needs to increase every cell


        CS_ColorGrid.ColorCells(validPushLocations, Color.blue);
        AwaitTile tileRequest = new AwaitTile(validPushLocations);
        MB_PlayerInput.inputRequest = tileRequest;
        Tile tileToPushTarget = await tileRequest.WaitForUserConfirmation();



        return tileToPushTarget;
    }

    public async Task ForcedMovement(Tile pushedInto)
    {
        //Shoves the actor into the next square to their destination, up to the distance
        //If something exists in that space, take damage and don't move 
        MB_Actor targetActor = (MB_Actor)target.entity;
        Vector2Int nextCell = targetActor.currentTile.position;
        while (targetActor.currentTile != pushedInto)
        {
            if (distance == 0)
            {
                break;
            }

            if (targetActor.currentTile.position.x != pushedInto.position.x)
            {
                nextCell.x = pushedInto.position.x > targetActor.currentTile.position.x ? nextCell.x + 1 : nextCell.x - 1;
            }

            if (targetActor.currentTile.position.y != pushedInto.position.y)
            {
                nextCell.y = pushedInto.position.y > targetActor.currentTile.position.y ? nextCell.y + 1 : nextCell.y - 1;
            }
            distance -= 1;

            if (!targetActor.UpdatePosition(targetActor.gridData.GetTile(nextCell)))
            {
                //Need differentiate between taking damage from an ability and taking damage like this
                //Wouldn't hurt to apply this all at once either
                //await targetActor.TakeDamage(1);

                //await targetActor.gridData.GetTile(nextCell).entity.TakeDamage(1);

                nextCell = targetActor.currentTile.position;
            }
        }

        await SO_BattleEvents.TriggerActorForcedMovedEvents(targetActor.currentTile);
        //UpdatePosition(origin);
    }

}

