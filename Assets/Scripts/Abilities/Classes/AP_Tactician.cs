using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AP_Tactician", menuName = "Scriptable Objects/AbilityPacks/Classes/Tactician")]
public class AP_Tactician : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_Parry() };

}


public class A_Parry : CS_Ability
{
    public override string Name => "Parry";
    public override string Description => "You lost the moment you entered these woods";
    public override E_ActionType Type => E_ActionType.trigger;
    public override List<string> Tags => new List<string> { "weapon", "melee"};
    public override int Range => 2;




    public override CS_AbilityReturnData Use(TurnData data)
    {


        Debug.Log("Parry!");

        return new CS_AbilityReturnData(true);
    }
}