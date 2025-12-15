using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CS_Characteristics
{
    public int Might = 0;
    public int Agility = 0;
    public int Reason = 0;
    public int Intuition = 0;
    public int Presence = 0;


    
    public CS_Characteristics(int M = 0, int A = 0, int R = 0, int I = 0, int P = 0)
    {
        Might = M;
        Agility = A;
        Reason = R;
        Intuition = I;
        Presence = P;
    }
}

[CreateAssetMenu(fileName = "SO_CharacterSheet", menuName = "Scriptable Objects/CharacterSheet")]
public class SO_CharacterSheet : ScriptableObject
{
    public CS_Characteristics stats = new CS_Characteristics();
    public List<SO_AbilityPack> abilityPacks;


    public List<CS_Ability> LoadAbilities()
    {
        List<CS_Ability> abilties = new List<CS_Ability>();
        foreach(SO_AbilityPack abilityPack in abilityPacks)
        {
            abilties.AddRange(abilityPack.Abilities);
        }

        return abilties;
    }
}
