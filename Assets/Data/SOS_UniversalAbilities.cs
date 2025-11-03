using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CreateAssetMenu(fileName = "SO_AbilityPack", menuName = "Scriptable Objects/AbilityPack")]
public class SO_UniversalAbilities : ScriptableObject
{
    public List<Ability> abilities = new List<Ability>(); 
    //public Ability FreeStrike = new Ability(1, AbilityFunc.FuncFreeStrike);
}
