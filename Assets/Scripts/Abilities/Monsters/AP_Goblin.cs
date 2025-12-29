using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AP_Goblin", menuName = "Scriptable Objects/Ability Packs/Monsters/Goblins")]
public class AP_Goblin : SO_AbilityPack
{
    public bool warrior = true;
    public bool sniper = false;
    public override List<CS_Ability> Abilities => new List<CS_Ability> { ClassGoblin() };
    private CS_Ability ClassGoblin()
    {
        if (warrior)
        {
            return new A_SpearCharge();
        }
        else if (sniper)
        {
            return new A_Bow();
        }

        return null;
    }
}



public class A_SpearCharge : CS_Ability
{
    public override string Name => "Spear Charge";
    public override string Description => "The goblin rushes forward";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "charge", "melee", "strike" };
    public override int Range => 1;


    public override CS_AbilityReturnData Use(TurnData data)
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
}

public class A_Bow : CS_Ability
{
    public override string Name => "Bow";
    public override string Description => "deadass this is the only attack named bow";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "ranged", "weapon", "strike" };
    public override int Range => 10;


    public override CS_AbilityReturnData Use(TurnData data)
    {
        CS_Characteristics stats = data.actor.sheet.stats;
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;

        int edge = 0;
        if (data.actions[E_ActionType.move] == data.actor.Speed)
        {
            data.actions[E_ActionType.move] = 0;
            edge++;
            
        }

        int tier = CS_DiceRoller.PowerRoll(favoredStat, data.edges + edge, data.banes);

        switch (tier)
        {
            case 1:
                data.target.entity.TakeDamage(2);
                break;

            case 2:
                data.target.entity.TakeDamage(4);
                break;

            case 3 or 4:
                data.target.entity.TakeDamage(5);
                break;

        }

        return new CS_AbilityReturnData(true);
    }
}
