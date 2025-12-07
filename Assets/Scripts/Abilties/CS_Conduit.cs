using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;


[CreateAssetMenu(fileName = "AP_Conduit", menuName = "Scriptable Objects/Ability Packs/Classes/Conduit")]
public class AP_Conduit : SO_AbilityPack
{

    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_Lightfall(), new A_HealingGrace(), new A_RayOfWrath() };

}


public class A_Lightfall : CS_Ability
{
    public override string Name => "Lightfall";
    public override string Description => "A rain of holy light scours your enemies and repositions your allies.";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "signature" };
    public override int Range => 0;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        Queue<CS_CallbackData> callbackQueue = new();
        List<Tile> targetedTiles;
        List<MB_Actor> targetedActors;
        CS_Characteristics stats = data.actor.sheet.stats;
        int damage = 0;
        int distance = 2;


        int tier = CS_DiceRoller.PowerRoll(stats.Intuition, data.edges, data.banes);

        switch (tier)
        {
            case 1:
                damage = 2;
                break;

            case 2:
                damage = 3;
                break;

            case 3 or 4:
                damage = 5;
                break;
        }

        CS_AoeReturnData TilesAndActors = CS_GridUtility.GetTilesAndActorsWithin(data.actor.currentTile, distance);
        targetedActors = TilesAndActors.affectedActors;
        targetedTiles = TilesAndActors.affectedArea;


        foreach(MB_Actor target in targetedActors)
        {
            if(data.actor.CompareTag(target.tag))
            {
                callbackQueue.Enqueue(new CS_CallbackData(TeleportActor, target, targetedTiles));
            }
            else
            {
                target.TakeDamage(damage);
            }
        }

        return new CS_AbilityReturnData(true, callbackQueue);


    }

    private void TeleportActor(MB_Actor target, Tile destination)
    {
        target.Teleport(destination.position);
    }
}

public class A_HealingGrace : CS_Ability
{
    public override string Name => "Healing Grace";
    public override string Description => "A Divine Heal";
    public override E_ActionType Type => E_ActionType.manuever;
    public override List<string> Effects => new List<string> { "signature" };
    public override int Range => 10;

    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        if(data.target.entity.CompareTag("Hero"))
        {
            MB_Hero friend = (MB_Hero)data.target.entity;
            return new CS_AbilityReturnData(friend.SpendRecovery());
        }

        return new CS_AbilityReturnData(false);
    }
}

public class A_RayOfWrath : CS_Ability
{
    public override string Name => "Ray of Wrath";
    public override string Description => "A blast of holy light";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "free", "ranged" };
    public override int Range => 10;

    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {

        CS_Characteristics stats = data.actor.sheet.stats;
        int favoredStat = stats.Intuition;

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
}
