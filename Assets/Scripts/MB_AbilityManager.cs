using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MB_AbilityManager : MonoBehaviour
{
    [SerializeField] SO_BattleManager BattleManager;
    [SerializeField] Transform ContentHolder;
    [SerializeField] GameObject AbilityPrefab;

    private E_ActionType typeToDisplay = E_ActionType.move;

    private List<GameObject> displayedAbilties = new List<GameObject>();

    private void OnEnable()
    {
        BattleManager.EventActivateActor.AddListener(LoadAbilities);
        BattleManager.EventTurnEnd.AddListener(ClearAbilities);
    }

    public void LoadAbilities(CS_ActorTurnStats turn)
    {
        ClearAbilities();

        //Hardcode, Monsters don't trigger this
        if(turn.actor.CompareTag("Monster"))
        {
            return;
        }



       

        foreach(CS_Ability ability in turn.actor.abilities)
        {
            if(ability.Type != typeToDisplay)
            { continue;}

            if (turn.abilityTagRestrict != null && !ability.Effects.Contains(turn.abilityTagRestrict))
            { continue; }

            GameObject obj = Instantiate(AbilityPrefab, ContentHolder);

            Button abilityButton = obj.GetComponent<Button>();
            abilityButton.onClick.AddListener(delegate { BattleManager.StartLookingForTarget(ability); });

            MB_Ability abilityInstance = obj.GetComponent<MB_Ability>();
            abilityInstance.Ability = ability;

            abilityInstance.UpdateText();

            displayedAbilties.Add(obj);
        }
    }

    public void ClearAbilities()
    {
        foreach (GameObject ability in displayedAbilties)
        {
            DestroyImmediate(ability);
        }
    }

    public void SetAbilityType(SO_AbilityType newType)
    {
        typeToDisplay = newType.actionType;
        if(BattleManager.activeActor != null)
        {
            LoadAbilities(BattleManager.temporaryReferenceToActiveActor);
        }
        
    }
}
