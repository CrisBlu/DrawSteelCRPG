using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AP_BaseHero", menuName = "Scriptable Objects/AbilityPacks/AP_BaseHero")]
public class AP_BaseHero : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_MeleeFreeStrike() };
}
