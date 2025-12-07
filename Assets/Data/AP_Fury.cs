using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

[CreateAssetMenu(fileName = "AP_Fury", menuName = "Scriptable Objects/Ability Packs/Classes/Fury")]
public class AP_Fury : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_BrutalSlam(), new A_DevastatingRush()};
}

public class A_BrutalSlam : CS_Ability
{
    public override string Name => "Brutal Slam";
    public override string Description => "Drive them back!";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "melee", "signature", "strike" };
    public override int Range => 1;

    int distance;
    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {

        CS_Characteristics stats = data.actor.sheet.stats;
        MB_Actor targetActor = (MB_Actor)data.target.entity;
        Queue<CS_CallbackData> callbackQueue = new Queue<CS_CallbackData>();
        int favoredStat = stats.Might;

        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges, data.banes);



        switch (tier)
        {
            case 1:
                data.target.entity.TakeDamage(3 + favoredStat);
                distance = 1;
                break;

            case 2:
                data.target.entity.TakeDamage(6 + favoredStat);
                distance = 2;
                break;

            case 3 or 4:
                data.target.entity.TakeDamage(9 + favoredStat);
                distance = 4;
                break;

        }

        List<Tile> validPushLocations = CS_GridUtility.GetValidPushArea(data.actor.currentTile, data.target, distance);



        callbackQueue.Enqueue(new CS_CallbackData(KnockbackActor, targetActor, validPushLocations));

        return new CS_AbilityReturnData(true, callbackQueue);
    }

    private void KnockbackActor(MB_Actor target, Tile destination)
    {
        target.ForcedMovement(destination, distance);
    }
}


public class A_DevastatingRush : CS_Ability
{
    public override string Name => "Devastating Rush";
    public override string Description => "Ready or not!";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "melee", "signature", "strike" };
    public override int Range => 4;

    int damage;
    Tile target;
    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        CS_Characteristics stats = data.actor.sheet.stats;
        target = data.target;

        int favoredStat = stats.Might;
        

        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges, data.banes);


        List<Tile> smallCharge = CS_GridUtility.GetMovementArea(data.actor.currentTile, 3, true);

        List<Tile> tilesNextToTarget = data.target.FindNeighbors();
        Tile closestTile = null;
        foreach (Tile tile in tilesNextToTarget)
        {
            if (tile.entity) { continue; }
            if (!smallCharge.Contains(tile)) { continue; }

            if(closestTile == null || (data.actor.currentTile.position - tile.position).magnitude < (data.actor.currentTile.position - closestTile.position).magnitude) { closestTile = tile; }

        }

        List<Tile> chargePath = CS_GridUtility.GridMakePath(closestTile, data.actor.currentTile);
        int extraDamage = chargePath.Count;


        switch (tier)
        {
            case 1:
                damage = 3 + favoredStat + extraDamage;
                break;

            case 2:
                damage = 6 + favoredStat + extraDamage;
                break;

            case 3 or 4:
                damage = 13 + favoredStat + extraDamage;
                break;

        }

        data.actor.movement += extraDamage;
        data.actor.ActorStartWalking(chargePath, LetErRip);

        return new CS_AbilityReturnData(true);

    }

    void LetErRip()
    {
        target.entity.TakeDamage(damage);
    }
}