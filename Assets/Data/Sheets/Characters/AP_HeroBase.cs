using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AP_HeroBase", menuName = "Scriptable Objects/Ability Packs/Hero Base")]
public class AP_HeroBase : SO_AbilityPack
{
    public override List<CS_Ability> Abilities => new List<CS_Ability> { new A_MeleeFreeStrike(), new A_RangedFreeStrike(), new A_CatchBreath() };
}
