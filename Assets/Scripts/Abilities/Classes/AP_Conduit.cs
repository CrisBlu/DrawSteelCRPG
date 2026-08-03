using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AP_Conduit", menuName = "Scriptable Objects/AbilityPacks/Classes/Conduit")]
public class AP_Conduit : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_Lightfall(), new A_HealingGrace(), new A_RayOfWrath()};
}


public class A_Lightfall : CS_Ability, ITieredAbility
{
    public override string Name => "Lightfall";
    public override string Description => "A rain of holy light scours your enemies and repositions your allies.";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "signature" };
    public override int Range => 0;
    public List<E_Stats> BonusStat => new() { E_Stats.I };


    public async override Task<bool> Use(int tier = 0)
    {
        List<Tile> targetedTiles;
        List<MB_Actor> targetedActors;
        CS_Characteristics stats = Owner.sheet.stats;
        int damage = 0;
        int distance = 2;



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

        CS_AoeReturnData TilesAndActors = CS_GridUtility.GetTilesAndActorsWithin(Owner.currentTile, distance);
        targetedActors = TilesAndActors.affectedActors;
        targetedTiles = TilesAndActors.affectedArea;

        List<MB_Actor> friends = new List<MB_Actor>();

        foreach (MB_Actor target in targetedActors)
        {
            if (Owner.CompareTag(target.tag))
            {
                friends.Add(target);
            }
            else
            {
                SO_BattleEvents.AddRequest(new RequestDamage(target.currentTile, damage));
            }
        }

        foreach (MB_Actor target in friends)
        {
            SO_BattleEvents.AddRequest(new RequestMovmentWithInput(target, targetedTiles, E_MoveType.teleport));
        }

        return true;


    }

    public override CS_AbilityTargetingData Target(Tile origin)
    {

        return new CS_AbilityTargetingData(CS_GridUtility.GetTilesFromOrigin(origin, 2, false), new List<Tile>() { origin });


    }


}

public class A_HealingGrace : CS_Ability
{
    public override string Name => "Healing Grace";
    public override string Description => "A Divine Heal";
    public override E_ActionType Type => E_ActionType.maneuver;
    public override List<string> Tags => new List<string> { "signature" };
    public override int Range => 10;

    public async override Task<bool> Use(int tier = 0)
    {
        if (targets[0].entity.CompareTag("Hero"))
        {
            MB_Actor friend = targets[0].entity as MB_Actor;
            return true;
        }

        return true;
    }
}

public class A_RayOfWrath : CS_Ability, ITieredAbility
{
    public override string Name => "Ray of Wrath";
    public override string Description => "A blast of holy light";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "free", "ranged" };
    public override int Range => 10;
    public List<E_Stats> BonusStat => new() { E_Stats.I };

    public async override Task<bool> Use(int tier = 0)
    {

        CS_Characteristics stats = Owner.sheet.stats;
        int favoredStat = stats.Intuition;

        int damage = favoredStat;

        switch (tier)
        {
            case 1:
                damage += 2;
                break;

            case 2:
                damage += 4;
                break;

            case 3 or 4:
                damage += 6;
                break;

        }

        SO_BattleEvents.AddRequest(new RequestDamage(targets[0], damage));

        return true;
    }
}