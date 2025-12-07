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
        if(warrior)
        {
            return new A_SpearCharge();
        }else if(sniper){
            return new A_RangedFreeStrike();
        }

        return null;
    }
}
