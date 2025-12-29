using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    public List<SO_AI> behaviors;


    public List<CS_Ability> LoadAbilities()
    {
        List<CS_Ability> abilities = new List<CS_Ability>();
        foreach(SO_AbilityPack abilityPack in abilityPacks)
        {
            abilities.AddRange(abilityPack.Abilities);
        }

        return abilities;
    }

    public List<GameInput> RunBehavior(SO_User user, List<MB_Actor> potentialTargets)
    {
        //Target hero which is in closest range

        //Take preferred move and do closest square calculations out here, to then pass in

        //Attack closest target
        MB_Actor self = user.activeTurn.actor;
        MB_Actor target = null;
        int closestCount = 999;

        foreach(MB_Actor actor in potentialTargets)
        {

            int distanceCount = CS_GridUtility.FindPath(actor.currentTile, self.currentTile).Count;

            if(distanceCount < closestCount)
            {
                target = actor;
                closestCount = distanceCount;
            }
            
        }


        for (int i = 0; i < behaviors.Count; i++)
        {
            //Run through the behaivor list again until all behaviors come up false;


            List<GameInput> aiActions = behaviors[i].RunBehavior(user.activeTurn, target);

            if (aiActions != null) { return aiActions; }

        }



        return null;

        
    }
}

/// return a list of action commands to the ui?
/// move: tile, selectAbility: ability, target: tile, resolve: tile
///
