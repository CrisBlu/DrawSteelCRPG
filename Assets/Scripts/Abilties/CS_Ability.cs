using UnityEditor.Playables;
using UnityEngine;

public abstract class CS_Ability
{
    public abstract string Name { get; }
    public abstract string Description { get; }
    public abstract int Range { get; }
    public abstract void Use(Tile target);
}

public class A_MeleeFreeStrike : CS_Ability
{
    public override string Name => "Melee Free Strike";
    public override string Description => "A simple strike";
    public override int Range => 1;


    public override void Use(Tile target)
    {
        Debug.Log("Free Strike " + target.entity);
        target.entity.TakeDamage(3);
    }
}
