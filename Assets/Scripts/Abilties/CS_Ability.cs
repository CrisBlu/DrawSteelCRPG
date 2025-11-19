
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class CS_Ability
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract E_ActionType Type { get;  }
    public abstract List<string> Effects { get; }
    public abstract int Range { get; }

    //The tile array will be used for extra data for the abilities that need it
    //This will have to get more complicated but for now, Tile[0] the main target, Tile[1] is reserved for Forced Movement, and Tile[2] and on is for
    public abstract CS_AbilityReturnData Use(CS_AbilityInputData data);

}
public class CS_AbilityInputData
{
    public MB_Actor actor;
    public Tile target;

    public CS_AbilityInputData(MB_Actor actorUsingAbility, Tile targetedTile)
    {
        actor = actorUsingAbility;
        target = targetedTile;
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
    public Action<MB_Actor, Tile> abilityCallback = null;
    public MB_Actor target = null;


    public CS_CallbackData(Action<MB_Actor, Tile> callback = null, MB_Actor currentTarget = null)
    {
        abilityCallback = callback;
        target = currentTarget;


    }
}

public class A_MeleeFreeStrike : CS_Ability
{
    public override string Name => "Melee Free Strike";
    public override string Description => "A simple strike";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new();
    public override int Range => 1;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        int tier = CS_DiceRoller.PowerRoll();

        Debug.Log("Melee Free Strike " + data.target.entity + "A tier " + tier);

        switch (tier)
        {
            case 1:
                data.target.entity.TakeDamage(2);
                break;

            case 2:
                data.target.entity.TakeDamage(5);
                break;

            case 3 or 4:
                data.target.entity.TakeDamage(7);
                break;

        }

        return new CS_AbilityReturnData(true);
    }
}

public class A_RangedFreeStrike: CS_Ability
{
    public override string Name => "Ranged Free Strike";
    public override string Description => "A pot shot";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new List<string> { "ranged" };
    public override int Range => 5;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {


        //Code to determine if you get a bane while doing a ranged attack; almost certain should be somewhere where any ranged ability could access it ----------------------------------------------------------
        List<Tile> nextTo = data.actor.currentTile.FindNeighbors(data.actor.gridSystem);
        int bane = 0;

        foreach(Tile neighbor in nextTo)
        {
            //This sucks and really, I should just be able to use tags
            if(neighbor.entity && neighbor.entity.GetType().IsSubclassOf(typeof(MB_Actor)))
            {
                //If actor has different tag from entity in neighbor tile
                if(!data.actor.CompareTag(neighbor.entity.tag))
                {
                    bane = 1;
                    break;
                }
            }
        }
        //Code section ends here -----------------------------------------------------------------------------------

        int tier = CS_DiceRoller.PowerRoll(0, 0, bane);

        Debug.Log("Ranged Free Strike " + data.target.entity + "A tier " + tier);

        switch (tier)
        {
            case 1:
                data.target.entity.TakeDamage(2);
                break;

            case 2:
                data.target.entity.TakeDamage(5);
                break;

            case 3 or 4:
                data.target.entity.TakeDamage(7);
                break;

        }


        return new CS_AbilityReturnData(true);
    }

}

public class A_Knockback : CS_Ability
{
    public override string Name => "Knockback";
    public override string Description => "Push your target back";
    public override E_ActionType Type => E_ActionType.manuever;
    public override List<string> Effects => new List<string> { "push" };
    public override int Range => 1;


    public override CS_AbilityReturnData Use(CS_AbilityInputData data)
    {
        Queue<CS_CallbackData> callbackList = new();

        MB_Actor targetActor = (MB_Actor)data.target.entity;
        callbackList.Enqueue(new CS_CallbackData(KnockbackActor, targetActor));

        
        return new CS_AbilityReturnData(true, callbackList);
    }

    private void KnockbackActor(MB_Actor target, Tile destination)
    {
        target.ForcedMovement(destination, 3);
    }
}

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
}

/*
 *  public override string Name => "";
    public override string Description => "";
    public override E_ActionType Type => E_ActionType.;
    public override List<string> Effects => new();
    public override int Range => ;


    public override void Use(Tile[] target)
    {
        
    }

 * 
 */