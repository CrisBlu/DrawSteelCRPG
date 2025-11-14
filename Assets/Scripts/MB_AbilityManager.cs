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
    }

    public void LoadAbilities(MB_Actor actor)
    {
        foreach (GameObject ability in displayedAbilties)
        {
            DestroyImmediate(ability);
        }

        foreach(CS_Ability ability in actor.abilities)
        {
            if(ability.Type != typeToDisplay)
            {
                continue;
            }

            GameObject obj = Instantiate(AbilityPrefab, ContentHolder);

            Button abilityButton = obj.GetComponent<Button>();
            abilityButton.onClick.AddListener(delegate { BattleManager.StartLookingForTarget(ability); });

            MB_Ability abilityInstance = obj.GetComponent<MB_Ability>();
            abilityInstance.Ability = ability;

            abilityInstance.UpdateText();

            displayedAbilties.Add(obj);
        }
    }

    public void SetAbilityType(SO_AbilityType newType)
    {
        typeToDisplay = newType.actionType;
        if(BattleManager.activeActor != null)
        {
            LoadAbilities(BattleManager.activeActor);
        }
        
    }
}
