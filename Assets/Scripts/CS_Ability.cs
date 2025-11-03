using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public abstract class Ability
{
    public abstract int Range { get; }
    //public abstract List<char> Characteristics = new List<char>();

    //There is a set list of tags, probably can be enums
    //public abstract List<string> Tags = new List<string>();

    //Action Cost
    //public abstract string ActionType;

    //Target will likely be another enum that referes to like "Single Target Enemy" or "a location"
    //This will be a bespoke function for each ability that intakes the target and direct acts upon it


    public abstract void Use(Tile target);
}

public class A_FreeStrike : Ability
{
    public override int Range => 1;


    public override void Use(Tile target)
    {
        Debug.Log("Free Strike " + target.entity);
        target.entity.TakeDamage(3);
    }
}
