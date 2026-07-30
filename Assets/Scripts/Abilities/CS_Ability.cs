
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;


public abstract class CS_Ability
{

    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract E_ActionType Type { get; }
    public abstract List<string> Tags { get; }
    public abstract int Range { get; }
    public int Cost = 0;
    public List<Tile> targets = new List<Tile>();

    public MB_Actor Owner;

    public virtual List<E_AbilityInstructions> Instructions { get { return new List<E_AbilityInstructions>() { E_AbilityInstructions.SelectTarget, E_AbilityInstructions.Confirm }; } }
    
   




    
    //This should be virtual
    public abstract Task<CS_AbilityReturnData> Use(TurnData data);

    public virtual CS_AbilityTargetingData Target(Tile origin)
    {

        return CS_GridUtility.GetTilesAndAllWithin(origin, Range, true);


    }

    public virtual void Spend(bool spent) { return; }



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

    public CS_AbilityReturnData(bool success)
    {
        isSuccessful = success;

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

        Debug.Log("Melee Free Strike " + targets[0].entity + "A tier " + tier);
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

        await targets[0].entity.TakeDamage(damage + favoredStat);

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

    public override async Task<CS_AbilityReturnData> Use(TurnData data)
    {
        Queue<CS_CallbackData> callbackList = new();
        //MB_Actor targetActor = (MB_Actor)targets[0].entity;
        CS_Characteristics stats = Owner.sheet.stats;
        int distance = 0;

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
        
        SO_BattleEvents.RequestQueue.Enqueue(new RequestForceMove(targets[0], distance, Owner.currentTile));
        SO_BattleEvents.TestingGoThroughQueue();


        return new CS_AbilityReturnData(true);
    }

    public override CS_AbilityTargetingData Target(Tile origin)
    {
        //Push should only target actors but w/e for now
        return CS_GridUtility.GetTilesAndAllWithin(origin, Range, true);
    }

}

public class A_Charge : CS_Ability
{
    public override string Name => "Charge";
    public override string Description => "Move up to your speed in a straight and free strike";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new();
    public override int Range => 0;


    public override async Task<CS_AbilityReturnData> Use(TurnData data)
    {

        List<Tile> validChargePath = CS_GridUtility.GetStepsToTake(targets[0], Owner.currentTile);

   

        await Movement.ActorMovement(Owner, validChargePath);



        TurnData newTurn = data.TurnManager.CreateAndStoreTurn(Owner, 1, 0, 0, "charge");




        return new CS_AbilityReturnData(true);
    }

    public override CS_AbilityTargetingData Target(Tile origin)
    {
        MB_Actor acting = (MB_Actor)origin.entity;
        List<Tile> abilityRange = CS_GridUtility.GetWalkableTilesFromOrigin(origin, acting.Speed, true);
        return new CS_AbilityTargetingData(abilityRange, abilityRange);

    }



}




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