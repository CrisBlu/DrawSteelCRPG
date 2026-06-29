
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public abstract class CS_Ability
{
    
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract E_ActionType Type { get;  }
    public abstract List<string> Tags { get; }
    public abstract int Range { get; }




    
    //This should be virtual
    public abstract Task<CS_AbilityReturnData> Use(TurnData data);

    public virtual CS_AbilityTargetingData Target(Tile origin) { return null; }
    

}


public class CS_AbilityTargetingData
{
    //Valid Area is the ability's range
    public List<Tile> validArea;

    //Valid targets are the tiles that are valid to click on
    public List<Tile> validTargets;

    public CS_AbilityTargetingData(List<Tile> validArea, List<Tile> validTargets)
    {
        this.validArea = validArea;
        this.validTargets = validTargets;
    }
}

public class CS_AbilityReturnData
{
    public bool isSuccessful;
    public Queue<CS_CallbackData> callbackQueue = null;

    public CS_AbilityReturnData(bool success, Queue<CS_CallbackData> callbackToDo = null)
    {
        isSuccessful = success;
        callbackQueue = callbackToDo;

    }
}

public class CS_CallbackData
{
    public Action<TurnData, Tile> abilityCallback = null;
    public MB_Actor target = null;
    public List<Tile> validTiles = null;


    public CS_CallbackData(Action<TurnData, Tile> callback = null, MB_Actor currentTarget = null, List<Tile> validTiles = null)
    {
        abilityCallback = callback;
        target = currentTarget;
        this.validTiles = validTiles;


    }
}

public class A_MeleeFreeStrike : CS_Ability
{
    public override string Name => "Melee Free Strike";
    public override string Description => "A simple strike";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "charge", "signature", "melee", "strike"};
    public override int Range => 1;


    public override async Task<CS_AbilityReturnData> Use(TurnData data)
    {
        CS_Characteristics stats = data.actor.sheet.stats;
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;
       


        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges);

        Debug.Log("Melee Free Strike " + data.target.entity + "A tier " + tier);
        int damage = 0;
        switch (tier)
        {
            case 1:
                damage = 2;
                break;

            case 2:
                damage = 5;
                break;

            case 3 or 4:
                damage = 7;
                break;

        }

        await data.target.entity.TakeDamage(damage + favoredStat);

        return new CS_AbilityReturnData(true);
    }

   

    public override CS_AbilityTargetingData Target(Tile origin)
    {
        return CS_GridUtility.GetTilesAndAllWithin(origin, Range, true);
    }
}
/*


public class A_RangedFreeStrike: CS_Ability
{
    public override string Name => "Ranged Free Strike";
    public override string Description => "A pot shot";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "ranged", "signature" };
    public override int Range => 5;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {



        CS_Characteristics stats = data.actor.sheet.stats;
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;

        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges, data.banes);



        switch (tier)
        {
            case 1:
                data.target.entity.TakeDamage(2 + favoredStat);
                break;

            case 2:
                data.target.entity.TakeDamage(4 + favoredStat);
                break;

            case 3 or 4:
                data.target.entity.TakeDamage(6 + favoredStat);
                break;

        }


        return new CS_AbilityReturnData(true);
    }

}*/

public class A_Knockback : CS_Ability
{
    public override string Name => "Knockback";
    public override string Description => "Push your target back";
    public override E_ActionType Type => E_ActionType.maneuver;
    public override List<string> Tags => new List<string> { "push" };
    public override int Range => 1;

    private int distance = 0;
    public override async Task<CS_AbilityReturnData> Use(TurnData data)
    {
        Queue<CS_CallbackData> callbackList = new();
        MB_Actor targetActor = (MB_Actor)data.target.entity;
        CS_Characteristics stats = data.actor.sheet.stats;
        distance = 0;

        int tier = CS_DiceRoller.PowerRoll(stats.Might, data.edges, data.banes);

        switch (tier)
        {
            case 1:
                distance = 1;
                break;

            case 2:
                distance = 2;
                break;

            case 3 or 4:
                distance = 3;
                break;
        }

        List<Tile> validPushLocations = CS_GridUtility.GetValidPushArea(data.actor.currentTile, data.target, distance);
        //if original distance is (0,1), then this is along the y axis, only y needs to increase every cell
        //if original distance is (1,0), then this is along the y axis, only x needs to increase every cell


        callbackList.Enqueue(new CS_CallbackData(KnockbackActor, targetActor, validPushLocations));


        return new CS_AbilityReturnData(true, callbackList);
    }

