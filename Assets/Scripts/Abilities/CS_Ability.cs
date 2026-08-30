
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;


public abstract class CS_Ability
{

    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract E_ActionType Type { get; }
    public abstract List<string> Tags { get; }
    public abstract int Range { get; }

    //first entry is associated with intial resource cost to use the ability, second entry is any resource cost associated within the ability
    public virtual int[] Cost { get { return new int[2] { 0, 0 }; } }
    public List<Tile> targets = new List<Tile>();

    public MB_Actor Owner;

    public virtual List<E_AbilityInstructions> Instructions { get { return new List<E_AbilityInstructions>() { E_AbilityInstructions.SelectTarget, E_AbilityInstructions.Confirm }; } }
    
   




    
    
    public abstract Task<bool> Use(int tier = 0);

    public virtual CS_AbilityTargetingData Target(Tile origin)
    {

        return CS_GridUtility.GetTilesAndAllWithin(origin, Range, true);


    }

    public virtual void Spend(bool spent) { return; }



}



public interface ITieredAbility
{
    public List<E_Stats> BonusStat { get;}

    public void RollAbility(int edges, int banes)
    {
        CS_Ability abilityData = this as CS_Ability;
        CS_Characteristics stats = abilityData.Owner.sheet.stats;

        int bonus = -5;
        foreach(E_Stats stat in BonusStat)
        {
            if(stats.Get(stat) > bonus)
            {
                bonus = stats.Get(stat);
            }
        }
        

        SO_BattleEvents.AddRequest(new RequestPowerRoll(abilityData, bonus, edges, banes));
    }
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

public class A_MeleeFreeStrike : CS_Ability, ITieredAbility
{
    public override string Name => "Melee Free Strike";
    public override string Description => "A simple strike";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "charge", "signature", "melee", "strike"};
    public override int Range => 1;
    public List<E_Stats> BonusStat => new List<E_Stats>() { E_Stats.M, E_Stats.A};

    public override async Task<bool> Use(int tier = 0)
    {
        CS_Characteristics stats = Owner.sheet.stats;
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;
   

     
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

        SO_BattleEvents.AddRequest(new RequestDamage(targets[0], damage + favoredStat));

        return true;
    }

    public override CS_AbilityTargetingData Target(Tile origin)
    {
        return CS_GridUtility.GetTilesAndAllWithin(origin, Range, true);
    }

    
 

}


public class A_Knockback : CS_Ability, ITieredAbility
{
    public override string Name => "Knockback";
    public override string Description => "Push your target back";
    public override E_ActionType Type => E_ActionType.maneuver;
    public override List<string> Tags => new List<string> { "push" };
    public override int Range => 1;
    public List<E_Stats> BonusStat => new List<E_Stats>() { E_Stats.M};

    public override async Task<bool> Use(int tier = 0)
    {

        int distance = 0;

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
        
        SO_BattleEvents.AddRequest(new RequestForceMove(targets[0], distance, Owner.currentTile));


        return true;
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


    public override async Task<bool> Use(int tier = 0)
    {

        List<Tile> validChargePath = CS_GridUtility.GetStepsToTake(targets[0], Owner.currentTile);

   

        await Movement.ActorMovement(Owner, validChargePath);



        TurnData newTurn = SO_TurnManager.Instance.CreateAndStoreTurn(Owner, 1, 0, 0, "charge");




        return true;
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