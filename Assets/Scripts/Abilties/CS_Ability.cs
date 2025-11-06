
using System.Collections.Generic;
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
    public abstract void Use(Tile[] target);

    //Dictionary<string, Tile[]>
    // Target | Tile[1,1]
    // Tar


    //Targets | Tile[All tiles targets are standing on]
    //ForcedMove | Tile[A new tile for each target]
    //
}

public class A_MeleeFreeStrike : CS_Ability
{
    public override string Name => "Melee Free Strike";
    public override string Description => "A simple strike";
    public override E_ActionType Type => E_ActionType.main;
    public override List<string> Effects => new();
    public override int Range => 1;


    public override void Use(Tile[] target)
    {
        Debug.Log("Free Strike " + target[0].entity);
        target[0].entity.TakeDamage(3);
    }
}

public class A_Knockback : CS_Ability
{
    public override string Name => "Knockback";
    public override string Description => "Push your target back";
    public override E_ActionType Type => E_ActionType.manuever;
    public override List<string> Effects => new List<string> { "push" };
    public override int Range => 1;


    public override void Use(Tile[] target)
    {
        int distance = 3;
        Debug.Log("PUUUUUSH");
        target[0].entity.ForcedMovement(target[1], distance);
    }
}