    public override CS_AbilityTargetingData Target(Tile origin)
    {
        //Push should only target actors but w/e for now
        return CS_GridUtility.GetTilesAndAllWithin(origin, Range, true);
    }

    private void KnockbackActor(TurnData data, Tile destination)
    {
        MB_Actor targetActor = (MB_Actor)data.target.entity;
        targetActor.ForcedMovement(destination, distance);
    }
}
/*
public class A_Advance : CS_Ability
{
    public override string Name => "Advance";
    public override string Description => "Move up to your speed";
    public override E_ActionType Type => E_ActionType.move;
    public override List<string> Effects => new();
    public override int Range => 0;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        MB_Actor movingActor = (MB_Actor)data.target.entity;
        movingActor.movement = movingActor.Speed;
        return new CS_AbilityReturnData(true);
    }
}

public class A_CatchBreath : CS_Ability
{
    public override string Name => "Catch Breath";
    public override string Description => "Recover a third of your stamina";
    public override E_ActionType Type => E_ActionType.manuever;
    public override List<string> Effects => new();
    public override int Range => 0;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        MB_Hero hero = (MB_Hero)data.target.entity;
        hero.SpendRecovery();
        return new CS_AbilityReturnData(true);
    }
}*/

public class A_Charge : CS_Ability
{
    public override string Name => "Charge";
    public override string Description => "Move up to your speed in a straight and free strike";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new();
    public override int Range => 0;

    private MB_Actor actor;
    public override Task<CS_AbilityReturnData> Use(TurnData data)
    {
        actor = data.actor;
        Queue<CS_CallbackData> callbackQueue = new Queue<CS_CallbackData> ();

        List<Tile> validCharge = CS_GridUtility.GetWalkableTilesFromOrigin(data.target, data.actor.Speed, true);
        callbackQueue.Enqueue(new CS_CallbackData(Charge, data.actor, validCharge));

        return Task.FromResult(new CS_AbilityReturnData(true, callbackQueue));
    }

    public override CS_AbilityTargetingData Target(Tile origin)
    {
        MB_Actor self = (MB_Actor)origin.entity;
        //Display charge range, but click on actor to use
        return new CS_AbilityTargetingData(CS_GridUtility.GetWalkableTilesFromOrigin(origin, self.Speed, true), new List<Tile>() { origin });
    }

    //TODO: This callback isn't required, just fold charge into the main Use function
    async void Charge(TurnData data, Tile destination)
    {
        Debug.Log("Charging");
        int path = CS_GridUtility.FindShortestPath(destination, data.actor.currentTile).Count;
        TurnData newTurn = data.TurnManager.CreateAndStoreTurn(data.actor, 1, 0, path, "charge", E_TurnState.HoldingForAnimation);
        
        await Movement.ActorMovement(newTurn, destination);
        newTurn.DefaultToState();
    }


}


/*
public class A_StrikeNow : CS_Ability
{
    public override string Name => "Strike Now!";
    public override string Description => "a opening!";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "ranged" };
    public override int Range => 10;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        data.addActorToTurn(new CS_ActorTurnStats((MB_Actor)data.target.entity, 1, 0, 0, "signature"));
        return new CS_AbilityReturnData(true);
    }

}

public class A_SpearCharge : CS_Ability
{
    public override string Name => "Spear Charge";
    public override string Description => "The goblin rushes forward";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "charge", "melee", "strike" };
    public override int Range => 1;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        CS_Characteristics stats = data.actor.sheet.stats;
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;



        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges, data.banes);

        switch (tier)
        {
            case 1:
                data.target.entity.TakeDamage(1 + favoredStat);
                break;

            case 2:
                data.target.entity.TakeDamage(2 + favoredStat);
                break;

            case 3 or 4:
                data.target.entity.TakeDamage(3 + favoredStat);
                break;

        }

        return new CS_AbilityReturnData(true);
    }
}*/




/*
 *  public override string Name => "";
    public override string Description => "";
    public override E_ActionType Type => E_ActionType.;
    public override List<string> Effects => new(); new List<string> { "" };
    public override int Range => ;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        
    }

 * 
 */