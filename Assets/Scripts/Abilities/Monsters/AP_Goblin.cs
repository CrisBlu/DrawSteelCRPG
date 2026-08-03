using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "AP_Goblin", menuName = "Scriptable Objects/AbilityPacks/Monsters/Goblins")]
public class AP_Goblin : SO_AbilityPack
{
    public bool warrior = true;
    public bool sniper = false;
    public override List<CS_Ability> Abilities => new List<CS_Ability> {new A_GoblinFreeStrike(), ClassGoblin() };
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



public class A_SpearCharge : CS_Ability, ITieredAbility
{
    public override string Name => "Spear Charge";
    public override string Description => "The goblin rushes forward";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "charge", "melee", "strike" };
    public override int Range => 1;
    public List<E_Stats> BonusStat => new() { E_Stats.M, E_Stats.A };


    public override async Task<bool> Use(int tier = 0)
    {
        CS_Characteristics stats = Owner.sheet.stats;
        int favoredStat = stats.Might >= stats.Agility ? stats.Might : stats.Agility;


        int damage = 0;
        switch (tier)
        {
            case 1:
                damage = 1;
                break;

            case 2:
                damage = 2;
                break;

            case 3 or 4:
                damage = 3;
                break;

        }

        SO_BattleEvents.AddRequest(new RequestDamage(targets[0], damage + favoredStat));

        return true;
    }
}

public class A_Bow : CS_Ability, ITieredAbility
{
    public override string Name => "Bow";
    public override string Description => "deadass this is the only attack named bow";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Tags => new List<string> { "ranged", "weapon", "strike" };
    public override int Range => 10;
    public List<E_Stats> BonusStat => new() { E_Stats.M, E_Stats.A};

    public override async Task<bool> Use(int tier = 0)
    {


        /*int edge = 0;
        if (data.actions[E_ActionType.move] == data.actor.Speed)
        {
            data.actions[E_ActionType.move] = 0;
            edge++;

        }*/

        int damage = 0;
        switch (tier)
        {
            case 1:
                damage = 2;
                break;

            case 2:
                damage = 4;
                break;

            case 3 or 4:
                damage = 5;
                break;

        }

        SO_BattleEvents.AddRequest(new RequestDamage(targets[0], damage));
        return true;
    }
}

public interface ITrigger
{
    public void SetTrigger(SO_BattleEvents events, MB_Actor user);
}

public class A_GoblinFreeStrike : CS_Ability, ITrigger
{
    public override string Name => "Free Strike";
    public override string Description => "a pot shot";
    public override E_ActionType Type => E_ActionType.trigger;
    public override List<string> Tags => new List<string> { "charge", "melee", "strike" };
    public override int Range => 1;

    MB_Actor user;

    public override async Task<bool> Use(int tier = 0)
    {

        Debug.Log("Free strike");
        //data.target.entity.TakeDamage(2);

        return true;
    }

    public async void Trigger(Tile exit, Tile entered, MB_Actor actor)
    {
        //This should be, check your neighbors and see if the tile exited is one of yours, then see if the tile entered is also one of yours
        //This ends up finding the path from every enemy to the user's tile exited and entered, which will probably be more work than the former approach.
        if (CS_GridUtility.FindShortestPath(entered, user.currentTile).Count <= Range) { return; }

        if (CS_GridUtility.FindShortestPath(exit, user.currentTile).Count > Range) { return; }

        

        if (!actor.CompareTag(user.tag))
        {
            //await actor.TakeDamage(2);
        }


        

    }

    public void SetTrigger(SO_BattleEvents events, MB_Actor user)
    {
        this.user = user;
        events.EventActorLeftTile += Trigger;
        
    }
}


